using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public Animator enemyAnimator;
    public GameObject player;
    public float damage = 20.0f;
    public float damageDelay = 1.0f;
    public bool canAtack = true;
    public float health = 100.0f;
    public GameManager gameManager;
    public Slider healthBar;
    
    void Start()
    {
        // Aquest cop, no arrossegarem la variable GameObject del FPS
        // des de l'inspector, sinò que l'assginarem des del codi
        // En concret volem cercar al jugador principal!!
        player = GameObject.FindGameObjectWithTag("Player");
        healthBar.maxValue = health;
        healthBar.value = health;
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
        
        // Para impedir que el Zombie golpee muchas veces seguidas, se ha aplicado un segundo de delay entre ataques.
        

    }

    // Detectar la col·lisió
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == player && canAtack)
        {
            Debug.Log("El FPS m'ataca!!");
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAtack = false;
        player.GetComponent<PlayerManager>().Hit(damage);
        yield return new WaitForSeconds(damageDelay);
        canAtack = true;
    }
    
    public void Hit(float damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
        {
            Destroy(gameObject);
            gameManager.enemiesAlive--;
        }
    }

    
}
