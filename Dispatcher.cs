//Attach this script to the Main Camera
//Attach GOL.compute to GGol ComputeShader in the inspector
//Attach Colorer.compute to Colorer ComputeShader in the inspector
//Attach Biewport.compute to Viewport ComputeShader in the inspector

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : MonoBehaviour
{
    private static int NUMTHREADS_X = 8;   //No changy this...
    private static int NUMTHREADS_Y = 8;   //nor this...           (Unless you know what you are doing of course)

    public Texture2D input;

    public ComputeShader GGol;
    public ComputeShader Colorer;
    public ComputeShader Viewport;

    private RenderTexture board0;
    private RenderTexture board1;
    private RenderTexture colored;
    private RenderTexture output;

    public int boardWidth = 512;
    public int boardHeight = 512;
    public float simSpeed = 1.0f;
    public float steppingFactor = 0.1f;
    public float zoom = 100.0f; //board pixels
    public int enableTiling = 1;
    public bool[] centerViewport = {false, false};
    public float[] viewportCoords = {0.0f, 0.0f};   //board coordinates
    public bool enableSim = false;
    public string rule = "B3S23";
    public float[] mask = {1.0f, 1.0f, 1.0f, 1.0f};
    public string colorScheme = "Vanilla";
    public float[] colors = {1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1};

    private Dictionary<string, int> kernelColorer = new Dictionary<string, int>();

    private int kernelGGOL;
    private int kernelViewport;
    private float nextUpdate;
    private bool boardState = false;
    private int[] rules = {0, 0, 2, 3, 0, 0, 0, 0, 0};   //Conway's Game of Life rules

    void Start()
    {
        if(input == null) { print("no input image!"); }
        else {
            boardWidth = input.width;
            boardHeight = input.height;
        }
        CreateTextures();
        Graphics.Blit(input, board0);
        Graphics.Blit(input, board1);
        StartShaders();
        nextUpdate = Time.time;
    }

    void Update(){
        if(enableSim){
            while(nextUpdate <= Time.time) {
                nextUpdate += Mathf.Abs(simSpeed);
                bool newRules = ParseRules();
                if(boardState){
                    GGol.SetTexture(kernelGGOL, "result", board0);
                    GGol.SetTexture(kernelGGOL, "input", board1);
                    boardState = false;
                }
                else {
                    GGol.SetTexture(kernelGGOL, "result", board1);
                    GGol.SetTexture(kernelGGOL, "input", board0);
                    boardState = true;
                }
                GGol.SetInts("rules", PadRules(rules));
                GGol.SetFloat("steppingFactor", steppingFactor);
                GGol.SetFloats("mask", mask);
                GGol.Dispatch(kernelGGOL, boardWidth / NUMTHREADS_X, boardHeight / NUMTHREADS_Y, 1);
            }
        }
        else {
            nextUpdate = Time.time;
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest){
        if(output.width != Screen.width || output.height != Screen.height){
            output.Release();
            colored.Release();
            output = new RenderTexture(Screen.width, Screen.height, 24);
            colored = new RenderTexture(Screen.width, Screen.height, 24);
            output.enableRandomWrite = true;
            colored.enableRandomWrite = true;
            output.useMipMap = false;
            colored.useMipMap = false;
            output.Create();
            colored.Create();
        }
        if(boardState){ Viewport.SetTexture(kernelViewport, "board", board1); }
        else { Viewport.SetTexture(kernelViewport, "board", board0); }
        Viewport.SetFloats("viewportCoords", resolveViewportCoords());
        Viewport.SetFloat("deltaPixel", (zoom / (float)boardWidth) / (float)output.width);
        Viewport.SetInt("enableTiling", enableTiling);
        Viewport.SetTexture(kernelViewport, "output", output);
        Viewport.Dispatch(kernelViewport, output.width / NUMTHREADS_X, output.height / NUMTHREADS_Y, 1);

        int kernel = kernelColorer[colorScheme];
        Colorer.SetFloats("colors", colors);
        Colorer.SetFloats("deltaPixel", new float[2]{1.0f / colored.width, 1.0f / colored.height});
        Colorer.SetTexture(kernel, "input", output);
        Colorer.SetTexture(kernel, "output", colored);
        Colorer.Dispatch(kernel, colored.width / NUMTHREADS_X, colored.height / NUMTHREADS_Y, 1);

        Graphics.Blit(colored, dest);
    }

    private void CreateTextures(){
        board0 = new RenderTexture(boardWidth, boardHeight, 24);
        board1 = new RenderTexture(boardWidth, boardHeight, 24);
        colored = new RenderTexture(1, 1, 24);
        output = new RenderTexture(1, 1, 24);
        board0.enableRandomWrite = true;
        board1.enableRandomWrite = true;
        board0.useMipMap = false;
        board1.useMipMap = false;
        board0.filterMode = FilterMode.Point;
        board1.filterMode = FilterMode.Point;
        board0.Create();
        board1.Create();
        colored.Create();
        output.Create();
    }

    private void StartShaders(){
        
        float[] deltaPixel = {1.0f / (float)boardWidth, 1.0f / (float)boardHeight, 0.0f, 0.0f};
        GGol.SetFloats("deltaPixel", deltaPixel);
        GGol.SetFloat("width", (float)boardWidth);
        GGol.SetFloat("height", (float)boardHeight);
        kernelGGOL = GGol.FindKernel("GGOL");
        kernelViewport = Viewport.FindKernel("Viewport");

        kernelColorer.Add("Vanilla", Colorer.FindKernel("Vanilla"));
        kernelColorer.Add("Raw", Colorer.FindKernel("Raw"));
        kernelColorer.Add("Lerp", Colorer.FindKernel("InterpolateLinear"));
        kernelColorer.Add("Ratio", Colorer.FindKernel("Ratio"));
    }

    private int[] PadRules(int[] rulesIn){
        int[] paddedArray = new int[rulesIn.Length * 4];
        for(int i = rulesIn.Length - 1; i >= 0; i--){ 
            int paddedIndex = i * 4;
            for(int j = 0; j < 3; j++){
                paddedArray[paddedIndex + j] = 0;
            }
            paddedArray[paddedIndex + 3] = rules[i];
        }
        return paddedArray;
    }

    //Converts string based rule into internal "rules". Returns whether rules have changed  
    private bool ParseRules(){ 
        int[] newRules = {0, 0, 0, 0, 0, 0, 0, 0, 0};
        bool born = false;
        bool survive = false;
        bool validRules = true;
        bool rulesChanged = false;

        foreach(char character in rule){
            if(character == 'B' || character == 'b'){
                born = true;
                survive = false;
                continue;
            }
            if(character == 'S' ||character == 's'){
                born = false;
                survive = true;
                continue;
            }
            //rules meaning (indexed by number of neighbor cells alive):
            //0: alive = dead   ||  dead = dead
            //1: alive = dead   ||  dead = alive
            //2: alive = alive  ||  dead = dead
            //3: alive = alive  ||  dead = alive
            int num = (int)char.GetNumericValue(character);
            if(num < 0 || num > 8){
                print("Malformed rules!");
                validRules = false;
                break;
            }
            else {
                if(born){ newRules[num] += 1; }
                else if(survive){ newRules[num] += 2; }
                else{
                    print("Malformed rules!");
                    validRules = false;
                    break;
                }
            }
        }
        if(validRules){
            for(int i = rules.Length - 1; i >= 0; i--){
                if(newRules[i] < 0 || newRules[i] > 3){
                    rulesChanged = false;
                    print("Malformed rules!");
                    break;
                }
                if(rules[i] != newRules[i]){
                    rulesChanged = true;
                    break;
                }
            }
            if(rulesChanged){
                rules = newRules;
            }
        }
        return rulesChanged;
    }

    private float[] resolveViewportCoords(){
        float xcoord;
        float ycoord;
        if(centerViewport[0]){ xcoord = (viewportCoords[0] - (zoom / 2.0f)) * (1.0f / boardWidth); } 
        else { xcoord = viewportCoords[0] * (1.0f / boardWidth); }
        if(centerViewport[1]){ ycoord = (viewportCoords[1] - (zoom / 2.0f)) * (1.0f / boardHeight); }
        else { ycoord = viewportCoords[1] * (1.0f / boardHeight); }
        return new float[2]{xcoord, ycoord};
    }
}








