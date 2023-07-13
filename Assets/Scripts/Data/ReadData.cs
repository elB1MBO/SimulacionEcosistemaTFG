using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReadData : MonoBehaviour
{

    public string fileName; 

    // Start is called before the first frame update
    void Start()
    {
        string filePath = "Assets/Data/" + fileName;
        Debug.Log(filePath);
        if (File.Exists(filePath))
        {
            //Lee el contenido del archivo
            string content = File.ReadAllText(filePath);

            //Comprobar que lo lee
            Debug.Log(content);
        } else
        {
            Debug.LogError("El archivo no existe");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
