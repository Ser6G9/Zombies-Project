using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
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
    
    public PhotonView photonView;
    
    void Start()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        if (!PhotonNetwork.InRoom)
        {
            //Parar el tiempo
            Time.timeScale = 1;
        }
        
        // Instanciar los spwnPoints de enemigos:
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    void Update()
    {
        if (!PhotonNetwork.InRoom || (PhotonNetwork.IsMasterClient && photonView.IsMine))
        {
            // Siguiente ronda si no quedan enemigos
            if (enemiesAlive <= 0)
            {
                round++;
                NextWave(round);
            }
            
            if (PhotonNetwork.InRoom)
            {
                Hashtable hash = new Hashtable();
                hash.Add("currentRound", round);
                hash.Add("enemiesAlive", enemiesAlive);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
            else
            {
                UpdateHUDinfo(round,enemiesAlive);
            }
            //UpdateHUDinfo(round,enemiesAlive);
        }
        
        // Pause
        if (Input.GetKeyDown(KeyCode.Escape) && !paused && !gameOver)
        {
            Pause();
        } else if (Input.GetKeyDown(KeyCode.Escape) && paused && !gameOver)
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
            GameObject enemyInstance;
            
            GameObject zombieEnemy = SetRandomEnemy();
            if (PhotonNetwork.InRoom)
            {
                enemyInstance = PhotonNetwork.Instantiate(zombieEnemy.name, spawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                enemyInstance = Instantiate(zombieEnemy, spawnPoint.transform.position, Quaternion.identity);
            }
            enemyInstance.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    public void UpdateHUDinfo(int rounds, int enemies)
    {
        // Actualizar HUD
        enemiesText.text = "Enemigos restantes: "+enemies; 
        roundsText.text = "Oleada "+rounds;
    }

    // Como tengo más de un tipo de Zombie, con este método hago que a instancia de enemigos sea uno de los tipos de Zombie aleatorios cada vez.
    public GameObject SetRandomEnemy()
    {
        int randPos = Random.Range(0, enemysPrefabs.Length);
        return enemysPrefabs[randPos];
    }
    
    // Gestion de menús:
    public void RestartGame()
    {
        if (!PhotonNetwork.InRoom)
        {
            //reanudar el tiempo
            Time.timeScale = 1;
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(3);
        }
        
    }

    public void BackToMainMenu()
    {
        if (!PhotonNetwork.InRoom)
        {
            //reanudar el tiempo
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    public void Pause()
    {
        paused = true;
        pausePanel.SetActive(true);
        if (!PhotonNetwork.InRoom)
        {
            //Parar el tiempo
            Time.timeScale = 0;
        }
        PlayerCanPlay(false);
    }
    
    public void Resume()
    {
        paused = false;
        pausePanel.SetActive(false);
        if (!PhotonNetwork.InRoom)
        {
            //reanudar el tiempo
            Time.timeScale = 1;
        }
        PlayerCanPlay(true);
    }

    public void GameOver()
    {
        gameOver = true;
        gameOverPanel.SetActive(true);

        if (!PhotonNetwork.InRoom)
        {
            //Parar el tiempo
            Time.timeScale = 0;
        }
        
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
        player.GetComponent<PlayerMovement>().enabled = state;
    }
    

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (photonView.IsMine)
        {
            if (changedProps["currentRound"] != null && changedProps["enemiesAlive"] != null)
            {
                UpdateHUDinfo((int)changedProps["currentRound"],(int)changedProps["enemiesAlive"]);
            }
        }
    }
}
