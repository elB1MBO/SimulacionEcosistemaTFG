using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{

    public class TileGenerator : MonoBehaviour
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
        [SerializeField] AnimationCurve heigthCurve;
        [SerializeField] Wave[] waves;
        // mapa de calor
        [SerializeField] TerrainType[] heatTerrainTypes;
        [SerializeField] private VisualizationMode visualizationMode;
        // mapa de humedad
        [SerializeField] TerrainType[] moistureTerrainTypes;
        [SerializeField] AnimationCurve moistureCurve;
        [SerializeField] Wave[] moistureWaves;
        //Bioma
        [SerializeField] BiomeRow[] biomes;
        [SerializeField] Color waterColor;

        public void GenerateTile(float centerVertexZ, float maxDistanceZ)
        {
            //calculamos depth & width dependiendo en los vertices del mesh (nuestro Plane)
            Vector3[] meshVertices = this.meshFilter.mesh.vertices;

            int terrainDepth = (int)Mathf.Sqrt(meshVertices.Length);
            int terrainWidth = terrainDepth;

            //Calculamos los offsets basandonos en la posicion del tile
            float offsetX = -this.gameObject.transform.position.x;
            float offsetZ = -this.gameObject.transform.position.z;

            //Inicializamos el heightMap usando el generador de NoiseMap
            float[,] heightMap = this.noiseMap.GeneratePerlinNoiseMap(terrainDepth, terrainWidth, scale, offsetX, offsetZ, waves);

            //calculamos el offset del vertice basado en la posicion del tile y la distancia entre vertices
            Vector3 tileDimensions = this.meshFilter.mesh.bounds.size;
            float distanceBetweenVertices = tileDimensions.z / (float) terrainDepth;
            float vertexOffsetZ = this.gameObject.transform.position.z / distanceBetweenVertices; 

            //Creamos el heatMap uniforme usando el generador de UniformNoiseMap
            float[,] uniformHeatMap = this.noiseMap.GenerateUniformNoiseMap(terrainDepth, terrainWidth, centerVertexZ, maxDistanceZ, vertexOffsetZ);
            //Y generamos el heatMap aleatorio con el PerlinNoise
            float[,] randomHeatMap = this.noiseMap.GeneratePerlinNoiseMap(terrainDepth, terrainWidth, scale, offsetX, offsetZ, waves);
            float[,] heatMap = new float[terrainDepth, terrainWidth];

            for (int z = 0; z < terrainDepth; z++)
            {
                for (int x = 0; x < terrainWidth; x++)
                {
                    // mezclamos ambos heat maps juntos multiplicando cada uno de sus valores
                    heatMap[z,x] = uniformHeatMap[z,x] * randomHeatMap[z,x];
                    // hacemos las regiones más altas más frias, añadiendo el valor de la altura al heat map
                    heatMap[z,x] = heightMap[z,x] * heightMap[z,x];
                }
            }

            // generamos el moistureMap usando el Perlin Noise
            float[,] moistureMap = this.noiseMap.GeneratePerlinNoiseMap(terrainDepth, terrainWidth, scale, offsetX, offsetZ, moistureWaves);
            for (int z = 0; z < terrainDepth; z++)
            {
                for (int x = 0; x < terrainWidth; x++)
                {
                    // hacemos que las zonas más altas las más secas, reduciendo el valor del mapa
                    moistureMap[z,x] -= this.moistureCurve.Evaluate(heightMap[z,x]) * heightMap[z,x];
                }
            }

            // construimos la textura con el mapa de altura
            TerrainType[,] chosenHeightTerrainTypes = new TerrainType[terrainDepth, terrainWidth];
            Texture2D heightTexture = BuildTexture(heightMap, this.terrainTypes, chosenHeightTerrainTypes);
            // construimos la textura para el heat map
            TerrainType[,] chosenHeatTerrainTypes = new TerrainType[terrainDepth, terrainWidth];
            Texture2D heatTexture = BuildTexture(heatMap, this.heatTerrainTypes, chosenHeatTerrainTypes);
            // construimos la textura para el moisture map
            TerrainType[,] chosenMoistureTerrainTypes = new TerrainType[terrainDepth, terrainWidth];
            Texture2D moistureTexture = BuildTexture(moistureMap, this.moistureTerrainTypes, chosenMoistureTerrainTypes);

            // creamos una textura del bioma a partir de los tres valores anteriores
            Texture2D biomeTexture = BuildBiomeTexture(chosenHeightTerrainTypes, chosenHeatTerrainTypes, chosenMoistureTerrainTypes);

            //Mostraremos uno u otro en función del valor de visualizationMode
            switch (this.visualizationMode)
            {
                case VisualizationMode.Height:
                    this.meshRenderer.material.mainTexture = heightTexture;
                    break;
                case VisualizationMode.Heat:
                    this.meshRenderer.material.mainTexture = heatTexture;
                    break;
                case VisualizationMode.Moisture:
                    this.meshRenderer.material.mainTexture = moistureTexture;
                    break;
                case VisualizationMode.Biome:
                    this.meshRenderer.material.mainTexture = biomeTexture;
                    break;
                default:
                    break;
            }

            //Actualizamos el mesh de acuerdo al mapa de altura
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
                    meshVertices[vertexIndex] = new Vector3(vertex.x, this.heigthCurve.Evaluate(height) * heightMultiplier, vertex.z);
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
        /// para cada coordenada del heightMap, tendrá un color distinto en la escala de grises
        /// dependiendo de su valor, gracias a la funcion Color.Lerp (más negro, más profundo)
        /// </summary>
        /// <param name="heightMap"></param>
        /// <returns>textura del terreno en escala de grises</returns>
        private Texture2D BuildTexture(float[,] heightMap, TerrainType[] terrainTypes, TerrainType[,] chosenTerrainType) 
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
                    //TerrainType terrainType = GetTerrainType(height);
                    TerrainType terrainType = ChooseTerrainType(height, terrainTypes);
                    colorMap[colorIndex] = terrainType.color;

                        //guardamos el tipo de terreno escogido
                        chosenTerrainType[z,x] = terrainType;
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
        //Heat map
        TerrainType ChooseTerrainType(float height, TerrainType[] terrainTypes)
        {
            foreach (TerrainType type in terrainTypes)
            {
                if (height < type.height) return type;
            }
            //Devuelve el último tipo que debería ser el más alto
            return terrainTypes[terrainTypes.Length - 1];
        }

        //Para asignar un bioma a una region de acuerdo al tipo de terreno
        private Texture2D BuildBiomeTexture(TerrainType[,] heightTerrainTypes, TerrainType[,] heatTerrainTypes, TerrainType[,] moistureTerrainTypes)
        {
            int depth = heatTerrainTypes.GetLength(0);
            int width = heatTerrainTypes.GetLength(1);

            Color[] colorMap = new Color[depth*width];
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    int colorIndex = z * width + x;

                    TerrainType heightTerrainType = heightTerrainTypes[z,x];
                    // comprueba si la coordenada es de una region de agua
                    if(heightTerrainType.name != "water")
                    {
                        // si la coordenada no es agua, definimos el bioma por el calor y la humedad
                        TerrainType heatTerrainType = heatTerrainTypes[z,x];
                        TerrainType moistureTerrainType = moistureTerrainTypes[z,x];

                        // usamos el indice de la clase TerrainType para acceder a la tabla de biomas
                        Biome biome = this.biomes[moistureTerrainType.index].biomes[heatTerrainType.index];
                        // asignamos el color de acuerdo al bioma 
                        colorMap[colorIndex] = biome.color;
                    } else
                    {
                        // las regiones de agua no tienen un bioma, son siempre del mismo color
                        colorMap[colorIndex] = this.waterColor;
                    }
                }
            }
            // crea una nueva textura y establece sus colores
            Texture2D terrainTexture = new Texture2D(width, depth);
            terrainTexture.filterMode = FilterMode.Point;
            terrainTexture.wrapMode = TextureWrapMode.Clamp;
            terrainTexture.SetPixels(colorMap);
            terrainTexture.Apply();

            return terrainTexture;
        }

    }

    [System.Serializable]
    public class TerrainType
    {
        public string name;
        public float height;
        public Color color;
        public int index;
    }

    /**
     * Vamos a necesitar 2 clases, ya que en Unity no podemos tener un array 2D serializable:
     *  BiomeRow: representa una fila de biomas de la "tabla" de biomas que tenemos
     *  Biome: representa una celda de la tabla de biomas
     */

    [System.Serializable]
    public class Biome
    {
        public string name;
        public Color color;
    }

    [System.Serializable]
    public class BiomeRow
    {
        public Biome[] biomes;
    }

    enum VisualizationMode
    {
        Height,
        Heat,
        Moisture,
        Biome
    }

}
