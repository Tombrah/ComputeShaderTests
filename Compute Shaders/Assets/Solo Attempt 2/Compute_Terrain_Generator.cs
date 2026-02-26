using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Compute_Terrain_Generator : MonoBehaviour
{
    [SerializeField, Range(2, 256)] private int resolution = 2;
    [SerializeField] private float scale = 1;
    [SerializeField] private bool isCentered;

    [SerializeField] ComputeShader cs;

    MeshFilter meshFilter;

    private void Start()
    {
        InitialiseMesh();
        GenerateMesh(meshFilter.sharedMesh, resolution, scale, transform, isCentered);
    }

    private void InitialiseMesh()
    {
        if (meshFilter == null)
        {
            GameObject meshObj = new GameObject("Terrain");
            meshObj.transform.parent = transform;

            meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            meshFilter = meshObj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
        }
    }

    private void GenerateMesh(Mesh mesh, int _resolution, float _scale, Transform localTransform, bool centered = true)
    {
        int triIndex = 0;
        Vector3[] vertices = new Vector3[_resolution * _resolution];
        int[] triangles = new int[_resolution * _resolution * 6];

        for (int y = 0; y < _resolution; y++)
        {
            for (int x = 0; x < _resolution; x++)
            {
                int i = x + _resolution * y;
                Vector2 percent = new Vector2(x, y) / (_resolution - 1);

                Vector3 pointOnPlane = (percent.x - 0.5f) * localTransform.right + (percent.y - 0.5f) * localTransform.forward;
                Vector3 pointOnPlaneNotCentered = percent.x * localTransform.right + percent.y * localTransform.forward;
                vertices[i] = centered ? pointOnPlane * _scale : pointOnPlaneNotCentered * scale;

                if (x != _resolution - 1 && y != _resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + _resolution;
                    triangles[triIndex + 2] = i + _resolution + 1;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + _resolution + 1;
                    triangles[triIndex + 5] = i + 1;

                    triIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }


    [ContextMenu("Randomize Heights")]
    private void RandomizeHeight()
    {
        Vector3[] vertices = meshFilter.sharedMesh.vertices;
        Debug.Log(vertices[232]);

        ComputeBuffer positionsBuffer = new ComputeBuffer(resolution * resolution, sizeof(float) * 3);
        positionsBuffer.SetData(vertices);
        cs.SetBuffer(0, "Positions", positionsBuffer);

        cs.SetInt("Resolution", resolution);
        cs.SetFloat("Step", 2f / resolution);
        cs.SetFloat("Time", Time.time);

        int groups = Mathf.CeilToInt(resolution / 32);
        cs.Dispatch(cs.FindKernel("SetTerrain"), groups, groups, 1);

        positionsBuffer.GetData(vertices);

        positionsBuffer.Release();

        meshFilter.sharedMesh.vertices = vertices;
    }
}
