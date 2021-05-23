using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformationManager : MonoBehaviour
{
    public RandomManager randomManager = new RandomManager();

    public GameObject refObject = null;

    [SerializeField]
    [Range(0.5f, 50.0f)]
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
        UpdateTexture();
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

                color = m_Colour.Evaluate(GetSphereHeight(p));
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
