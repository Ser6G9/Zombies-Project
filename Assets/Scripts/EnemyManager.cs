using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public GameObject player;
    public float damage = 20.0f;
    public Animator enemyAnimator;

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            player.Hit(damage);
        }
    }
    
    void Start()
    {
        // Aquest cop, no arrossegarem la variable GameObject del FPS
        // des de l'inspector, sinò que l'assginarem des del codi
        // En concret volem cercar al jugador principal!!
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        // Accedim al component NavMeshComponent, el qual té un element que es destination de tipus Vector3
        // Li podem assignar la posició del jugador, que el tenim a la variable player gràcies al seu tranform
        GetComponent<NavMeshAgent>().destination = player.transform.position;
    
        // En primer lloc hem d'accedir a la velocitat del Zombiem, des del component NavMeshAgent
        if (GetComponent<NavMeshAgent>().velocity.magnitude > 1)
        {
            enemyAnimator.SetBool("isRunning", true);
        }
        else
        {
            enemyAnimator.SetBool("isRunning", false);
        }

    }
    
    // Detectar la col·lisió
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == player)
        {
            Debug.Log("L'enemic m'ataca!!");
        }
    }

}
