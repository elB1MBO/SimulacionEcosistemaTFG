using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public Material terrainMaterial, edgeMaterial;
    public GameObject[] treePrefabs, plantsPrefabs;

    public int size = 150;
    public float waterLevel = .35f, scale = .1f;
    public float treeNoiseScale = .005f;
    public float treeDensity = .01f;
    public float plantsNoiseScale = .05f;
    public float plantsDensity = .35f;

    Cell[,] map;
    // Start is called before the first frame update
    void Start()
    {
        float[,] noiseMap = new float[size,size];
        (float xOffset, float zOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for(int z = 0; z < size; z++) {
            for(int x = 0; x < size; x++) {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, z * scale + zOffset);
                noiseMap[x, z] = noiseValue;
            }
        }

        float[,] falloffMap = new float[size, size];
        for(int z = 0; z < size; z++) {
            for(int x = 0; x < size; x++) {
                float xv = x / (float)size * 2 - 1;
                float zv = z / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(zv));
                falloffMap[x, z] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }

        map = new Cell[size,size];
        for(int z=0; z<size; z++){
            for(int x=0; x<size; x++){
                float noiseValue = noiseMap[x,z];
                noiseValue -= falloffMap[x,z];
                bool isWater = noiseValue < waterLevel;
                Cell cell = new Cell(isWater);
                map[x,z] = cell;
            }
        }

        DrawTerrainMesh(map);
        DrawEdgeMesh(map);
        DrawTexture(map);
        GenerateTrees(map);
        GeneratePlants(map);
    }

    void DrawTerrainMesh(Cell[,] map) {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for(int z = 0; z < size; z++) {
            for(int x = 0; x < size; x++) {
                Cell cell = map[x, z];
                if(!cell.isWater) {
                    Vector3 a = new Vector3(x - .5f, 0, z + .5f);
                    Vector3 b = new Vector3(x + .5f, 0, z + .5f);
                    Vector3 c = new Vector3(x - .5f, 0, z - .5f);
                    Vector3 d = new Vector3(x + .5f, 0, z - .5f);
                    Vector2 uvA = new Vector2(x / (float)size, z / (float)size);
                    Vector2 uvB = new Vector2((x + 1) / (float)size, z / (float)size);
                    Vector2 uvC = new Vector2(x / (float)size, (z + 1) / (float)size);
                    Vector2 uvD = new Vector2((x + 1) / (float)size, (z + 1) / (float)size);
                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                    Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                    for(int i = 0; i < 6; i++) {
                        vertices.Add(v[i]);
                        triangles.Add(triangles.Count);
                        uvs.Add(uv[i]);
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
    }

    void DrawEdgeMesh(Cell[,] map) {
        Mesh edgeMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for(int z = 0; z < size; z++) {
            for(int x = 0; x < size; x++) {
                Cell cell = map[x, z];
                if(!cell.isWater) {
                    //Hay que comprobar las celdas colindantes
                    if(x > 0) {
                        Cell left = map[x - 1, z];
                        if(left.isWater) {
                            Vector3 a = new Vector3(x - .5f, 0, z + .5f);
                            Vector3 b = new Vector3(x - .5f, 0, z - .5f);
                            Vector3 c = new Vector3(x - .5f, -1, z + .5f);
                            Vector3 d = new Vector3(x - .5f, -1, z - .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for(int k = 0; k < 6; k++) {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if(x < size - 1) {
                        Cell right = map[x + 1, z];
                        if(right.isWater) {
                            Vector3 a = new Vector3(x + .5f, 0, z - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, z + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, z - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, z + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for(int k = 0; k < 6; k++) {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if(z > 0) {
                        Cell down = map[x, z - 1];
                        if(down.isWater) {
                            Vector3 a = new Vector3(x - .5f, 0, z - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, z - .5f);
                            Vector3 c = new Vector3(x - .5f, -1, z - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, z - .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for(int k = 0; k < 6; k++) {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if(z < size - 1) {
                        Cell up = map[x, z + 1];
                        if(up.isWater) {
                            Vector3 a = new Vector3(x + .5f, 0, z + .5f);
                            Vector3 b = new Vector3(x - .5f, 0, z + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, z + .5f);
                            Vector3 d = new Vector3(x - .5f, -1, z + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for(int k = 0; k < 6; k++) {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                }
            }
        }
        edgeMesh.vertices = vertices.ToArray();
        edgeMesh.triangles = triangles.ToArray();
        edgeMesh.RecalculateNormals();

        GameObject edgeObj = new GameObject("Edge");
        edgeObj.transform.SetParent(transform);

        MeshFilter meshFilter = edgeObj.AddComponent<MeshFilter>();
        meshFilter.mesh = edgeMesh;

        MeshRenderer meshRenderer = edgeObj.AddComponent<MeshRenderer>();
        meshRenderer.material = edgeMaterial;
    }

    void DrawTexture(Cell[,] map) {
        Texture2D texture = new Texture2D(size, size);
        Color[] colorMap = new Color[size * size];
        for(int z = 0; z < size; z++) {
            for(int x = 0; x < size; x++) {
                Cell cell = map[x, z];
                if(cell.isWater)
                    colorMap[z * size + x] = Color.blue;
                else
                    colorMap[z * size + x] = Color.green;
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap);
        texture.Apply();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = terrainMaterial;
        meshRenderer.material.mainTexture = texture;
    }

    void GenerateTrees(Cell[,] map)
    {
        float[,] noiseMap = new float[size,size];
        (float xOffset, float zOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for(int z = 0; z < size; z++) {
            for(int x = 0; x < size; x++) {
                float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset, z * treeNoiseScale + zOffset);
                noiseMap[x, z] = noiseValue;
            }
        }

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                Cell cell = map[x, y];
                if(!cell.isWater) {
                    float v = Random.Range(0f, treeDensity);
                    if(noiseMap[x, y] < v) {
                        //Es un arbol
                        GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                        GameObject tree = Instantiate(prefab, transform);
                        tree.transform.position = new Vector3(x, 0, y);
                        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        tree.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                    }
                }
            }
        }
    }

    void GeneratePlants(Cell[,] map)
    {
        float[,] noiseMap = new float[size,size];
        (float xOffset, float zOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for(int z = 0; z < size; z++) {
            for(int x = 0; x < size; x++) {
                float noiseValue = Mathf.PerlinNoise(x * plantsNoiseScale + xOffset, z * plantsNoiseScale + zOffset);
                noiseMap[x, z] = noiseValue;
            }
        }

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                Cell cell = map[x, y];
                if(!cell.isWater) {
                    float v = Random.Range(0f, plantsDensity);
                    if(noiseMap[x, y] < v) {
                        //Es un arbol
                        GameObject prefab = plantsPrefabs[Random.Range(0, plantsPrefabs.Length)];
                        GameObject plant = Instantiate(prefab, transform);
                        plant.transform.position = new Vector3(x, 0, y);
                        plant.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        plant.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                    }
                }
            }
        }
    }

    /* void OnDrawGizmos() {
        if(!Application.isPlaying) return;
        for(int z = 0; z < size; z++) {
            for(int x = 0; x < size; x++) {
                Cell cell = map[x, z];
                if(cell.isWater)
                    Gizmos.color = Color.blue;
                else
                    Gizmos.color = Color.green;
                Vector3 pos = new Vector3(x, 0, z);
                Gizmos.DrawCube(pos, new Vector3(1,.001f, 1));
            }
        }
    } */

}


/* for(int z=0; z<size; z++){
    for(int x=0; x<size; x++){
                
    }
} */