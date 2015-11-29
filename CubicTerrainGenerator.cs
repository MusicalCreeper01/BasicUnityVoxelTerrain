using UnityEngine;
using System.Collections.Generic;


public class CubicTerrainGenerator : MonoBehaviour
{

    //A default unity 1x1x1 cube should be put in here
    public GameObject cube;
    //The material you want the cubes to have
    public Material material;

    //Noise settings. A higher frq will create larger scale details. Each seed value will create a unique look
    //P.S. Mountains seem to be broken, working on trying to get them fixed
    public int m_groundSeed = 0;
    public float m_groundFrq = 800.0f;
    public int m_mountainSeed = 1;
    public float m_mountainFrq = 1200.0f;

    //Chunk/Terrain settings
    public int m_tilesX = 2; //Number of chuncks on the x axis
    public int m_tilesZ = 2; //Number of chuncks on the z axis

    public int m_heightMapSize = 513; //Higher number will create more detailed height maps
    public int amplitude = 100; //Higher numbers with exaggerate the terrain

    //Private
    PerlinNoise m_groundNoise, m_mountainNoise;

    void Start()
    {
        m_groundNoise = new PerlinNoise(m_groundSeed);
        m_mountainNoise = new PerlinNoise(m_mountainSeed);

        float[,] htmap = new float[m_heightMapSize, m_heightMapSize];

        for (int tx = 0; tx < m_tilesX; tx++)
        {
            for (int tz = 0; tz < m_tilesZ; tz++)
            {
                FillHeights(htmap, tx, tz);
                GameObject tile = new GameObject();
                List<Mesh> meshs = new List<Mesh>();
                for (int x = 0; x < m_heightMapSize; x++)
                {
                    for (int z = 0; z < m_heightMapSize; z++)
                    {
                        meshs.Add(GenerateCubeMesh(x, htmap[z, x], z));
                        if (meshs.Count > 1000)
                        {
                            foreach (Mesh mesh in CombinedMeshes(meshs))
                            {
                                GameObject o = new GameObject();
                                o.AddComponent<MeshFilter>().mesh = mesh;
                                MeshRenderer renderer = o.AddComponent<MeshRenderer>();
                                renderer.material = material;
                                o.transform.parent = tile.transform;
                            }
                            meshs.Clear();
                        }

                    }
                }
                {
                    foreach (Mesh mesh in CombinedMeshes(meshs))
                    {
                        GameObject o = new GameObject();
                        o.AddComponent<MeshFilter>().mesh = mesh;
                        MeshRenderer renderer = o.AddComponent<MeshRenderer>();
                        renderer.material = material;
                        o.transform.parent = tile.transform;
                    }
                    meshs.Clear();

                }
                tile.transform.position = new Vector3(tx * m_heightMapSize, 0, tz * m_heightMapSize);
            }
        }
    }

    void FillHeights(float[,] htmap, int tileX, int tileZ)
    {

        for (int x = 0; x < m_heightMapSize; x++)
        {
            for (int z = 0; z < m_heightMapSize; z++)
            {
                float worldPosX = ((tileX * m_heightMapSize) + x) * 0.02f;
                float worldPosZ = ((tileZ * m_heightMapSize) + z) * 0.02f;

                float mountains = Mathf.Max(0.0f, m_mountainNoise.FractalNoise2D(worldPosX, worldPosZ, 6, m_mountainFrq, 0.8f));

                float plain = m_groundNoise.FractalNoise2D(worldPosX, worldPosZ, 4, m_groundFrq, 0.1f) + 0.1f;

                float height = plain + mountains;
                height = height * amplitude;

                Debug.Log(worldPosX + "x" + worldPosZ + ":" + height);
                htmap[z, x] = height;
            }
        }
    }

    Mesh GenerateCubeMesh(float x, float y, float z)
    {

        // You can change that line to provide another MeshFilter
        //MeshFilter filter = gameObject.AddComponent< MeshFilter >();
        Mesh mesh = new Mesh();// = filter.mesh;
        mesh.Clear();

        float length = 1f;
        float width = 1f;
        float height = 1f;

        #region Vertices
        Vector3 p0 = new Vector3(x + -length * .5f, y + -width * .5f, z + height * .5f);
        Vector3 p1 = new Vector3(x + length * .5f, y + -width * .5f, z + height * .5f);
        Vector3 p2 = new Vector3(x + length * .5f, y + -width * .5f, z + -height * .5f);
        Vector3 p3 = new Vector3(x + -length * .5f, y + -width * .5f, z + -height * .5f);

        Vector3 p4 = new Vector3(x + -length * .5f, y + width * .5f, z + height * .5f);
        Vector3 p5 = new Vector3(x + length * .5f, y + width * .5f, z + height * .5f);
        Vector3 p6 = new Vector3(x + length * .5f, y + width * .5f, z + -height * .5f);
        Vector3 p7 = new Vector3(x + -length * .5f, y + width * .5f, z + -height * .5f);

        Vector3[] vertices = new Vector3[]
    {
			// Bottom
			p0, p1, p2, p3,
			
			// Left
			p7, p4, p0, p3,
			
			// Front
			p4, p5, p1, p0,
			
			// Back
			p6, p7, p3, p2,
			
			// Right
			p5, p6, p2, p1,
			
			// Top
			p7, p6, p5, p4
    };
        #endregion

        #region Normales
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 front = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normales = new Vector3[]
    {
			// Bottom
			down, down, down, down,
			
			// Left
			left, left, left, left,
			
			// Front
			front, front, front, front,
			
			// Back
			back, back, back, back,
			
			// Right
			right, right, right, right,
			
			// Top
			up, up, up, up
    };
        #endregion

        #region UVs
        Vector2 _00 = new Vector2(0f, 0f);
        Vector2 _10 = new Vector2(1f, 0f);
        Vector2 _01 = new Vector2(0f, 1f);
        Vector2 _11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
    {
			// Bottom
			_11, _01, _00, _10,
			
			// Left
			_11, _01, _00, _10,
			
			// Front
			_11, _01, _00, _10,
			
			// Back
			_11, _01, _00, _10,
			
			// Right
			_11, _01, _00, _10,
			
			// Top
			_11, _01, _00, _10,
    };
        #endregion

        #region Triangles
        int[] triangles = new int[]
    {
			// Bottom
			3, 1, 0,
            3, 2, 1,			
			
			// Left
			3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
			
			// Front
			3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
			
			// Back
			3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
			
			// Right
			3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
			
			// Top
			3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
            3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

    };
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }

    public Mesh[] CombinedMeshes(List<Mesh> meshObjectList)
    {
        List<Mesh> meshs = new List<Mesh>();

        // combine meshes
        int verts = 0;
        List<CombineInstance> combine = new List<CombineInstance>();
        int i = 0;
        while (i < meshObjectList.Count)
        {
            verts = verts + meshObjectList[i].vertexCount;
            CombineInstance instance = new CombineInstance();
            instance.mesh = meshObjectList[i];
            instance.transform = transform.localToWorldMatrix;

            combine.Add(instance);


            //Just a backup incase a mesh adds a ton of vertices, maybe over 65000 so we need to check a bit lower
            if (verts > 60000)
            {
                Mesh combinedMesh = new Mesh();
                combinedMesh.CombineMeshes(combine.ToArray());
                meshs.Add(combinedMesh);
                combine = new List<CombineInstance>();
                verts = 0;
            }
            i++;
        }
        Mesh lastCombinedMesh = new Mesh();
        lastCombinedMesh.CombineMeshes(combine.ToArray());
        meshs.Add(lastCombinedMesh);

        return meshs.ToArray();
    }
}
