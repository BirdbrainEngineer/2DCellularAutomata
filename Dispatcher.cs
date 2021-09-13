using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : MonoBehaviour
{
    private int NUMTHREADS_X = 8;   //No changy this...
    private int NUMTHREADS_Y = 8;   //nor this...           (Unless you know what you are doing of course)

    public ComputeShader GGol;
    public Texture2D input;
    private RenderTexture board0;
    private RenderTexture board1;

    public int boardWidth = 512;
    public int boardHeight = 512;
    public float simSpeed = 1.0f;

    public bool enableSim = false;

    //rules meaning (indexed by number of neighbor cells alive):
    //0: alive = dead   ||  dead = dead
    //1: alive = dead   ||  dead = alive
    //2: alive = alive  ||  dead = dead
    //3: alive = alive  ||  dead = alive
    public int[] rules = {0, 0, 2, 3, 0, 0, 0, 0, 0};   //Conway's Game of Life rules

    private int kernelGGOL;
    private int kernelViewport;

    private float nextUpdate;

    private bool boardState = false;

    void Start()
    {
        kernelGGOL = GGol.FindKernel("GGOL");
        CreateTextures();
        if(input != null && input.width == board0.width && input.height == board0.height) { 
            Graphics.Blit(input, board0);
            Graphics.Blit(input, board1);
        } else { print("Could not use input image - dimensions differ"); }
        StartGGOL();
        StartViewport();
        nextUpdate = Time.time + simSpeed;
    }

    void Update(){
        if(enableSim && nextUpdate <= Time.time) {
            nextUpdate = Time.time + simSpeed;
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
            GGol.Dispatch(kernelGGOL, boardWidth / NUMTHREADS_X, boardHeight / NUMTHREADS_Y, 1);
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest){
        if(boardState){ Graphics.Blit(board1, dest); }
        else { Graphics.Blit(board0, dest); }
    }

    private void CreateTextures(){
        board0 = new RenderTexture(boardWidth, boardHeight, 24);
        board1 = new RenderTexture(boardWidth, boardHeight, 24);
        board0.enableRandomWrite = true;
        board1.enableRandomWrite = true;
        board0.wrapMode = TextureWrapMode.Repeat;
        board1.wrapMode = TextureWrapMode.Repeat;
        board0.useMipMap = false;
        board1.useMipMap = false;
        board0.filterMode = FilterMode.Point;
        board1.filterMode = FilterMode.Point;
        board0.Create();
        board1.Create();
    }

    private void StartGGOL(){
        float[] deltaPixel = {1.0f / (float)boardWidth, 1.0f / (float)boardHeight, 0.0f, 0.0f};
        GGol.SetFloats("deltaPixel", deltaPixel);
        int[] paddedArray = new int[rules.Length * 4];  //making the rules array compatible with how the shader accepts data
        for(int i = rules.Length - 1; i >= 0; i--){ 
            int paddedIndex = i * 4;
            for(int j = 0; j < 3; j++){
                paddedArray[paddedIndex + j] = 0;
            }
            paddedArray[paddedIndex + 3] = rules[i];
        }
        GGol.SetInts("rules", paddedArray);
        GGol.SetFloat("width", (float)boardWidth);
        GGol.SetFloat("height", (float)boardHeight);
    }

    private void StartViewport(){

    }
}








