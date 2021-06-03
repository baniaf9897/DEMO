using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformationManager : MonoBehaviour
{
    public RandomManager randomManager1 = new RandomManager();
    public RandomManager randomManager2 = new RandomManager();
    public RandomManager randomManager3 = new RandomManager();

    public GameObject refObject = null;

    [SerializeField]
    [Range(0.1f, 50.0f)]
    private float m_Height = 1;

    public Gradient m_Colour = new Gradient();
    public AnimationCurve m_heightCurve;


    public Texture2D m_temperatureTex = null;
    public int m_textureSize = 512;

    Bounds m_meshBounds;
    Vector3[] refVertices = new Vector3[0];

    // Start is called before the first frame update
    void Start()
    {

      

        Debug.Log("set original vertices");
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        refVertices = mesh.vertices;

        randomManager1.m_frequency = 0.1f;
        randomManager1.m_gain = 0.1f;
        randomManager1.m_lacunarity = 0.1f;
        randomManager1.m_NumOctaves = 2;
        randomManager1.m_offsetX = 2;
        randomManager1.m_offsetY = 4;
        randomManager1.m_offsetZ = 1;

        randomManager2.m_frequency = 0.1f;
        randomManager2.m_gain = 0.1f;
        randomManager2.m_lacunarity = 0.1f;
        randomManager2.m_NumOctaves = 2;
        randomManager2.m_offsetX = 2;
        randomManager2.m_offsetY = 4;
        randomManager2.m_offsetZ = 1;


        randomManager3.m_frequency = 0.1f;
        randomManager3.m_gain = 0.1f;
        randomManager3.m_lacunarity = 0.1f;
        randomManager3.m_NumOctaves = 2;
        randomManager3.m_offsetX = 2;
        randomManager3.m_offsetY = 4;
        randomManager3.m_offsetZ = 1;


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
        //UpdateTexture();
    }

   /* public float GetTerrainHeight(float x, float z)
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
    }*/

    public float Get3DHeight(float x, float y, float z, RandomManager randomManager)
    {
        return randomManager.FBM3D(x, y, z);
    }


    public float GetSphereHeight(Vector3 worldpos, RandomManager randomManager)
    {
        float height = m_Height * (m_heightCurve.Evaluate(Get3DHeight(worldpos.x, worldpos.y, worldpos.z,randomManager)));
        return height;
    }

    public void UpdateSphere()
    {

        Vector3[] vertices = (Vector3[])transform.GetComponent<TesselationManager>().m_origVertices.Clone();

        for(int i = 0; i < vertices.Length/3.0f; i++)
        {
            vertices[i] *= GetSphereHeight(vertices[i],randomManager1);
        }
        for (int i = vertices.Length / 3; i < 2 * vertices.Length / 3.0f; i++)
        {
            vertices[i] *= GetSphereHeight(vertices[i], randomManager2);
        }
        for (int i = 2 * vertices.Length / 3; i < vertices.Length; i++)
        {
            vertices[i] *= GetSphereHeight(vertices[i], randomManager3);
        }
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh.vertices  = vertices;
        meshFilter.sharedMesh.RecalculateBounds();
        meshFilter.sharedMesh.RecalculateNormals();
        meshFilter.sharedMesh.RecalculateTangents();
        meshFilter.sharedMesh.Optimize();

        //meshFilter.sharedMesh.SetTriangles((int[])transform.GetComponent<TesselationManager>().m_origIndices.Clone(), 0);

    }

    public void UpdateTexture()
    {
        if (m_temperatureTex == null)
        {
            m_temperatureTex = new Texture2D(m_textureSize, m_textureSize);
            m_temperatureTex.name = "TemperatureMap";
            m_temperatureTex.wrapMode = TextureWrapMode.Repeat;
        }

        if (m_temperatureTex.width != m_textureSize)
            m_temperatureTex.Resize(m_textureSize, m_textureSize);

        Color color = new Color();

        Vector3 p = new Vector3(0, 0, 0);
        float u, v;
        float invh = 1.0f / (float)m_temperatureTex.height;
        float invw = 1.0f / (float)m_temperatureTex.width;

        for (int z = 0; z < m_temperatureTex.height; z++)
        {
            v = z * invh;

            for (int x = 0; x < m_temperatureTex.width; x++)
            {
                u = x * invw;
                TransformUVtoWorld(u, v, ref p);

                color = m_Colour.Evaluate(GetSphereHeight(p,randomManager1));
                m_temperatureTex.SetPixel(x, z, color);
            }
        }

        m_temperatureTex.Apply();

        Renderer renderer = GetComponent<Renderer>();
        renderer.sharedMaterial.mainTexture = m_temperatureTex;
    }

    public void TransformWorldToUV(Vector3 p, out float u, out float v)
    {
        // Transform from world space to texture space...
        p = transform.InverseTransformPoint(p);
        u = 0.5f + (p.x / m_meshBounds.size.x);
        v = 0.5f + (p.z / m_meshBounds.size.z);
    }

    public void TransformUVtoWorld(float u, float v, ref Vector3 p)
    {
        p = transform.TransformPoint((u - 0.5f) * m_meshBounds.size.x, 0, (v - 0.5f) * m_meshBounds.size.z);
    }
}
