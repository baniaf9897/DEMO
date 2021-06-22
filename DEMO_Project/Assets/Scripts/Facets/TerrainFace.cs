using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{

    ShapeGenerator shapeGenerator;
    Vector3[] origVertices;
    Mesh mesh;
    int resolution;
    public  Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;
    public RandomManager randomManager;

    bool random = false;
    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp, bool random)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.random = random;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);

        Debug.Log(axisA);
        InitRandomManager();
    }

    void InitRandomManager()
    {
        randomManager.m_frequency = 0.1f;
        randomManager.m_gain = 0.1f;
        randomManager.m_lacunarity = 0.1f;
        randomManager.m_NumOctaves = 2;
        randomManager.m_offsetX = 2;
        randomManager.m_offsetY = 4;
        randomManager.m_offsetZ = 1;
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        origVertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public void DeformMesh()
    {
        Vector3[] vertices = origVertices.Clone() as Vector3[];

        for (int i = 0; i < vertices.Length ; i++)
        {
            vertices[i] *= GetSphereHeight(vertices[i], randomManager);
        }
      
      
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.Optimize();

    }
    public float Get3DHeight(float x, float y, float z, RandomManager randomManager)
    {
        return randomManager.FBM3D(x, y, z);
    }


    public float GetSphereHeight(Vector3 worldpos, RandomManager randomManager)
    {
        float height = 2.0f * (Get3DHeight(worldpos.x, worldpos.y, worldpos.z, randomManager));
        return height;
     }
    }
