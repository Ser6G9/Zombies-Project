using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public ZombieStateMachine stateMachine;
    
    public Animator enemyAnimator;
    public GameObject player;
    public GameObject corpseTarget = null;
    public GameObject thisZombieCorpse;
    public float damage = 20.0f;
    public float chaseSpeed = 6f;
    public float walkSpeed = 2f;
    public bool canAtack = true;
    public float maxHealth = 100.0f;
    public float health;
    public GameManager gameManager;
    public Slider healthBar;
    
    // Animación y delay de ataque
    public float chaseDistance = 20f;
    public bool playerInReach = false;
    public float attackDelayTimer = 0f;
    public float howMuchEarlierStartAttackAnimation = 2f;
    public float delayBetweenAttacks = 1.0f;
    public float attackRange = 2.0f;

    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public float audioCountDownTimer = 0f;
    
    private GameObject[] playersInScene;
    
    public PhotonView photonView;
    
    void Start()
    {
        // Se asigna al jugador
        playersInScene = GameObject.FindGameObjectsWithTag("Player");
        healthBar.maxValue = health;
        healthBar.value = health;
        health = maxHealth;
        thisZombieCorpse.SetActive(false);
        
        stateMachine = new ZombieStateMachine(this);
        stateMachine.ChangeState(new IdleState());  // Estado inicial
    }


    void Update()
    {
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

        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        // Si está al 50% de vida o menos busca un cuerpo para comer
        if (((health*maxHealth)/100) <= 50 && corpseTarget==null)
        {
            SearchNearestCorpse();
        }
        
        stateMachine.Update();
        
        GetClosestPlayer();
        if(player != null)
        {
            // Se asigna al Player como el destino objetivo
            //GetComponent<NavMeshAgent>().destination = player.transform.position;
            
            healthBar.transform.LookAt(player.transform);
        }
        
        /*// Animación de movimiento
        if (GetComponent<NavMeshAgent>().velocity.magnitude > 1)
        {
            enemyAnimator.SetBool("isRunning", true);
        }
        else
        {
            enemyAnimator.SetBool("isRunning", false);
        }*/

        
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
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("TakeDamage", RpcTarget.All, damage, photonView.ViewID);
        }
        else
        {
            TakeDamage(damage, photonView.ViewID);
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            health -= damage;
            healthBar.value = health;
            if (health <= 0)
            {
                //Destroy(gameObject);
                Destroy(gameObject,40f);
                Destroy(GetComponent<BoxCollider>());
                Destroy(GetComponent<NavMeshAgent>());
                Destroy(GetComponent<EnemyManager>());
                Destroy(GetComponent<CapsuleCollider>());
                enemyAnimator.SetTrigger("isDead");
                //gameObject.tag = "Corpse";
                thisZombieCorpse.SetActive(true);

                if (!PhotonNetwork.InRoom || (PhotonNetwork.IsMasterClient && photonView.IsMine))
                {
                    gameManager.enemiesAlive--;
                }
                
            }
        }
    }

    private void GetClosestPlayer()
    {
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject p in playersInScene)
        {
            if (p != null)
            {
                float distance = Vector3.Distance(p.transform.position, currentPosition);
                if (distance < minDistance)
                {
                    player = p;
                    minDistance = distance;
                }
            }
        }
    }
    
    public bool JugadorALaVista()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < chaseDistance)
            {
                NavMeshPath path = new NavMeshPath();
                NavMeshAgent agent = GetComponent<NavMeshAgent>();
                            
                // Calcula el camino hasta el jugador
                bool pathFound = agent.CalculatePath(player.transform.position, path);
                            
                // Es alcanzable si el path es completo
                return pathFound && path.status == NavMeshPathStatus.PathComplete;
            }
        }
        return false;
    }
    
    public void SearchNearestCorpse(float detectionRadius = 10f)
    {
        GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject corpse in corpses)
        {
            float dist = Vector3.Distance(currentPos, corpse.transform.position);
            if (dist < minDistance && dist <= detectionRadius)
            {
                corpseTarget = corpse;
                minDistance = dist;
                stateMachine.ChangeState(new EatCorpseState());
            }
        }
        
        
    }
}
