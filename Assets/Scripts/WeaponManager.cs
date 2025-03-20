using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject playerCam; // Fa referència a la càmera del jugador FPS
    public float range; // Fins on volem que arribin els tirs
    public float damage = 25f;
    
    public Animator playerAnimator;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //Debug.Log("Pium!");
            Shoot();
        }
        
        if (playerAnimator.GetBool("isShooting"))
        {
            //playerAnimator.SetBool("isShooting", false);
        }

    }

    private void Shoot()
    {
        playerAnimator.SetBool("isShooting", true);
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range))
        {
            //Debug.Log("Tocat!");
            // Si no hem ferit a un Zombie, la component EnemyManager valdrà null, però sinò prendrà el valor de la component del Zombie que hem ferit.
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.Hit(damage);
            }
        }

    }
}
