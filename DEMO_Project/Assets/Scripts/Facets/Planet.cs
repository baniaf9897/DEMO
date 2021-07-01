using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{

    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;

    public float height;

    ShapeGenerator shapeGenerator;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    GameObject[] faceGameObjects;

    int currentFace = 0;



    private Vector3[] upFacets =
    {
            Vector3.up + Vector3.left/4.0f + Vector3.back/4.0f,
            Vector3.up + Vector3.left/4.0f + Vector3.forward/4.0f,
            Vector3.up + Vector3.right/4.0f + Vector3.back/4.0f,
            Vector3.up + Vector3.forward/4.0f + Vector3.forward/4.0f,

        };

    private Vector3[] downFacets =
    {
            Vector3.down + Vector3.left/4.0f + Vector3.back/4.0f,
            Vector3.down + Vector3.left/4.0f + Vector3.forward/4.0f,
            Vector3.down + Vector3.right/4.0f + Vector3.back/4.0f,
            Vector3.down + Vector3.forward/4.0f + Vector3.forward/4.0f,


     };
    private Vector3[] leftFacets =
   {
            Vector3.left + Vector3.up/4.0f + Vector3.back/4.0f,
            Vector3.left + Vector3.up/4.0f + Vector3.forward/4.0f,
            Vector3.left + Vector3.down/4.0f + Vector3.back/4.0f,
            Vector3.left + Vector3.down/4.0f + Vector3.forward/4.0f,

     };
    private Vector3[] rightFacets =
    {
            Vector3.right + Vector3.up/4.0f + Vector3.back/4.0f,
            Vector3.right + Vector3.up/4.0f + Vector3.forward/4.0f,
            Vector3.right + Vector3.down/4.0f + Vector3.back/4.0f,
            Vector3.right + Vector3.down/4.0f + Vector3.forward/4.0f,

     };
    private Vector3[] backFacets =
    {
            Vector3.back + Vector3.up/4.0f + Vector3.left/4.0f,
            Vector3.back + Vector3.up/4.0f + Vector3.right/4.0f,
            Vector3.back + Vector3.down/4.0f + Vector3.left/4.0f,
            Vector3.back + Vector3.down/4.0f + Vector3.right/4.0f

     };
    private Vector3[] forwardFacets =
    {
            Vector3.forward + Vector3.up/4.0f + Vector3.left/4.0f,
            Vector3.forward + Vector3.up/4.0f + Vector3.right/4.0f,
            Vector3.forward + Vector3.down/4.0f + Vector3.left/4.0f,
            Vector3.forward + Vector3.down/4.0f + Vector3.right/4.0f

     };

     void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    private void Start()
    {
        Initialize();
        GenerateMesh();
    }
    void Initialize()
    {

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[24];
        }
        terrainFaces = new TerrainFace[24];

        Vector3[] directions = new Vector3[24];

        faceGameObjects = new GameObject[24]; 

        upFacets.CopyTo(directions, 0);
        downFacets.CopyTo(directions, 4);
        leftFacets.CopyTo(directions, 8);
        rightFacets.CopyTo(directions, 12);
        backFacets.CopyTo(directions, 16);
        forwardFacets.CopyTo(directions, 20);

        for (int i = 0; i < 24; i++)
        {
            shapeGenerator = new ShapeGenerator(height);

            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;
                
                Material mat = (Material)Resources.Load("Materials/Shape_Material", typeof(Material));

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = mat;
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();

            }
            faceGameObjects[i] = meshFilters[i].gameObject;
            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    public TerrainFace GetNextFace()
    {
        float minDeg = 360.0f;
        int index = 0;
        for(int i = 0; i < terrainFaces.Length; i++)
        {
            if(Vector3.Angle(transform.rotation * terrainFaces[i].GetEstimatedNormal(), Vector3.back ) < minDeg)
            {
                Debug.Log(Vector3.Angle(transform.rotation * terrainFaces[i].GetEstimatedNormal(), Vector3.back));
                minDeg = Vector3.Angle(transform.rotation * terrainFaces[i].GetEstimatedNormal(), Vector3.back);
                index = i;
            } 
        }
        currentFace = index;

        //currentFace ++;
        if(currentFace >= terrainFaces.Length)
        {
            currentFace = 0;
        }

        return terrainFaces[currentFace];
    }

    public TerrainFace GetCurrentFace()
    {
        return terrainFaces[currentFace];
    }

    public GameObject GetCurrentFaceGameObject()
    {
        return faceGameObjects[currentFace];
    }
}
