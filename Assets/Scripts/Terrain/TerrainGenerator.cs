using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private int mapWidthInTiles, mapDepthInTiles;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private float centerVertexZ, maxDistanceZ;
        [SerializeField] private TreeGenerator treeGenerator;

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
            Vector3[] tileMeshVertices = tilePrefab.GetComponent<MeshFilter>().sharedMesh.vertices; // IMPORTANTE, tiene que ser sharedMesh, pq mesh no es accesible y devuelve Null
            int tileDepthInVertices = (int) Mathf.Sqrt(tileMeshVertices.Length);
            int tileWidthInVertices = tileDepthInVertices;

            float distBetweenVertices = (float)tileDepth / (float)tileDepthInVertices;

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

            //genera arboles en el terreno
            treeGenerator.GenerateTrees(this.mapDepthInTiles * tileDepthInVertices, this.mapWidthInTiles * tileWidthInVertices, distBetweenVertices, terrainData);
        }
    }

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
        //Este metodo se utiliza para generar los árboles
        public TileCoord ConvertToTileCoord(int z, int x)
        {
            //Calculamos los indices del tile dividiendo el indice por el numero de tiles del eje
            int tileZ = (int)Mathf.Floor((float)z / (float)this.tileDepthInVertices);
            int tileX = (int)Mathf.Floor((float)x / (float)this.tileWidthInVertices);
            //el indice de la coordenada se calcula con el modulo de la division anterior
            //además, tenemos que mover el origen de coordenadas a la esquina inferior izqda
            int coordZ = this.tileDepthInVertices - (z % this.tileDepthInVertices) - 1;
            int coordX = this.tileWidthInVertices - (x % this.tileWidthInVertices) - 1;

            TileCoord tileCoord = new TileCoord(tileZ, tileX, coordZ, coordX);
            return tileCoord;
        }

    }
    //Clase para representar una coordenada en el sistema de los tiles
    public class TileCoord
    {
        public int tileZ, tileX, coordX, coordZ;
        public TileCoord(int tileZ, int tileX, int coordZ, int coordX)
        {
            this.tileZ = tileZ;
            this.tileX = tileX;
            this.coordX = coordX;
            this.coordZ = coordZ;
        }
    }
}

