using System;
using UnityEngine;

public class SandErosionTest : MonoBehaviour
{
    public ComputeShader sandComputeShader;
    public Texture2D sourceTexture;
    public RenderTexture resultTexture;

    void Start()
    {



        Graphics.Blit(sourceTexture, resultTexture);
        sandComputeShader.SetTexture(0, "Height", resultTexture);
        //sandComputeShader.SetTexture(0, "resultTex", resultTexture);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            sandComputeShader.Dispatch(0, sourceTexture.width / 8, sourceTexture.height / 8, 1);
        }
    }
}
