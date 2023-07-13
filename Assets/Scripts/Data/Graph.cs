using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] public List<float> datos;
    public RectTransform graphLine;
    public string fileName;

    // Start is called before the first frame update
    void Start()
    {
        datos = new List<float>();

        string filePath = "Assets/Data/" + fileName;

        if (File.Exists(filePath))
        {
            // Lee los datos del archivo de texto
            string[] lineas = File.ReadAllLines(filePath);

            // Convierte las líneas a valores numéricos y los agrega a la lista de datos
            foreach (string linea in lineas)
            {
                float dato;
                if (float.TryParse(linea, out dato))
                {
                    datos.Add(dato);
                }
            }
        }
        else
        {
            Debug.LogError("El archivo no existe: " + fileName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGraph();
    }

    void UpdateGraph()
    {
        // Ajusta el tamaño de la gráfica según el número de datos
        float anchoPunto = graphLine.rect.width / datos.Count;

        // Recorre los datos y actualiza la posición y tamaño de la línea de la gráfica
        for (int i = 0; i < datos.Count; i++)
        {
            float alturaPunto = datos[i] * graphLine.rect.height;
            RectTransform punto = Instantiate(graphLine, transform);
            punto.localPosition = new Vector3(i * anchoPunto, alturaPunto, 0f);
            punto.sizeDelta = new Vector2(anchoPunto, alturaPunto);
        }
    }
}
