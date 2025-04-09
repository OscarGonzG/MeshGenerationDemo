using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[Serializable]
public struct SimpleNoiseSettings
{
    public float amplitude;
    public float frequency;
    [Range(0, 1)]
    public float persistence;
    [Min(1)]
    public float lacunarity;

    [Range(0, 5)]
    public int octaves;
}

/// <summary>
/// Class for generating 2d and 3d noise maps. Based on
/// <see href="https://youtu.be/wbpMiKiSKm8?si=VQIN0wESPUaoq5oD">Sebastian Lague's tutorial series</see>.
/// </summary>
public static class NoiseMap
{

    /// <summary>
    /// Returns a 2D noise map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="baseHeight"></param>
    /// <param name="seed"></param>
    /// <param name="settings"></param>
    /// <param name="origin">the coordinates of the center of the map</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
   public static float[,] NoiseMap2D(int x, int y, int baseHeight, long seed, SimpleNoiseSettings settings, Vector2 origin)
   {
        if (x <= 0)
        {
            throw new ArgumentException("Width must be positive");
        }
        if (y <= 0)
        {
            throw new ArgumentException("Height must be positive");
        }

        float[,] noiseMap = new float[x, y];

        for (int u = 0; u < x; u++)
        {
            for (int v = 0; v < y; v++)
            {

                float noiseValue = baseHeight;
                float pointAmplitude = settings.amplitude;
                float pointFrequency = 1;

                float sampleX = u + origin.x - (x/2);
                float sampleY = v + origin.y - (y/2);
                for (int i = 0; i < settings.octaves; i++)
                {
                    noiseValue += OpenSimplex2S.Noise2(seed, settings.frequency * pointFrequency * sampleX,
                                                                settings.frequency * pointFrequency * sampleY) * pointAmplitude;
                    pointAmplitude *= settings.persistence;
                    pointFrequency *= settings.lacunarity;
                }

                noiseMap[u, v] = noiseValue;
            }
        }
        return noiseMap;
   }

    /// <summary>
    /// Converts a height map to a 3D noise grid.
    /// </summary>
    /// <param name="heightMap"></param>
    /// <param name="maxHeight"></param>
    /// <returns></returns>
    public static float[,,] HeightMapTo3D(float[,] heightMap, int maxHeight, float terrainValue, float airValue)
    {
        float[,,] noiseGrid = new float[heightMap.GetLength(0), maxHeight + 1, heightMap.GetLength(1)];

        for (int x = 0; x < heightMap.GetLength(0); x++)
        {
            for (int z = 0; z < heightMap.GetLength(1); z++)
            {
                float localHeight = heightMap[x, z];
                if (localHeight > maxHeight)
                {
                    localHeight = maxHeight;
                }

                int y;
                for (y = 0; y < localHeight; y++)
                {
                    noiseGrid[x, y, z] = terrainValue;
                }

                if (y < maxHeight)
                {
                    noiseGrid[x, y, z] = localHeight - (int) localHeight;
                }

                while (y < maxHeight - 1)
                {
                    y++;
                    noiseGrid[x, y, z] = airValue;
                }
                
            }
        }

        return noiseGrid;
    }
}
