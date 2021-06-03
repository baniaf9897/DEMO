using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesselationManager : MonoBehaviour
{
	[Range(1.0f, 3.0f)]
	public float m_Radius = 1.0f;
	[Range(1, 200)]
	public int m_CountLong = 2;
	[Range(1, 200)]
	public int m_CountLat = 2;

    [Range(1, 5)]
    public int m_Granularity = 2;

    public Material m_Material;
	GameObject m_PlanetMesh;

	public List<Polygon> m_Polygons;
	public List<Vector3> m_Vertices;

    public Vector3[] m_origVertices;
    public int[] m_origIndices;
	// Start is called before the first frame update
	void Start()
    {
		//CreateSphere();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
		//MeshFilter filter = transform.GetComponent<MeshFilter>();
		//filter.mesh = CreateSphere(CreateSphereVertices());
		InitAsIcosohedron();
		Subdivide(m_Granularity);
		GenerateMesh();

	}

    public List<Vector3> CreateVertices()
    {
        // An icosahedron has 12 vertices, and
        // since they're completely symmetrical the
        // formula for calculating them is kind of
        // symmetrical too:

        List<Vector3> v = new List<Vector3>();

        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        v.Add(new Vector3(-1, t, 0).normalized);
        v.Add(new Vector3(1, t, 0).normalized);
        v.Add(new Vector3(-1, -t, 0).normalized);
        v.Add(new Vector3(1, -t, 0).normalized);
        v.Add(new Vector3(0, -1, t).normalized);
        v.Add(new Vector3(0, 1, t).normalized);
        v.Add(new Vector3(0, -1, -t).normalized);
        v.Add(new Vector3(0, 1, -t).normalized);
        v.Add(new Vector3(t, 0, -1).normalized);
        v.Add(new Vector3(t, 0, 1).normalized);
        v.Add(new Vector3(-t, 0, -1).normalized);
        v.Add(new Vector3(-t, 0, 1).normalized);

        return v;
    }

    public List<Polygon> CreatePolygones()
    {
        List < Polygon > p  = new List<Polygon>();


        p.Add(new Polygon(0, 11, 5));
        p.Add(new Polygon(0, 5, 1));
        p.Add(new Polygon(0, 1, 7));
        p.Add(new Polygon(0, 7, 10));
        p.Add(new Polygon(0, 10, 11));
        p.Add(new Polygon(1, 5, 9));
        p.Add(new Polygon(5, 11, 4));
        p.Add(new Polygon(11, 10, 2));
        p.Add(new Polygon(10, 7, 6));
        p.Add(new Polygon(7, 1, 8));
        p.Add(new Polygon(3, 9, 4));
        p.Add(new Polygon(3, 4, 2));
        p.Add(new Polygon(3, 2, 6));
        p.Add(new Polygon(3, 6, 8));
        p.Add(new Polygon(3, 8, 9));
        p.Add(new Polygon(4, 9, 5));
        p.Add(new Polygon(2, 4, 11));
        p.Add(new Polygon(6, 2, 10));
        p.Add(new Polygon(8, 6, 7));
        p.Add(new Polygon(9, 8, 1));

        return p;
    }

    public void InitAsIcosohedron()
    {
        m_Polygons = CreatePolygones();
        m_Vertices = CreateVertices();
        
    }

    public void Subdivide(int recursions)
    {
        var midPointCache = new Dictionary<int, int>();

        for (int i = 0; i < recursions; i++)
        {
            var newPolys = new List<Polygon>();
            foreach (var poly in m_Polygons)
            {
                int a = poly.m_Vertices[0];
                int b = poly.m_Vertices[1];
                int c = poly.m_Vertices[2];

                // Use GetMidPointIndex to either create a
                // new vertex between two old vertices, or
                // find the one that was already created.

                int ab = GetMidPointIndex(midPointCache, a, b);
                int bc = GetMidPointIndex(midPointCache, b, c);
                int ca = GetMidPointIndex(midPointCache, c, a);

                // Create the four new polygons using our original
                // three vertices, and the three new midpoints.
                newPolys.Add(new Polygon(a, ab, ca));
                newPolys.Add(new Polygon(b, bc, ab));
                newPolys.Add(new Polygon(c, ca, bc));
                newPolys.Add(new Polygon(ab, bc, ca));
            }
            // Replace all our old polygons with the new set of
            // subdivided ones.
            m_Polygons = newPolys;
        }
    }
    public int GetMidPointIndex(Dictionary<int, int> cache, int indexA, int indexB)
    {
        // We create a key out of the two original indices
        // by storing the smaller index in the upper two bytes
        // of an integer, and the larger index in the lower two
        // bytes. By sorting them according to whichever is smaller
        // we ensure that this function returns the same result
        // whether you call
        // GetMidPointIndex(cache, 5, 9)
        // or...
        // GetMidPointIndex(cache, 9, 5)

        int smallerIndex = Mathf.Min(indexA, indexB);
        int greaterIndex = Mathf.Max(indexA, indexB);
        int key = (smallerIndex << 16) + greaterIndex;

        // If a midpoint is already defined, just return it.

        int ret;
        if (cache.TryGetValue(key, out ret))
            return ret;

        // If we're here, it's because a midpoint for these two
        // vertices hasn't been created yet. Let's do that now!

        Vector3 p1 = m_Vertices[indexA];
        Vector3 p2 = m_Vertices[indexB];
        Vector3 middle = Vector3.Lerp(p1, p2, 0.5f).normalized;

        ret = m_Vertices.Count;
        m_Vertices.Add(middle);

        // Add our new midpoint to the cache so we don't have
        // to do this again. =)

        cache.Add(key, ret);
        return ret;
    }

    public void GenerateMesh()
    {
       /* if (m_PlanetMesh)
            Destroy(m_PlanetMesh);

        m_PlanetMesh = new GameObject("Planet Mesh");

        MeshRenderer surfaceRenderer = m_PlanetMesh.AddComponent<MeshRenderer>();
        surfaceRenderer.material = m_Material;
       */
        Mesh terrainMesh = new Mesh();

        int vertexCount = m_Polygons.Count * 3;

        int[] indices = new int[vertexCount];

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        Color32[] colors = new Color32[vertexCount];

        Color32 green = new Color32(20, 255, 30, 255);
        Color32 brown = new Color32(220, 150, 70, 255);

        for (int i = 0; i < m_Polygons.Count; i++)
        {
            var poly = m_Polygons[i];

            indices[i * 3 + 0] = i * 3 + 0;
            indices[i * 3 + 1] = i * 3 + 1;
            indices[i * 3 + 2] = i * 3 + 2;

            vertices[i * 3 + 0] = m_Vertices[poly.m_Vertices[0]];
            vertices[i * 3 + 1] = m_Vertices[poly.m_Vertices[1]];
            vertices[i * 3 + 2] = m_Vertices[poly.m_Vertices[2]];

            Color32 polyColor = Color32.Lerp(green, brown, Random.Range(0.0f, 1.0f));

            colors[i * 3 + 0] = polyColor;
            colors[i * 3 + 1] = polyColor;
            colors[i * 3 + 2] = polyColor;

            normals[i * 3 + 0] = m_Vertices[poly.m_Vertices[0]];
            normals[i * 3 + 1] = m_Vertices[poly.m_Vertices[1]];
            normals[i * 3 + 2] = m_Vertices[poly.m_Vertices[2]];
        }

        terrainMesh.vertices = vertices;
        m_origVertices = vertices;
        terrainMesh.normals = normals;
        terrainMesh.colors32 = colors;

        m_origIndices = indices;

        terrainMesh.SetTriangles(indices, 0);

        MeshFilter terrainFilter = transform.GetComponent<MeshFilter>();
        terrainFilter.mesh = terrainMesh;
    }
    /*public Vector3[] CreateSphereVertices() {
		float radius = m_Radius;
 		int nbLong = m_CountLong;
 		int nbLat = m_CountLat;
		 
		Vector3[] vertices = new Vector3[(nbLong + 1) * nbLat + 2];
		float _pi = Mathf.PI;
		float _2pi = _pi * 2f;

		vertices[0] = Vector3.up * radius;
		for (int lat = 0; lat < nbLat; lat++)
		{
			float a1 = _pi * (float)(lat + 1) / (nbLat + 1);
			float sin1 = Mathf.Sin(a1);
			float cos1 = Mathf.Cos(a1);

			for (int lon = 0; lon <= nbLong; lon++)
			{
				float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
				float sin2 = Mathf.Sin(a2);
				float cos2 = Mathf.Cos(a2);

				vertices[lon + lat * (nbLong + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
			}
		}
		vertices[vertices.Length - 1] = Vector3.up * -radius;

		return vertices;
	}
	public Mesh CreateSphere(Vector3[] _vertices)
    {
		//MeshFilter filter = transform.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();// filter.mesh;
		mesh.Clear();

		float radius = m_Radius;
		// Longitude |||
		int nbLong = m_CountLong;
		// Latitude ---
		int nbLat = m_CountLat;

		Vector3[] vertices = _vertices;//CreateSphereVertices();

		#region Normales		
		Vector3[] normales = new Vector3[vertices.Length];
		for (int n = 0; n < vertices.Length; n++)
			normales[n] = vertices[n].normalized;
		#endregion

		#region UVs
		Vector2[] uvs = new Vector2[vertices.Length];
		uvs[0] = Vector2.up;
		uvs[uvs.Length - 1] = Vector2.zero;
		for (int lat = 0; lat < nbLat; lat++)
			for (int lon = 0; lon <= nbLong; lon++)
				uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
		#endregion

		#region Triangles
		int nbFaces = vertices.Length;
		int nbTriangles = nbFaces * 2;
		int nbIndexes = nbTriangles * 3;
		int[] triangles = new int[nbIndexes];

		//Top Cap
		int i = 0;
		for (int lon = 0; lon < nbLong; lon++)
		{
			triangles[i++] = lon + 2;
			triangles[i++] = lon + 1;
			triangles[i++] = 0;
		}

		//Middle
		for (int lat = 0; lat < nbLat - 1; lat++)
		{
			for (int lon = 0; lon < nbLong; lon++)
			{
				int current = lon + lat * (nbLong + 1) + 1;
				int next = current + nbLong + 1;

				triangles[i++] = current;
				triangles[i++] = current + 1;
				triangles[i++] = next + 1;

				triangles[i++] = current;
				triangles[i++] = next + 1;
				triangles[i++] = next;
			}
		}

		//Bottom Cap
		for (int lon = 0; lon < nbLong; lon++)
		{
			triangles[i++] = vertices.Length - 1;
			triangles[i++] = vertices.Length - (lon + 2) - 1;
			triangles[i++] = vertices.Length - (lon + 1) - 1;
		}
		#endregion

		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		return mesh;
	}*/
}
