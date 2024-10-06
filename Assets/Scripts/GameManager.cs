using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UiManager ui;
    public FogOfWar fog;
    public AntHill anthill;
    public WaveManager wave;
    public BuildingManager build;
    public UpgradesManager upgrade;
    public FoodSpawner spawner;
    public int kills;
    public TMP_InputField nameInput;
    public GameObject submitButton;
    public GameObject submitError;
    public List<Creature> playersCreatures = new List<Creature>();
    public PlayerController player;

    public bool isPaused = false;
    public bool gameStarted = false;
    public GameObject paused;
    public GameObject gameOverScreen;

    private void Awake()
    {
        instance = this;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        gameStarted = true;
        wave.enabled = true;
        spawner.enabled = true;
        ui.mainMenu.SetActive(false);
        ui.inGameUi.SetActive(true);
    }

    public void ResetGame()
    {
        SceneManager.UnloadSceneAsync(0);
        SceneManager.LoadSceneAsync(0);
    }
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void TogglePause()
    {
        if (gameStarted)
        {
            isPaused = !isPaused;
            if(isPaused)
            {
                Time.timeScale = 0;
                paused.SetActive(true);
                ui.inGameUi.SetActive(false);
            } else
            {
                Time.timeScale = 1;
                paused.SetActive(false);
                ui.inGameUi.SetActive(true);
            }
        }
    }
}
