using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Init, InGame, Paused, GameOver }
    public GameState CurrentState { get; private set; }

    // События для обновления UI и других систем
    public event Action OnGameStarted;
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    public event Action OnGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGame();
    }

    // Инициализация игры
    private void InitializeGame()
    {
        CurrentState = GameState.Init;
        // Дополнительная инициализация
        Debug.Log("Игра инициализирована");
    }

    // Начало новой игры
    public void StartNewGame()
    {
        CurrentState = GameState.InGame;
        OnGameStarted?.Invoke();
        SceneManager.LoadScene("GameScene"); // Замените на имя вашей основной игровой сцены
    }

    // Пауза игры
    public void PauseGame()
    {
        if (CurrentState == GameState.InGame)
        {
            CurrentState = GameState.Paused;
            OnGamePaused?.Invoke();
            Time.timeScale = 0f; // Остановка времени в игре
        }
    }

    // Возобновление игры
    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.InGame;
            OnGameResumed?.Invoke();
            Time.timeScale = 1f; // Возобновление времени в игре
        }
    }

    // Завершение игры
    public void GameOver()
    {
        Debug.Log("ПРОИГРАЛ");

/*        CurrentState = GameState.GameOver;
        OnGameOver?.Invoke();
        SceneManager.LoadScene("GameOverScene"); */
    }

    // Выход из игры
    public void QuitGame()
    {
        Application.Quit();
    }
}
