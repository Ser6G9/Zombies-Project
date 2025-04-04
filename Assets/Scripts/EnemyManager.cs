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
    
    // Animación y delay de ataque
    public bool playerInReach = false;
    public float attackDelayTimer = 0f;
    public float howMuchEarlierStartAttackAnimation = 2f;
    public float delayBetweenAttacks = 1.0f;

    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public float audioCountDownTimer = 0f;
    
    void Start()
    {
        // Se asigna al jugador
        player = GameObject.FindGameObjectWithTag("Player");
        healthBar.maxValue = health;
        healthBar.value = health;
    }


    void Update()
    {
        // Se asigna al Player como el destino objetivo
        GetComponent<NavMeshAgent>().destination = player.transform.position;
    
        // Animación de movimiento
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

    // Detectar colisión con jugador
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == player && canAtack)
        {
            //Debug.Log("Mordisco!!");
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
