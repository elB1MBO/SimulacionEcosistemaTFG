using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: esto ocurre si está seleccionado el icono de Gallina de la interfaz. Lo mismo con el zorro. Hay que tener en cuenta la camara que esconde el ratón al hacer click4.
        //PROVISIONAL
        //if (Input.GetKeyDown(KeyCode.H)) { animal = 0; }
        //if (Input.GetKeyDown(KeyCode.F)) { animal = 1; }
        //switch (animal)
        //{
        //    case 0:
        //        if (Input.GetMouseButtonDown(1))
        //        {
        //            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //            RaycastHit hit;

        //            if (Physics.Raycast(ray, out hit))
        //            {
        //                Vector3 pos = new Vector3(hit.point.x, hit.point.y + hen.transform.position.y, hit.point.z);
        //                if (pos.y < 5)
        //                {
        //                    GameObject newHen = Instantiate(hen, pos, Quaternion.identity, this.HenContainer.transform);
        //                    newHen.GetComponent<Animal>().SetAnimalContainer(HenContainer);
        //                }
        //                else { Debug.LogWarning("No puedes colocarlo sobre un árbol"); }
        //            }
        //        }
        //        break;

        //    case 1:
        //        if (Input.GetMouseButtonDown(1))
        //        {
        //            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //            RaycastHit hit;

        //            if (Physics.Raycast(ray, out hit))
        //            {
        //                Vector3 pos = new Vector3(hit.point.x, hit.point.y + fox.transform.position.y, hit.point.z);
        //                if (pos.y < 5)
        //                {
        //                    GameObject newFox = Instantiate(fox, pos, Quaternion.identity, this.HenContainer.transform);
        //                    newFox.GetComponent<Animal>().SetAnimalContainer(FoxContainer);
        //                }
        //                else { Debug.LogWarning("No puedes colocarlo sobre un árbol"); }
        //            }
        //        }
        //        break;

        //    default:
        //        Debug.Log("Primero debes elegir un tipo de animal");
        //        break;
        //}
        
    }
}
