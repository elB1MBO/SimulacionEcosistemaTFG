using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadtreeNode
{
    private const int MaxObjectsPerNode = 10;
    [SerializeField] private readonly List<GameObject> objects; // Lista de objetos en esta region
    private readonly Bounds bounds; //Los limites de la region que representa este nodo
    private QuadtreeNode[] children; // Subregiones del nodo
    private Dictionary<string, List<GameObject>> objectsByTag;

    public QuadtreeNode(Bounds bounds)
    {
        this.bounds = bounds;
        objects = new List<GameObject>();
        children = null;
        objectsByTag = new Dictionary<string, List<GameObject>>();
    }

    public void Insert(GameObject obj, string tag)
    {
        if (!objectsByTag.ContainsKey(tag))
        {
            objectsByTag[tag] = new List<GameObject>();
        }
        objectsByTag[tag].Add(obj);

        if(children != null)
        {
            // Si este nodo tiene subregiones, insertar el onjeto en la subregion correspondiente
            for (int i = 0; i < 4; i++) // es un quadtree
            {
                if (children[i].bounds.Contains(obj.transform.position))
                {
                    children[i].Insert(obj, tag);
                    return;
                }
            }
        }

        objects.Add(obj);

        // Si el num de objetos en este nodo supera el limite, lo subdivide
        if (objects.Count > MaxObjectsPerNode)
        {
            Subdivide(tag);
        }

    }

    private void Subdivide(string tag)
    {
        float halfSizeX = bounds.size.x * 0.5f;
        float halfSizeZ = bounds.size.z * 0.5f;

        children = new QuadtreeNode[4];
        children[0] = new QuadtreeNode(new Bounds(bounds.center + new Vector3(-halfSizeX, 0, -halfSizeZ), bounds.size * 0.5f));
        children[1] = new QuadtreeNode(new Bounds(bounds.center + new Vector3(halfSizeX, 0, -halfSizeZ), bounds.size * 0.5f));
        children[2] = new QuadtreeNode(new Bounds(bounds.center + new Vector3(-halfSizeX, 0, halfSizeZ), bounds.size * 0.5f));
        children[3] = new QuadtreeNode(new Bounds(bounds.center + new Vector3(halfSizeX, 0, halfSizeZ), bounds.size * 0.5f));

        // Mover objetos del nodo actual a las subregiones correspondientes
        for(int i = objects.Count -1; i >= 0; i--)
        {
            for (int j = 0; j < 4; j++)
            {
                if (children[j].bounds.Contains(objects[i].transform.position))
                {
                    children[j].Insert(objects[i], tag);
                    objects.RemoveAt(i); 
                    break;
                }
            }
        }
    }

    public List<GameObject> GetObjectsInRegion(Bounds region, string tag)
    {
        List<GameObject> objectsResult = new List<GameObject>();

        if (children != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (children[i].bounds.Intersects(region))
                {
                    objectsResult.AddRange(children[i].GetObjectsInRegion(region, tag));
                }
            }
        }

        // Agregar objetos del nodo actual que están dentro de la región
        if (objectsByTag.ContainsKey(tag))
        {
            foreach (GameObject obj in objectsByTag[tag])
            {
                if (region.Contains(obj.transform.position))
                {
                    objectsResult.Add(obj);
                }
            }
        }
    
        return objectsResult;
    }

}
