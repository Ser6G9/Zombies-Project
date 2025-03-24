using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public float health = 100.0f;
    public TextMeshProUGUI healthText;
    
    public GameObject playerCamera;
    // Variable per controlar el temps de vibració de la càmera
    private float shakeTime = 1f;
    private float shakeDuration = 0.5f;
    private Quaternion playerCameraOriginalRotation;

    public CanvasGroup hitPanel;

    void Start()
    {
        
    }

    void LateUpdate()
    {
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
        healthText.text = health.ToString();

        if (health <= 0)
        {
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
            gameManager.GameOver();
        }
        else
        {
            shakeTime = 0;
        }
    }
    
    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2f, 2f), 0, 0);
    }

}
