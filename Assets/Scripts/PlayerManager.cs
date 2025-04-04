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
    
    public GameObject playerCamera;
    // Controlar el tiempo de vibración de la camara
    private float shakeTime = 1f;
    private float shakeDuration = 0.5f;
    private Quaternion playerCameraOriginalRotation;

    public CanvasGroup hitPanel;
    
    public PhotonView photonView;

    void Start()
    {
        
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
        hitPanel.alpha = 1;
        health -= damage;
        
        if (health <= 0)
        {
            health = 0;
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
            gameManager.GameOver();
        }
        else
        {
            shakeTime = 0;
        }

        healthText.text = health.ToString();
    }
    
    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2f, 2f), 0, 0);
    }

}
