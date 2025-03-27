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
    public bool canAtack = true;
    public float health = 100.0f;
    public GameManager gameManager;
    public Slider healthBar;
    
    // Animacio i millora del xoc
    public bool playerInReach = false;
    public float attackDelayTimer = 0f;
    public float howMuchEarlierStartAttackAnimation = 2f;
    public float delayBetweenAttacks = 1.0f;

    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public float audioCountDownTimer = 0f;
    
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

        // ¿Cuanto tarda en hacer ruido el Zombie?
        if (audioCountDownTimer <= 2)
        {
            if (health > 0)
            {
                audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
                audioSource.Play();
                audioCountDownTimer = Random.Range(2, 8);
            }
        }
        else 
        {
            audioCountDownTimer -= Time.deltaTime;
        }
        
        

    }

    // Detectar la col·lisió
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == player && canAtack)
        {
            //Debug.Log("El FPS m'ataca!!");
            //player.GetComponent<PlayerManager>().Hit(damage);
            playerInReach = true;
        }
    }
    
    private void OnCollisionStay(Collision collision)
    {
        if (playerInReach)
        {
            attackDelayTimer += Time.deltaTime;
            if(attackDelayTimer >= delayBetweenAttacks - howMuchEarlierStartAttackAnimation && attackDelayTimer <= delayBetweenAttacks)
            {
                enemyAnimator.SetTrigger("isAttacking");
            }
            if(attackDelayTimer >= delayBetweenAttacks)
            {
                player.GetComponent<PlayerManager>().Hit(damage);
                attackDelayTimer = 0;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            playerInReach = false;
            attackDelayTimer = 0;
        }
    }
    
    public void Hit(float damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
        {
            //Destroy(gameObject);
            Destroy(gameObject,30f);
            Destroy(GetComponent<BoxCollider>());
            Destroy(GetComponent<NavMeshAgent>());
            Destroy(GetComponent<EnemyManager>());
            Destroy(GetComponent<CapsuleCollider>());
            enemyAnimator.SetTrigger("isDead");

            gameManager.enemiesAlive--;
        }
    }

    
}
