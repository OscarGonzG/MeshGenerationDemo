using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance { get; private set; } = null;
    [Range(1, 20)]
    public int ChunkGridSize;
    [Min (2)]
    public int ChunkWidth;
    [Min (1)]
    public int ChunkHeight;
    public int Seed;
    public SimpleNoiseSettings NoiseSettings;

    public GameObject ChunkPrefab;

    private GameObject[,] chunks;

    public void Start()
    {
        DeleteChunks();
        TerrainManager.Instance = this;

        chunks = new GameObject[ChunkGridSize, ChunkGridSize];

        Vector3 corner = new Vector3((float) (-ChunkGridSize * ChunkWidth) / 2, transform.position.y, (float) (-ChunkGridSize * ChunkWidth) / 2f);
        for (int i = 0; i < ChunkGridSize; i++)
        {
            for (int j = 0; j < ChunkGridSize; j++)
            {
                Vector3 position = corner + new Vector3(i * (ChunkWidth - 1), 0, j * (ChunkWidth - 1));
                chunks[i, j] = (GameObject) PrefabUtility.InstantiatePrefab(ChunkPrefab);
                chunks[i, j].name = "chunk" + i + "-" + j; 
                chunks[i, j].transform.position = position;
                chunks[i, j].GetComponent<Chunk>().Generate();
            }
        }
    }

    public void DeleteChunks()
    {
        int i, j;
        if (chunks != null)
        {
            for (i = 0; i < chunks.GetLength(0); i++)
            {
                for (j = 0; j < chunks.GetLength(1); j++)
                {
                    DestroyImmediate(chunks[i, j]);
                }
            }
        }

        i = 0;
        j = 0;
        GameObject chunk = GameObject.Find("chunk0-0");
        String chunkName = "chunk0-0"; 
        while (chunk != null)
        {
            DestroyImmediate(chunk);
            j++;
            chunkName = "chunk" + i + "-" + j;
            chunk = GameObject.Find(chunkName);
        }

        int oldSize = j;
        for (i = 1; i < oldSize; i++)
        {
            for (j = 0; j < oldSize; j++)
            { 
                chunkName = "chunk" + i + "-" + j;
                chunk = GameObject.Find(chunkName);
                DestroyImmediate(chunk);
            }
        }
    }
}
