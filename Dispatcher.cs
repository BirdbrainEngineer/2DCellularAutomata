using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : MonoBehaviour
{
    private int NUMTHREADS_X = 8;   //No changy this...
    private int NUMTHREADS_Y = 8;   //nor this...           (Unless you know what you are doing of course)

    public ComputeShader GGol;
    public RenderTexture Board;
    private RenderTexture PreviousBoard;

    public int boardWidth = 512;
    public int boardHeight = 512;

    public bool enableSim = false;

    //0=transition to dead, 1=transition to alive if dead, 2=survive if alive, 3=transition to alive if dead and survive if alive
    public int[] rules = {0, 0, 0, 0, 0, 0, 0, 0};

    private int kernelID;

    void Start()
    {
        kernelID = GGol.FindKernel("GGOL");
        CreateTextures();
        GGol.SetTexture(kernelID, "result", Board);
        GGol.SetTexture(kernelID, "previous", PreviousBoard);
        UpdateShader();
    }

    void Update(){
        Graphics.CopyTexture(Board, PreviousBoard);
        GGol.Dispatch(kernelID, boardWidth / NUMTHREADS_X, boardHeight / NUMTHREADS_Y, 1);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest){
        Graphics.Blit(Board, dest);
    }

    private void CreateTextures(){
        Board = new RenderTexture(boardWidth, boardHeight, 24);
        PreviousBoard = new RenderTexture(boardWidth, boardHeight, 24);
        Board.enableRandomWrite = true;
        Board.Create();
        PreviousBoard.Create();
    }

    private void UpdateShader(){
        GGol.SetFloat("width", (float)boardWidth);
        GGol.SetFloat("height", (float)boardHeight);
        GGol.SetInts("rules", rules);
    }
}
