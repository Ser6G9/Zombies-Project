using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 8.0f;
    public float jumpHeight = 2f;
    
    private Vector3 velocity;
    public float gravity = -9.81f;
    
    // GroundCheck
    public bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.4f; //Umbral de distància enterra
    public LayerMask groundMask;


    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(horizontal * Time.deltaTime * speed, 0,
            vertical * Time.deltaTime * speed);
        
        // Gravetat
        // Formula de velocitat = acceleració * temps^2
        velocity.y += gravity * Time.deltaTime;
        //controller.Move(velocity * Time.deltaTime);
        //print(velocity.y);
        
        // Mirar si estic tocant el terra
        isGrounded = Physics.CheckSphere(groundCheck.position,groundDistance,groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }


    }
}
