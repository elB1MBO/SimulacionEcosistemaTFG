using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class TreeGenerator : MonoBehaviour
    {
        [SerializeField] NoiseMap noiseMapGenerator;
        [SerializeField] private Wave[] waves;
        [SerializeField] private float terrainScale;
        [SerializeField] private float neighborDist; //definirá la concentración de árboles
        [SerializeField] private GameObject treePrefab;

        public void GenerateTrees(int terrainDepth, int terrainWidth, float verticesDist, TerrainData terrainData)
        {
            // generamos un noise map usando el Perlin Noise
            float[,] noiseMap = this.noiseMapGenerator.GeneratePerlinNoiseMap(terrainDepth, terrainWidth, this.terrainScale, 0, 0, this.waves);

            // float zSize = terrainDepth * this.terrainScale;
            // float xSize = terrainWidth * this.terrainScale;

            for (int z = 0; z < terrainDepth; z++)
            {
                for (int x = 0; x < terrainWidth; x++)
                {
                    // convertimos del sistema de coordenadas del Terreno al del Tile, y devolvemos el TileData correspondiente
                    TileCoord tileCoord = terrainData.ConvertToTileCoord(z, x);
                    TileData tileData = terrainData.tilesData[tileCoord.tileZ, tileCoord.tileX];
                    int tileWidth = tileData.heightMap.GetLength(1);

                    // calcula el indice del vertice del mesh
                    Vector3[] meshVertices = tileData.mesh.vertices;
                    int tileIndex = tileCoord.coordZ * tileWidth + tileCoord.coordX;

                    // obtenemos el tipo de terreno por la coordenada
                    TerrainType terrainType = tileData.chosenHeightTerrainTypes[tileCoord.coordZ, tileCoord.tileX];
                    // si el tipo de terreno es agua, no podrá haber un árbol
                    if (terrainType.name != "water")
                    {
                        float treeProb = noiseMap[z, x];

                        // compara el valor actual del ruido con el de los vecinos
                        // 0 -> begin, 1-> end
                        int[] neighborZValues = new int[2];
                        int[] neighborXValues = new int[2];

                        neighborZValues[0] = (int)Mathf.Max(0, z - this.neighborDist);
                        neighborZValues[1] = (int)Mathf.Min(terrainDepth - 1, z + this.neighborDist);
                        neighborXValues[0] = (int)Mathf.Max(0, x - this.neighborDist);
                        neighborXValues[1] = (int)Mathf.Min(terrainWidth - 1, x + this.neighborDist);

                        float max = 0;
                        for (int neighborZ = neighborZValues[0]; neighborZ <= neighborZValues[1]; neighborZ++)
                        {
                            for (int neighborX = neighborXValues[0]; neighborX <= neighborXValues[1]; neighborX++)
                            {
                                float neighborValue = noiseMap[neighborZ, neighborX];
                                if (neighborValue >= max) max = neighborValue;
                            }
                        }

                        // si el valor actual del ruido es el mayor, colocamos un árbol en esta posicion
                        if (treeProb == max)
                        {
                            Vector3 treePos = new Vector3(x*verticesDist, meshVertices[tileIndex].y, z*verticesDist);
                            GameObject tree = Instantiate(this.treePrefab, treePos, Quaternion.identity) as GameObject;
                            tree.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                        }
                    }
                }
            }

        }
    }

}