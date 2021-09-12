using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : MonoBehaviour
{
    private int NUMTHREADS_X = 8;   //No changy this...
    private int NUMTHREADS_Y = 8;   //nor this...           (Unless you know what you are doing of course)

    public ComputeShader GGol;
    public Texture2D input;
    private RenderTexture board;
    private RenderTexture previousBoard;

    public int boardWidth = 512;
    public int boardHeight = 512;

    public bool enableSim = false;

    //rules meaning (indexed by number of neighbor cells alive):
    //0: alive = dead   ||  dead = dead
    //1: alive = dead   ||  dead = alive
    //2: alive = alive  ||  dead = dead
    //3: alive = alive  ||  dead = alive
    public int[] rules = {0, 0, 2, 3, 0, 0, 0, 0, 0};   //Conway's Game of Life rules

    private int kernelID;

    void Start()
    {
        kernelID = GGol.FindKernel("GGOL");
        CreateTextures();
        GGol.SetTexture(kernelID, "result", board);
        GGol.SetTexture(kernelID, "previous", previousBoard);
        if(input != null && input.width == board.width && input.height == board.height) { Graphics.Blit(input, board); }
        UpdateShader();
    }

    void Update(){
        Graphics.Blit(board, previousBoard);
        if(enableSim) {
            GGol.Dispatch(kernelID, boardWidth / NUMTHREADS_X, boardHeight / NUMTHREADS_Y, 1);
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest){
        Graphics.Blit(board, dest);
    }

    private void CreateTextures(){
        board = new RenderTexture(boardWidth, boardHeight, 24);
        previousBoard = new RenderTexture(boardWidth, boardHeight, 24);
        board.enableRandomWrite = true;
        previousBoard.enableRandomWrite = true;
        board.wrapMode = TextureWrapMode.Repeat;
        previousBoard.wrapMode = TextureWrapMode.Repeat;
        board.useMipMap = false;
        previousBoard.useMipMap = false;
        board.filterMode = FilterMode.Point;
        previousBoard.filterMode = FilterMode.Point;
        board.Create();
        previousBoard.Create();
    }

    private void UpdateShader(){
        float[] deltaPixel = {1.0f / (float)boardWidth, 1.0f / (float)boardHeight};
        GGol.SetFloats("DPBuffer", deltaPixel);
        GGol.SetInts("rules", rules);
        GGol.SetFloat("width", (float)boardWidth);
        GGol.SetFloat("height", (float)boardHeight);
    }
}
