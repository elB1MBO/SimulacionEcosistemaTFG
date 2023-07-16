using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private float speed = 0.04f;
    private float zoomSpeed = 5f;
    private float rotationSpeed = 0.1f;

    private float maxHeight=40f;
    private float minHeight=4f;

    Vector3 p1;
    Vector3 p2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetCameraMovement();
        SetCameraRotation();
    }

    void SetCameraMovement()
    {
        float hsp = 0f;
        if (Input.GetKey(KeyCode.A)) // Tecla para mover a la izquierda
        {
            hsp = -speed;
        }
        if (Input.GetKey(KeyCode.D)) // Tecla para mover a la derecha
        {
            hsp = speed;
        }
        //float hsp = speed * Input.GetAxis("Horizontal");
        float vsp = 0f;
        if (Input.GetKey(KeyCode.S)) // Tecla para mover a la izquierda
        {
            vsp = -speed;
        }
        if (Input.GetKey(KeyCode.W)) // Tecla para mover a la derecha
        {
            vsp = speed;
        }
        //float vsp = speed * Input.GetAxis("Vertical");
        float scrollSp = Mathf.Log(transform.position.y) * -zoomSpeed * Input.GetAxis("Mouse ScrollWheel");
        if (transform.position.y >= maxHeight && scrollSp > 0)
        {
            scrollSp = 0;
        }
        else if (transform.position.y <= minHeight && scrollSp < 0)
        { 
            scrollSp = 0; 
        }

        if ((transform.position.y + scrollSp) > maxHeight)
        {
            scrollSp = maxHeight - transform.position.y;
        } 
        else if ((transform.position.y + scrollSp) < minHeight)
        {
            scrollSp = minHeight - transform.position.y;
        }

        Vector3 verticalMove = new Vector3(0, scrollSp, 0);
        Vector3 lateralMove = hsp * transform.right; //transform.right pq queremos movernos lateralmente desde la persp de la camara
        Vector3 forwardMove = transform.forward;

        forwardMove.y = 0;
        forwardMove.Normalize();
        forwardMove *= vsp;

        Vector3 move = verticalMove + lateralMove + forwardMove;

        transform.position += move;
    }

    void SetCameraRotation()
    {
        if (Input.GetMouseButtonDown(1)) // comprueba si esta pulsado el boton central
        {
            p1 = Input.mousePosition; 
        }

        if (Input.GetMouseButton(1))
        {
            p2 = Input.mousePosition;

            float dx = (p1.x - p2.x) * rotationSpeed;
            float dy = (p1.y - p2.y) * rotationSpeed;

            transform.rotation *= Quaternion.Euler(new Vector3(0,dx,0));
            transform.GetChild(0).rotation *= Quaternion.Euler(new Vector3(-dy,0,0));

            p1 = p2;
        }
    }

}
