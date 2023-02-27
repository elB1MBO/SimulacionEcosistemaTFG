using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain{

public class TerrainGenerator : MonoBehaviour
{
    public static string name;
    public float worldSize;
    public float waterDepth = .3f;
    public float edgeDepth = .2f;
    
    public Biome grass;
    public Biome sand;
    public Biome water;

    [ShowInExplorer]
    int totalTiles;
    int landTiles;
    int waterTiles;
    float waterProb = .2f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    //Genera el terreno, asignando sus propiedades
    public TerrainProps Generate()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Biome GetBiomeInfo(float height, List<Biome> biomes)
    {

    }

    public void CreateMeshComponents()
    {

    }

    public void UpdateColors()
    {

    }

}

}
