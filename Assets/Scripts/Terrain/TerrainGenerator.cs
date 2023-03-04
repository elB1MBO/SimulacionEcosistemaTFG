using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    [ExecuteInEditMode]
    public class TerrainGenerator : MonoBehaviour
    {
        public static string name = "Terrain Mesh";

        public float worldSize;
        public float waterDepth = .3f;
        public float edgeDepth = .2f;

        public Biome land;
        public Biome water;

        [Header("Info")]
        int totalTiles;
        int landTiles;
        int waterTiles;
        float waterProb = .2f;

        Mesh mesh;
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;

        bool needsUpdate = true;

        // Update is called once per frame
        void Update()
        {
            if (needsUpdate)
            {
                needsUpdate = false;
                Generate();
            }
            else
            {
                UpdateColors();
            }
        }


        //Genera el terreno, asignando sus propiedades
        public void Generate()
        {
            var biomes = new Biome[] { land, water };
        }

        /*public Biome GetBiomeByHeight(float height, List<Biome> biomes)
        {
            foreach (Biome biome in biomes)
            {
                if (biome.height == height)
                {
                    return biome;
                }
            }
        }*/

        public void CreateMeshComponents()
        {

        }

        public void UpdateColors()
        {

        }

    }

}
