using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct RockyPlanetSettings
{
    public float temperature;
    public float humidity;
    public float seaLevel;
    public float biomass;
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlanetTerrain : MonoBehaviour
{
    private static ComputeShader computeShader;
    private MeshFilter meshFilter;

    [Min(0)]
    public int Subdivisions;
    public float Radius;

    [SerializeField] private Vector3 noiseOffset;
    [SerializeField] private SimpleNoiseSettings mountainNoise;
    [SerializeField] private SimpleNoiseSettings warpingNoise;

    /// <summary>
    /// Generates the planet
    /// </summary>
    public void GenerateTerrain()
    {
        // Generates the planet mesh
        meshFilter.mesh = OctahedronSphere.GenerateSphere(Subdivisions, 1);
        // Modifies the planet mesh
        Mesh mesh = meshFilter.sharedMesh;

        DisplaceVertices(mesh, Radius);
    }

    public void DeleteTerrain()
    {
        if (meshFilter is null)
        {
            // Necessary if Start() hasn't been called
            meshFilter = gameObject.GetComponent<MeshFilter>();            
        }
        meshFilter.mesh = null;
    }

    /// <summary>
    /// Loads the static resources for this class.
    /// </summary>
    public static void LoadStaticResources()
    {
        computeShader = (ComputeShader) Resources.Load("ComputeShaders/PlanetGeneration");
    }

    private void DisplaceVertices(Mesh mesh, float planetRadius)
    {
        // Sends vertex data to the GPU
        Vector3[] vertices = mesh.vertices;
        ComputeBuffer vertBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3);
        vertBuffer.SetData(vertices);
        computeShader.SetBuffer(computeShader.FindKernel("GeneratePlanet"), "vertices", vertBuffer);
        
        computeShader.SetFloat("PlanetRadius", planetRadius);

        computeShader.SetFloats("NoiseOffset", new float[] { noiseOffset.x, noiseOffset.y, noiseOffset.z });

        // Sets noise settings
        SimpleNoiseSettings[] simpleNoiseSettings = { mountainNoise, warpingNoise };
        ComputeBuffer perlinSettingsBuffer = new ComputeBuffer(simpleNoiseSettings.Length, sizeof(float) * 4 + sizeof(int));
        
        computeShader.SetBuffer(computeShader.FindKernel("GeneratePlanet"), "PerlinSettings", perlinSettingsBuffer);
        perlinSettingsBuffer.SetData(simpleNoiseSettings);


        // Dispatches the shader
        computeShader.Dispatch(computeShader.FindKernel("GeneratePlanet"), (vertices.Length/64) + 1, 1, 1);

        vertBuffer.GetData(vertices);

        // Releases compute buffers
        vertBuffer.Release();
        perlinSettingsBuffer.Release();

        // Applies changes
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    public void Start()
    {
        PlanetTerrain.LoadStaticResources();
        // Gets a reference for the mesh filter and renderer
        meshFilter = (MeshFilter) gameObject.GetComponent<MeshFilter>();

        GenerateTerrain();

    }
}
