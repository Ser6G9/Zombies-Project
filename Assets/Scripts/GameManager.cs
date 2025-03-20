using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int enemiesAlive;
    public int round;
    public GameObject player;

    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI roundsText;
    public GameObject pausePanel;
    public bool paused = false;
    public GameObject gameOverPanel;

    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        NextWave(50);
    }

    // Update is called once per frame
    void Update()
    {
        if (roundsText != null)
        {
            enemiesText.text = "Enemigos restantes: "+enemiesAlive;
            roundsText.text = "Oleada "+round;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) && !paused)
        {
            Pause();
        } else if (Input.GetKeyDown(KeyCode.Escape) && paused)
        {
            Resume();
        }
    }
    
    public void NextWave(int round)
    {
        for (int i = 0; i < round; i++)
        {
            int randPos = Random.Range(0, spawnPoints.Length);
            GameObject spawnPoint = spawnPoints[randPos];

            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemyInstance.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
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
