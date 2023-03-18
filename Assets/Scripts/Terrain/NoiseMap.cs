using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{
    /// <summary>
    /// Genera una matriz que representa el noise map, con el ruido en cada coordenada, teniendo
    /// en cuenta el offset del tile contiguo
    /// </summary>
    /// <param name="depth"> profundidad del mapa: eje z </param>
    /// <param name="width"> ancho del mapa: eje x </param>
    /// <param name="scale"> funciona como el zoom en el nivel </param>
    /// <param name="offsetX">valor x del tile anterior para generar el continuo</param>
    /// <param name="offsetZ">valor z del tile anterior para generar el continuo</param>
    /// <returns> matriz con el NoiseMap </returns>
    /// 
    public float[,] GeneratePerlinNoiseMap(int depth, int width, float scale, float offsetX, float offsetZ, Wave[] waves)
    {
        //creamos la matriz noiseMap (depth y width deben ser de tipo int):
        float[,] noiseMap = new float[depth, width];

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float scaledZ = (z + offsetZ) / scale;
                float scaledX = (x + offsetX) / scale;

                float noise = 0f;
                float normalization = 0f;
                foreach (Wave wave in waves)
                {
                    //Generamos el ruido usando el PerlinNoise para una onda dada
                    noise += wave.amplitude * Mathf.PerlinNoise(scaledX * wave.frequency + wave.seed, scaledZ * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }
                //Normalizamos el valor del noise para que esté entre 0 y 1
                noise /= normalization;
                noiseMap[z,x] = noise;
            }
        }
        return noiseMap;
    }

    public float[,] GenerateUniformNoiseMap(int depth, int width, float centerVertexZ, float maxDistanceZ, float offsetZ)
    {
        //crea un noise map vacio con los valores coordenadas depth y width
        float[,] noiseMap = new float[depth, width];

        for(int z=0; z<depth; z++)
        {
            // calcula el valor z de ejemplo sumando el indice y el offset
            float sampleZ = z + offsetZ;
            // calcula el ruido proporcional a la distancia del valor sampleZ al centro del nivel
            float noise = Mathf.Abs(sampleZ - centerVertexZ) / maxDistanceZ;
            // aplica el ruido a todos los puntos con su coordenada z
            for (int x = 0; x < width; x++)
            {
                noiseMap[depth - z - 1, x] = noise;
            }
        }
        return noiseMap;
    }
}

[System.Serializable]
public class Wave
{
    public float seed;
    public float frequency;
    public float amplitude;
}
