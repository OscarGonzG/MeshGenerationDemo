using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    private TerrainManager terrainMgr { get; set; }

    private static float TERRAIN_VALUE = 1f;
    private static float AIR_VALUE = -1f;
    private static float ISOLEVEL = 0.5f;

    // Start is called before the first frame update
    public void Start()
    {
        Generate();
    }

    public void Generate()
    {
        terrainMgr = TerrainManager.Instance;

        Vector2 origin = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
        float[,] heightMap = NoiseMap.NoiseMap2D(terrainMgr.ChunkWidth, terrainMgr.ChunkWidth,
                                                 terrainMgr.ChunkHeight / 3, terrainMgr.Seed,
                                                 terrainMgr.TerrainNoise, origin);
        float[,,] scalarField = NoiseMap.HeightMapTo3D(heightMap, terrainMgr.ChunkHeight, TERRAIN_VALUE, AIR_VALUE);
        Mesh mesh = MarchingCubes.GenerateMesh(scalarField, ISOLEVEL);

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }
}
