using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class TerrainData
    {
        private int tileDepthInVertices, tileWidthInVertices;
        public TileData[,] tilesData;
        public TerrainData(int tileDepthInVertices, int tileWidthInVertices, int mapDepthInTiles, int mapWidthInTiles)
        {
            // Matriz de tiles basada en el ancho y largo del mapa
            tilesData = new TileData[tileDepthInVertices * mapDepthInTiles, tileWidthInVertices * mapWidthInTiles];
            this.tileDepthInVertices = tileDepthInVertices;
            this.tileWidthInVertices = tileWidthInVertices;
        }
        
        //Método que guarda el tile en su coordenada correspondiente
        public void AddTileData(TileData tileData, int tileZ, int tileX)
        {
            tilesData[tileZ, tileX] = tileData;
        }

    }
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private int mapWidthInTiles, mapDepthInTiles;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private float centerVertexZ, maxDistanceZ;

        private void Start()
        {
            GenerateMap();
        }

        void GenerateMap()
        {
            //obtenemos las dimensiones de cada tile del prefab
            Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
            int tileWidth = (int) tileSize.x;
            int tileDepth = (int) tileSize.z;

            //Calcula el número de vertices que tendrá el tile en cada eje usando su mesh
            Vector3[] tileMeshVertices = tilePrefab.GetComponent<MeshFilter>().mesh.vertices;
            int tileDepthInVertices = (int) Mathf.Sqrt(tileMeshVertices.Length);
            int tileWidthInVertices = tileDepthInVertices;

            //Construyo un objeto vacio de la clase TerrainData para llenarlo con los tiles que se generen
            TerrainData terrainData = new TerrainData(tileDepthInVertices, tileWidthInVertices, this.mapDepthInTiles, this.mapWidthInTiles);

            //para cada tile, instancia un tile en la posicion correcta
            for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++)
            {
                for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
                {
                    // calcula la posicion del tile basandose en los indices x,z
                    Vector3 tilePosition = new Vector3(this.gameObject.transform.position.x + xTileIndex * tileWidth,
                        this.gameObject.transform.position.y,
                        this.gameObject.transform.position.z + zTileIndex * tileDepth);

                    // instanciamos un nuevo tile
                    GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                    // generamos la textura del tile, y la guarda en el objeto TerrainData
                    TileData tileData = tile.GetComponent<TileGenerator>().GenerateTile(centerVertexZ, maxDistanceZ);
                    terrainData.AddTileData(tileData, zTileIndex, xTileIndex);
                }
            }
        }
    }
}

