using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public int enemiesAlive;
    public int round = 1;
    public GameObject player;

    public GameObject[] spawnPoints;
    public GameObject[] enemysPrefabs;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI roundsText;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    
    public bool paused = false;
    public bool gameOver = false;
    
    public static GameManager sharedInstance;

    private void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        NextWave(round);
        
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    // Update is called once per frame
    void Update()
    {
            if (roundsText != null)
            {
                enemiesText.text = "Enemigos restantes: "+enemiesAlive;
                roundsText.text = "Oleada "+round;
            }
                    
            if (Input.GetKeyDown(KeyCode.Escape) && !paused && !gameOver)
            {
                Pause();
            } else if (Input.GetKeyDown(KeyCode.Escape) && paused && !gameOver)
            {
                Resume();
            }
            
            if (enemiesAlive <= 0)
            {
                round++;
                NextWave(round);
            }
    }
    
    public void NextWave(int round)
    {
        for (int i = 0; i < round; i++)
        {
            int randPos = Random.Range(0, spawnPoints.Length);
            GameObject spawnPoint = spawnPoints[randPos];

            GameObject enemyInstance = Instantiate(SetRandomEnemy(), spawnPoint.transform.position, Quaternion.identity);
            enemyInstance.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    public GameObject SetRandomEnemy()
    {
        int randPos = Random.Range(0, enemysPrefabs.Length);
        return enemysPrefabs[randPos];
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        paused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        PlayerCanPlay(false);
    }
    
    public void Resume()
    {
        paused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        PlayerCanPlay(true);
    }

    public void GameOver()
    {
        gameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        PlayerCanPlay(false);
    }

    public void PlayerCanPlay(bool state)
    {
        if (state)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        player.GetComponent<MouseLook>().enabled = state;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
