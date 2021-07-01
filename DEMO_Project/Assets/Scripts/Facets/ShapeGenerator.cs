using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{

    public float height;
    public RandomManager randomManager;
    void InitRandomManager()
    {
        randomManager.m_frequency = 0.01f;
        randomManager.m_gain = 0.01f;
        randomManager.m_lacunarity = 0.01f;
        randomManager.m_NumOctaves = 6;
        randomManager.m_offsetX = -30.0f; //Random.Range(0,100);
        randomManager.m_offsetY = 99.0f; //Random.Range(-10, 100);
        randomManager.m_offsetZ = -10.0f;// Random.Range(10, 100);
        randomManager.m_ridges = 0.0f;
    }
    public ShapeGenerator(float height)
    {
        this.height = height;
        InitRandomManager();
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        pointOnUnitSphere *= GetSphereHeight(pointOnUnitSphere, randomManager);
        return pointOnUnitSphere * height;
    }

    public float Get3DHeight(float x, float y, float z, RandomManager randomManager)
    {
        return randomManager.FBM3D(x, y, z);
    }


    public float GetSphereHeight(Vector3 worldpos, RandomManager randomManager)
    {
        if(randomManager.m_frequency > 0.01f)
        {
            return 1.12f * (Get3DHeight(worldpos.x, worldpos.y, worldpos.z, randomManager));
        }
        else
        return  1.0f*  (Get3DHeight(worldpos.x, worldpos.y, worldpos.z, randomManager));
    }

}

