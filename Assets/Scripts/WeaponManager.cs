using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject playerCam; // Referencia a la camara del jugador FPS
    public float range; // Distancia de los disparos
    public float damage = 25f;
    public float fireRate = 0.5f;
    public float fireRateTimer = 0.0f;
    
    public Animator playerAnimator;

    public ParticleSystem flashParticleSystem;
    public GameObject bloodParticleSystem;
    public GameObject impactParticleSystem;
    
    public AudioSource audioSource;
    
    public PhotonView photonView;

    public GameManager gameManager;
    
    void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }
        
        if (!gameManager.paused && !gameManager.gameOver)
        {
            // Animación del FPS
            if (playerAnimator.GetBool("isShooting"))
            {
                playerAnimator.SetBool("isShooting", false);
            }
            
            // Disparo único
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
            // Disparo automático (mantener pulsado)
            if (Input.GetButton("Fire1"))
            {
                fireRateTimer += Time.deltaTime;
                if(fireRateTimer >= fireRate)
                {
                    Shoot();
                    fireRateTimer = 0;
                }
            }
            else
            {
                fireRateTimer = 0;
            }
        }
    }

    private void Shoot()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("WeaponShootSFX", RpcTarget.All, photonView.ViewID);
        }
        else
        {
            ShootVFX(photonView.ViewID);
        }
        
        playerAnimator.SetBool("isShooting", true);
        
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range))
        {
            // Se detecta si se acierta a un Zombie
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                // se genera una instacia de las particulas de sangre en la posición donde se acertó al zombie.
                GameObject particleInstance = Instantiate(bloodParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
                // Esta instancia es hija del objeto Zombie
                particleInstance.transform.parent = hit.transform;

                // Se le aplica el daño al Zombie
                enemyManager.Hit(damage);
            }
            else
            {
                // Si no le da a un Zombie, instancia las particulas predeterminadas de impacto
                GameObject particleInstance = Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
                particleInstance.transform.parent = hit.transform;
            }
        }

    }

    public void ShootVFX(int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            audioSource.Play();
            flashParticleSystem.Play();
        }
    }
}
