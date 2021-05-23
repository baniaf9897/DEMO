using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformationManager : MonoBehaviour
{
    public RandomManager randomManager = new RandomManager();

    public GameObject refObject = null;

    [SerializeField]
    [Range(0.5f, 3.0f)]
    private float m_Height = 1;

    public AnimationCurve m_heightCurve;


    Bounds m_meshBounds;
    Vector3[] refVertices = new Vector3[0];

    // Start is called before the first frame update
    void Start()
    {

      

        Debug.Log("set original vertices");
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        refVertices = mesh.vertices;

        randomManager.m_frequency = 0.1f;
        randomManager.m_gain = 0.1f;
        randomManager.m_lacunarity = 0.1f;
        randomManager.m_NumOctaves = 2;
        randomManager.m_offsetX = 2;
        randomManager.m_offsetY = 4;
        randomManager.m_offsetZ = 1;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        m_meshBounds = GetComponent<MeshFilter>().sharedMesh.bounds;

        UpdateSphere();
        //UpdatePlane();

    }

    public float GetTerrainHeight(float x, float z)
    {
        return m_Height * m_heightCurve.Evaluate(GetHeight(x, z));
    }

    public float GetTerrainHeight(Vector3 worldpos)
    {
        return m_Height * m_heightCurve.Evaluate(GetHeight(worldpos.x, worldpos.z));
    }

    public float GetHeight(Vector3 worldpos)
    {
        return randomManager.FBM(worldpos.x, worldpos.z);
    }

    public float GetHeight(float x, float z)
    {
        return randomManager.FBM(x, z);
    }

    public float Get3DHeight(float x, float y, float z)
    {
        return randomManager.FBM3D(x, y, z);
    }


    public float GetSphereHeight(Vector3 worldpos)
    {
        return m_Height * (m_heightCurve.Evaluate(Get3DHeight(worldpos.x, worldpos.y, worldpos.z)));
    }

    public void UpdateSphere()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;

        Vector3[] refVertices = transform.GetComponent<TesselationManager>().CreateSphereVertices();

        for (var i = 0; i < vertices.Length; i++) {

            Vector3 reference = new Vector3();
            reference.Set(refVertices[i].x, refVertices[i].y, refVertices[i].z);
            Vector3 displacement = reference.normalized * GetSphereHeight(transform.TransformPoint(refVertices[i]));


            vertices[i].x = reference.x + displacement.x;
            vertices[i].y = reference.y + displacement.y;
            vertices[i].z = reference.z + displacement.z;
        }

        Debug.Log(refVertices[0]);

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

    }

    /*public void UpdatePlane()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;

        for (var i = 0; i < vertices.Length; i++)
            vertices[i].y = GetTerrainHeight(transform.TransformPoint(vertices[i]));

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

    }*/
}
