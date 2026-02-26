using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderTest : MonoBehaviour
{
    static readonly int PositionsId = Shader.PropertyToID("_Positions");
    static readonly int ResolutionId = Shader.PropertyToID("_Resolution");
    static readonly int StepId = Shader.PropertyToID("_Step");
    static readonly int TimeId = Shader.PropertyToID("_Time");

    [SerializeField] private Material material;
    [SerializeField] private Mesh mesh;

    [SerializeField] ComputeShader computeShader;
    private ComputeBuffer positionsBuffer;

    [SerializeField, Range(1, 1000)] private int resolution;

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(resolution * resolution, 12);
    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    private void Update()
    {
        UpdateFunctionOnGPU();
    }

    private void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(ResolutionId, resolution);
        computeShader.SetFloat(StepId, step);
        computeShader.SetFloat(TimeId, Time.time);

        computeShader.SetBuffer(0, PositionsId, positionsBuffer);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1);

        material.SetBuffer(PositionsId, positionsBuffer);
        material.SetFloat(StepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
    }
}
