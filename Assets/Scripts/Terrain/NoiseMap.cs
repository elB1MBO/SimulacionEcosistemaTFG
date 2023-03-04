using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{

    /// <summary>
    /// Genera una matriz que representa el noise map, con el ruido en cada coordenada
    /// </summary>
    /// <param name="depth"> profundidad del mapa: eje z </param>
    /// <param name="width"> ancho del mapa: eje x </param>
    /// <param name="scale"> funciona como el zoom en el nivel </param>
    /// <returns> matriz con el NoiseMap </returns>
    public float[,] GenerateNoiseMap(int depth, int width, float scale)
    {
        //creamos la matriz noiseMap (depth y width deben ser de tipo int):
        float[,] noiseMap = new float[depth, width];

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float scaledZ = z / scale;
                float scaledX = x / scale;

                float noise = Mathf.PerlinNoise(scaledX, scaledZ);
                noiseMap[z,x] = noise;
            }
        }
        return noiseMap;
    }

}
