using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct RandomManager 
{
    [Range(1, 10)]
    public int m_NumOctaves;
    [Range(0.0f, 10.0f)]
    public float m_frequency;
    [Range(0.0f, 10.0f)]
    public float m_lacunarity;
    [Range(0.0f, 2.0f)]
    public float m_gain;
    [Range(-100.0f, 100.0f)]
    public float m_offsetX;
    [Range(-100.0f, 100.0f)]
    public float m_offsetY;
    [Range(-100.0f, 100.0f)]
    public float m_offsetZ;
    public float FBM(float x, float y)
    {
        return FBM(x + m_offsetX, y + m_offsetY, m_NumOctaves, m_frequency, m_gain, m_lacunarity);
    }

    public static float FBM(float x, float y, int octaves, float frequency, float gain, float lacunarity)
    {
        float sum = 0f;
        float range = 0f;
        float amplitude = 1f;

        for (int o = 0; o < octaves; o++)
        {
            sum += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
            range += amplitude;
            frequency *= lacunarity;
            amplitude *= gain;
        }
        if (range == 0.0f)
            return 0.0f;
        return sum / range;
    }

    public static float FBM3D (float x, float y,float z, int octaves, float frequency, float gain, float lacunarity)
    {

        float sum = 0f;
        float range = 0f;
        float amplitude = 1f;

        for (int o = 0; o < octaves; o++)
        {
            sum += Noise3D(x, y, z, frequency) * amplitude; // Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
           
            range += amplitude;
            frequency *= lacunarity;
            amplitude *= gain;
        }
        if (range == 0.0f)
            return 0.0f;
        return sum / range;
    }

    public float FBM3D(float x, float y, float z)
    {
        return FBM3D(x + m_offsetX, y + m_offsetY, z + m_offsetZ, m_NumOctaves, m_frequency, m_gain, m_lacunarity);
    }

    public static float Noise3D(float x ,float y, float z, float frequency)
    {
        // Get all permutations of noise for each individual axis
        float noiseXY = Mathf.PerlinNoise(x * frequency , y * frequency );
        float noiseXZ = Mathf.PerlinNoise(x * frequency , z * frequency );
        float noiseYZ = Mathf.PerlinNoise(y * frequency , z * frequency );

        // Reverse of the permutations of noise for each individual axis
        float noiseYX = Mathf.PerlinNoise(y * frequency , x * frequency );
        float noiseZX = Mathf.PerlinNoise(z * frequency , x * frequency );
        float noiseZY = Mathf.PerlinNoise(z * frequency , y * frequency );

        return (noiseXY + noiseXZ + noiseYZ + noiseYX + noiseZX + noiseZY) / 6.0f;
    }

}
