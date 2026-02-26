using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TextureTest : MonoBehaviour
{
    [SerializeField] private ComputeShader textureComputeShader;
    private RenderTexture renderTexture;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(256, 256, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }

        textureComputeShader.SetTexture(0, "Result", renderTexture);
        textureComputeShader.SetFloat("Resolution", renderTexture.width);
        textureComputeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);

        Graphics.Blit(renderTexture, destination);
    }
}
