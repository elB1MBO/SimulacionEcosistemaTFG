using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{

    public class TerrainGenerator : MonoBehaviour
    {
        /// <summary>
        /// noiseMap: script que genera el height o noise map
        /// meshRenderer: necesario para ver el height map
        /// meshFilter: componente necesario para acceder a los vertices del mesh
        /// collider: componente collider del mesh, necesario para las colisiones
        /// scale: escala del height map
        /// terrainTypes: tipos de terreno que tenemos
        /// </summary>
        ///             
        [SerializeField] NoiseMap noiseMap;
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] MeshFilter meshFilter;
        [SerializeField] MeshCollider collider;
        [SerializeField] float scale;
        [SerializeField] TerrainType[] terrainTypes;

        void Start()
        {
            GenerateTerrain();
        }


        void GenerateTerrain()
        {
            //calculamos depth & width dependiendo en los vertices del mesh (nuestro Plane)
            Vector3[] meshVertices = this.meshFilter.mesh.vertices;

            int terrainDepth = (int)Mathf.Sqrt(meshVertices.Length);
            int terrainWidth = terrainDepth;

            //Inicializamos el heightMap usando el generador de NoiseMap
            float[,] heightMap = this.noiseMap.GenerateNoiseMap(terrainDepth, terrainWidth, scale);

            //Tras generar el heightMap, crearemos una textura 2D a la que asignaremos un material
            Texture2D terrainTexture = BuildTexture(heightMap);
            this.meshRenderer.material.mainTexture = terrainTexture;
        }

        /// <summary>
        /// crea un Color array, que usaremos para crear una textura en 2D para el terreno
        /// para cada coordenada del heightMap, tendrá un color distinto en la escala de grises
        /// dependiendo de su valor, gracias a la funcion Color.Lerp (más negro, más profundo)
        /// </summary>
        /// <param name="heightMap"></param>
        /// <returns>textura del terreno en escala de grises</returns>
        Texture2D BuildTexture(float[,] heightMap)
        {
            int depth = heightMap.GetLength(0);
            int width = heightMap.GetLength(1);

            Color[] colorMap = new Color[depth*width];

            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Transformamos el 2D map en un Array
                    int colorIndex = z * width + x;
                    float height = heightMap[z,x];
                    //Este primer mapa será una escala de grises
                    //colorMap[colorIndex] = Color.Lerp(Color.black, Color.white, height);

                    //Usando los tipos de terreno
                    TerrainType terrainType = GetTerrainType(height);
                    colorMap[colorIndex] = terrainType.color;
                }
            }

            //Creamos una nueva textura y establecemos los colores a los pixeles
            Texture2D terrainTexture = new Texture2D(width, depth);
            terrainTexture.wrapMode = TextureWrapMode.Clamp;
            terrainTexture.SetPixels(colorMap);
            terrainTexture.Apply();

            return terrainTexture;
        }
        TerrainType GetTerrainType(float height)
        {
            foreach (TerrainType type in terrainTypes)
            {
                if(height < type.height) return type;
            }
            //Devuelve el último tipo que debería ser el más alto
            return terrainTypes[terrainTypes.Length - 1];
        }
    }

    [System.Serializable]
    public class TerrainType
    {
        public string type;
        public float height;
        public Color color;
    }

}
