using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public float health = 100.0f;
    public TextMeshProUGUI healthText;
    
    public GameManager gameManager;
    public GameObject playerCamera;
    // Controlar el tiempo de vibración de la camara
    private float shakeTime = 1f;
    private float shakeDuration = 0.5f;
    private Quaternion playerCameraOriginalRotation;

    public CanvasGroup hitPanel;
    
    public PhotonView photonView;

    public GameObject activeWeapon;
    
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    void LateUpdate()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            playerCamera.gameObject.SetActive(false);
            return;
        }
        
        if(shakeTime < shakeDuration)
        {
            shakeTime += Time.deltaTime;
            CameraShake();
        }
        
        if (hitPanel.alpha > 0)
        {
            hitPanel.alpha -= Time.deltaTime;
        }
    }

    public void Hit(float damage)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("PlayerTakeDamage", RpcTarget.All, damage, photonView.ViewID);
        }
        else
        {
            PlayerTakeDamage(damage,photonView.ViewID);
        }
    }

    [PunRPC]
    public void PlayerTakeDamage(float damage, int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            health -= damage;
        
            if (health <= 0)
            {
                health = 0;
                gameManager.GameOver();
            }
            else
            {
                shakeTime = 0;
                hitPanel.alpha = 1;
            }
            
            healthText.text = $"{health} HP";
        }
    }
    
    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2f, 2f), 0, 0);
    }

    [PunRPC]
    public void WeaponShootSFX(int viewID)
    {
        activeWeapon.GetComponent<WeaponManager>().ShootVFX(viewID);
    }
}
