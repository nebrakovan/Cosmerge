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

    // ������� ��� ���������� UI � ������ ������
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

    // ������������� ����
    private void InitializeGame()
    {
        CurrentState = GameState.Init;
        // �������������� �������������
        Debug.Log("���� ����������������");
    }

    // ������ ����� ����
    public void StartNewGame()
    {
        CurrentState = GameState.InGame;
        OnGameStarted?.Invoke();
        SceneManager.LoadScene("GameScene"); // �������� �� ��� ����� �������� ������� �����
    }

    // ����� ����
    public void PauseGame()
    {
        if (CurrentState == GameState.InGame)
        {
            CurrentState = GameState.Paused;
            OnGamePaused?.Invoke();
            Time.timeScale = 0f; // ��������� ������� � ����
        }
    }

    // ������������� ����
    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.InGame;
            OnGameResumed?.Invoke();
            Time.timeScale = 1f; // ������������� ������� � ����
        }
    }

    // ���������� ����
    public void GameOver()
    {
        Debug.Log("��������");

/*        CurrentState = GameState.GameOver;
        OnGameOver?.Invoke();
        SceneManager.LoadScene("GameOverScene"); */
    }

    // ����� �� ����
    public void QuitGame()
    {
        Application.Quit();
    }
}
