using UnityEngine;
using System.Collections.Generic;

public class GridScript : MonoBehaviour
{
    public GameObject[] treePrefabs;
    public GameObject[] grassPrefabs;
    public GameObject[] rockPrefabs;

    public Material terrainMaterial;
    public Material edgeMaterial;

    public float waterLevel = .3f;

    public float scale = .1f;
    public float treeNoiseScale = .12f;

    public float treeDensity = .45f;
    public float grassDensity = 0.6f;
    public float rockDensity = 0.60001f;

    public int size = 300;

    Cell[,] grid;



    void Start()
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));

        for(int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }


        float[,] falloffMap = new float[size, size];

        for(int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }


        grid = new Cell[size, size];

        for(int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y];
                noiseValue -= falloffMap[x, y];
                bool isWater = noiseValue < waterLevel;
                Cell cell = new Cell(isWater);
                grid[x, y] = cell;
            }
        }


        DrawTerrainMesh(grid);
        DrawEdgeMesh(grid);
        DrawTerrainTexture(grid);
        GenerateEnvironmentObjects(grid);

    }



    void DrawTerrainMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for(int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];

                if(!cell.isWater)
                {
                    Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                    Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                    Vector3 c = new Vector3(x - .5f, 0, y - .5f);
                    Vector3 d = new Vector3(x + .5f, 0, y - .5f);

                    Vector2 uvA = new Vector2(x / (float)size, y / (float)size);
                    Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                    Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                    Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                    Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };

                    for(int k = 0; k < 6; k++)
                    {
                        vertices.Add(v[k]);
                        triangles.Add(triangles.Count);
                        uvs.Add(uv[k]);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        MeshCollider meshCollider = transform.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

    }



    void DrawEdgeMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for(int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];

                if(!cell.isWater)
                {
                    if(x > 0)
                    {
                        Cell left = grid[x - 1, y];

                        if(left.isWater)
                        {
                            Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                            Vector3 b = new Vector3(x - .5f, 0, y - .5f);
                            Vector3 c = new Vector3(x - .5f, -1, y + .5f);
                            Vector3 d = new Vector3(x - .5f, -1, y - .5f);

                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };

                            for(int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }

                    if(x < size - 1)
                    {
                        Cell right = grid[x + 1, y];

                        if(right.isWater)
                        {
                            Vector3 a = new Vector3(x + .5f, 0, y - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, y - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, y + .5f);

                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };

                            for(int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }

                    if(y > 0)
                    {
                        Cell down = grid[x, y - 1];

                        if(down.isWater)
                        {
                            Vector3 a = new Vector3(x - .5f, 0, y - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, y - .5f);
                            Vector3 c = new Vector3(x - .5f, -1, y - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, y - .5f);

                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };

                            for(int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }

                    if(y < size - 1)
                    {
                        Cell up = grid[x, y + 1];

                        if(up.isWater)
                        {
                            Vector3 a = new Vector3(x + .5f, 0, y + .5f);
                            Vector3 b = new Vector3(x - .5f, 0, y + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, y + .5f);
                            Vector3 d = new Vector3(x - .5f, -1, y + .5f);

                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };

                            for(int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                }
            }
        }


        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        GameObject edgeObj = new GameObject("Edge");
        edgeObj.transform.SetParent(transform);

        MeshFilter meshFilter = edgeObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = edgeObj.AddComponent<MeshRenderer>();
        meshRenderer.material = edgeMaterial;
        //edgeMaterial.colorMap = new Color(138 / 255f, 196 / 255f, 86 / 255f, 50 / 100f);


    }



    void DrawTerrainTexture(Cell[,] grid)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] colorMap = new Color[size * size];

        for(int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];

                if(cell.isWater)
                    colorMap[y * size + x] = Color.blue;
                else
                    colorMap[y * size + x] = new Color(138 / 255f, 196 / 255f, 86 / 255f, 77 / 100f);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap);
        texture.Apply();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = terrainMaterial;
        meshRenderer.material.mainTexture = texture;

    }



    void GenerateEnvironmentObjects(Cell[,] grid)
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));

        for(int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset, y * treeNoiseScale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        for(int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];

                if(!cell.isWater)
                {
                    float t = Random.Range(0f, treeDensity);
                    float u = Random.Range(treeDensity + 0.001f, grassDensity);
                    float v = Random.Range(grassDensity + 0.001f, rockDensity);

                    if(noiseMap[x, y] < t)
                    {
                        GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                        GameObject tree = Instantiate(prefab, transform);

                        tree.transform.position = new Vector3(x, 0, y);
                        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        tree.transform.localScale = Vector3.one * Random.Range(0.6f, 0.75f);
                    }

                    else if(noiseMap[x, y] < u && noiseMap[x, y] > t)
                    {
                        int a = Random.Range(0, grassPrefabs.Length);

                        GameObject prefab = grassPrefabs[a];
                        GameObject grass = Instantiate(prefab, transform);

                        if (a == 0 || a == 1 || a == 2)
                        {
                            grass.transform.position = new Vector3(Random.Range(x - 0.4f, x + 0.4f), 0, Random.Range(y - 0.4f, y + 0.4f));
                        }

                        else
                        {
                            grass.transform.position = new Vector3(x, 0, y);
                        }

                        //grass.transform.position = new Vector3(Random.Range(x - 0.1f, x + 0.1f), 0, Random.Range(y - 0.1f, y + 0.1f));
                        grass.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        grass.transform.localScale = Vector3.one * Random.Range(0.4f, 0.7f);
                    }

                    else if(noiseMap[x, y] < v && noiseMap[x, y] > u && (Random.Range(0f, 1f) > 0.7f))
                    {
                        int b = Random.Range(0, rockPrefabs.Length + 3);

                        if (b == 0 || b == 1 || b == 2 || b == 3)
                        {
                            b = 0;
                        }

                        else
                            b = 1;

                        GameObject prefab = rockPrefabs[b];
                        GameObject rock = Instantiate(prefab, transform);

                        if (b == 0)
                        {
                            rock.transform.position = new Vector3(Random.Range(x - 0.2f, x + 0.2f), Random.Range(-0.3f, -0.15f), Random.Range(y - 0.2f, y + 0.2f));
                            rock.transform.localScale = Vector3.one * Random.Range(0.25f, 0.5f);
                        }

                        else
                        {
                            rock.transform.position = new Vector3(Random.Range(x - 0.2f, x + 0.2f), Random.Range(-0.15f, 0f), Random.Range(y - 0.2f, y + 0.2f));
                            rock.transform.localScale = Vector3.one * Random.Range(0.2f, 0.55f);
                        }

                        rock.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        
                    }

                }
            }
        }
    }

    /*void OnDrawGizmos() {
        if(!Application.isPlaying) return;
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                Cell cell = grid[x, y];
                if(cell.isWater)
                    Gizmos.color = Color.blue;
                else
                    Gizmos.color = Color.green;
                Vector3 pos = new Vector3(x, 0, y);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }*/
}