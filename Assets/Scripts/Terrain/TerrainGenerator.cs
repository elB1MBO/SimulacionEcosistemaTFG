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
        [SerializeField] private float heightMultiplier;

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

            //Cambiamos el valor y del mesh
            UpdateMeshVertices(heightMap);
        }

        /// <summary>
        /// metodo responsable de cambiar los vertices del Mesh dependiendo de los valores del heightMap
        /// </summary>
        /// <param name="heightMap"></param>
        private void UpdateMeshVertices(float[,] heightMap)
        {
            int depth = heightMap.GetLength(0);
            int width = heightMap.GetLength(1);

            Vector3[] meshVertices = this.meshFilter.mesh.vertices;

            //iterar sobre todas las coordenadas del mapa, actualizando los indices de los vectores
            int vertexIndex = 0;
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    float height = heightMap[z,x];
                    Vector3 vertex = meshVertices[vertexIndex];
                    //Cambiamos la coordenada Y , dependiendo de la altura, evaluada por la funcion EvaluateHeight que he creado
                    meshVertices[vertexIndex] = new Vector3(vertex.x, EvaluateHeight(height*heightMultiplier), vertex.z);
                    vertexIndex++;
                }
            }

            //Actualizamos los vertices del mesh y sus propiedades
            this.meshFilter.mesh.vertices = meshVertices;
            //IMPORTANTE: Hay que llamar a los metodos RecalculateBounds y RecalculateNormals cada vez que cambiemos los vertices de un mesh
            this.meshFilter.mesh.RecalculateBounds();
            this.meshFilter.mesh.RecalculateNormals();
            //Actualizar el mesh collider
            this.collider.sharedMesh = this.meshFilter.mesh;
        }

        private float EvaluateHeight(float height)
        {
            if (height < 0.4) return 0.4f;
            else if (height < 0.5) return 0.5f;
            else return height;
        }

        /// <summary>
        /// crea un Color array, que usaremos para crear una textura en 2D para el terreno
        /// para cada coordenada del heightMap, tendr� un color distinto en la escala de grises
        /// dependiendo de su valor, gracias a la funcion Color.Lerp (m�s negro, m�s profundo)
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
                    //Este primer mapa ser� una escala de grises
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
            //Devuelve el �ltimo tipo que deber�a ser el m�s alto
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
