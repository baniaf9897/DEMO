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

    public float m_ridges;


    public void addFreq(float freq)
    {
        if(m_frequency + freq < OSCReceiverFace.minFreq || m_frequency + freq > OSCReceiverFace.maxFreq)
        {
            Debug.Log("out of range freq" + m_frequency + freq);
            return;
        }

        m_frequency += freq;
    }

    public void addLuc(float luc)
    {
        if (m_lacunarity + luc < OSCReceiverFace.minLuc || m_lacunarity + luc > OSCReceiverFace.maxLuc)
        {
            Debug.Log("out of range luc" + m_lacunarity + luc);

            return;
        }

        m_lacunarity += luc;
    }

    public void addGain(float gain)
    {
        if (m_gain + gain < OSCReceiverFace.minGain || m_gain + gain > OSCReceiverFace.maxGain)
        {
            Debug.Log("out of range gain" + m_gain + gain);

            return;
        }

        m_gain += gain;
    }
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

    static float Ridgef(float h, float offset)
    {
        h = offset - Mathf.Abs(h);
        return (h * h);
    }

    public float RMF3D(float x, float y, float z)
    {
        return RMF3D(x + m_offsetX, y + m_offsetY, z +m_offsetZ, m_NumOctaves, m_frequency, m_gain, m_lacunarity, m_ridges);
    }
    public static float RMF3D(float x, float y, float z, int octaves, float frequency, float gain, float lacunarity, float offset)
    {
        float sum = 0.0f;
        float max = 0.0f;
        float prev = 1.0f;
        float amplitude = 0.5f;
        float maxo = Mathf.Max(Ridgef(0.0f, offset), Ridgef(1.0f, offset));
        for (int i = 0; i < octaves; i++)
        {

            float n = Ridgef(Noise3D(x, y, z , frequency) - 0.5f, offset);
            float f = amplitude * prev;
            sum += n * f;
            max += maxo * f;
            prev = n;
            frequency *= lacunarity;
            amplitude *= gain;
        }
        return (2.0f * sum / max) - 1.0f;
    }


}
