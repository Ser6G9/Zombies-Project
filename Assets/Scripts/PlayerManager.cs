using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public float health = 100.0f;
    public TextMeshProUGUI healthText;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Hit(float damage)
    {
        health -= damage;
        healthText.text = health.ToString();

        if (health <= 0)
        {
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
            gameManager.GameOver();
        }
    }
}
