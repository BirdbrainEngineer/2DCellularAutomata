using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : MonoBehaviour
{
    public ComputeShader gol;
    public RenderTexture board;
    private RenderTexture previousBoard;

    public int BoardWidth = 512;
    public int BoardHeight = 512;

    public bool EnableSim = false;

    void Start()
    {
        if(board == null){
            
        }
    }

    private void OnRenderImage(){

    }

    private void createTextures(){
        board = new RenderTexture(BoardWidth, BoardHeight, 24);
        previousBoard = new RenderTexture(BoardWidth, BoardHeight, 24);
        board.enableRandomWrite = true;
        board.Create();
    }
}
