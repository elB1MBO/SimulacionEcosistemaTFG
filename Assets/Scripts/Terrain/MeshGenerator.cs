using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20, zSize = 20;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        StartCoroutine(CreateShape());
    }

    void Update()
    {
        UpdateMesh();
    }

    IEnumerator CreateShape()
    {
        vertices = new Vector3[(xSize+1)*(zSize+1)];

        for(int i=0, z=0; z<=zSize; z++)
        {
            for(int x=0; x<=xSize; x++)
            {
                float y = Mathf.PerlinNoise(x*.3f, z*.3f);
                vertices[i] = new Vector3(x,y,z);
                i++;
            }
        }

        triangles = new int[xSize*zSize*6];
        for(int tris = 0, i=0, z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = i+0;
                triangles[tris + 1] = i+xSize+1;
                triangles[tris + 2] = i+1;
                triangles[tris + 3] = i+1;
                triangles[tris + 4] = i+xSize+1;
                triangles[tris + 5] = i+xSize+2;

                tris+=6;
                i++;

                yield return new WaitForSeconds(.005f);
            }
            i++;
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

}
