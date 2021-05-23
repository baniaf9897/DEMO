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

}
