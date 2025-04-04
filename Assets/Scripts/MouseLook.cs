using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float lookSpeed = 2;
    private Vector2 rotation = Vector2.zero;
    
    public GameObject player;

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor en el centro de la pantalla
        Look();
    }

    public void Look()
    {
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += Input.GetAxis("Mouse Y");
        
        rotation.x = Mathf.Clamp(rotation.x, -35f, 35f);
        
        transform.eulerAngles = new Vector2(0, rotation.y) * lookSpeed;
        
        player.transform.rotation = Quaternion.Euler(-rotation.x * lookSpeed, rotation.y * lookSpeed, 0f);
       
    }
}
