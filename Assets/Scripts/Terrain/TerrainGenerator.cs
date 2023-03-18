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
                    // generamos la textura del tile
                    tile.GetComponent<TileGenerator>().GenerateTile(centerVertexZ, maxDistanceZ);
                }
            }
        }
    }
}

