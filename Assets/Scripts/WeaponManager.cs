using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject playerCam; // Fa referència a la càmera del jugador FPS
    public float range; // Fins on volem que arribin els tirs
    public float damage = 25f;
    public float fireRate = 0.5f;
    public float fireRateTimer = 0.0f;
    
    public GameManager gameManager;
    public Animator playerAnimator;

    public ParticleSystem flashParticleSystem;
    public GameObject bloodParticleSystem;
    public GameObject impactParticleSystem;
    
    public AudioSource audioSource;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && gameManager.paused == false && gameManager.gameOver == false)
        {
            Shoot();
        }
        if (Input.GetButton("Fire1") && gameManager.paused == false && gameManager.gameOver == false)
        {
            fireRateTimer += Time.deltaTime;
            if(fireRateTimer >= fireRate)
            {
                //Debug.Log("Pium!");
                Shoot();
                fireRateTimer = 0;
            }
        }
        else
        {
            fireRateTimer = 0;
        }
        
        if (playerAnimator.GetBool("isShooting"))
        {
            playerAnimator.SetBool("isShooting", false);
        }

    }

    private void Shoot()
    {
        playerAnimator.SetBool("isShooting", true);
        audioSource.Play();
        flashParticleSystem.Play();
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range))
        {
            //Debug.Log("Tocat!");
            // Si no hem ferit a un Zombie, la component EnemyManager valdrà null, però sinò prendrà el valor de la component del Zombie que hem ferit.
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                // generam una instància del particle system, en el punt on hem ferit al Zombie,
                // i fent que l'animació sempre estigui rotada en direcció al tret
                GameObject particleInstance = Instantiate(bloodParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
                // Feim que la instància sigui filla del Zombie al qual hem ferit
                particleInstance.transform.parent = hit.transform;
                // Recordau que aquesta animació te seleccionat per Stop Action: "Destroy" ja que sinó es crearien infinites instàncies

                enemyManager.Hit(damage);
            }
            else
            {
                GameObject particleInstance = Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
                particleInstance.transform.parent = hit.transform;
            }
        }

    }
}
