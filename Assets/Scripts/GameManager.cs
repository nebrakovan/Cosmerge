using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        // Implement singleton pattern to ensure only one instance exists.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this instance alive across scene changes.
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //InitializeGame();
        StartGame();
    }

    /*private void InitializeGame()
    {
        // Set the initial game state. Typically, you might start at the main menu.
        CurrentState = GameState.MainMenu;
        // Load the main menu scene or perform any initial setup.
        SceneManager.LoadScene("MainMenuScene"); // Replace with your actual main menu scene name.
    }*/

    public void StartGame()
    {
        // Update game state and load the main game scene.
        CurrentState = GameState.Playing;
        SceneManager.LoadScene("GameScene"); // Replace with your actual game scene name.
    }

    public void TogglePause()
    {
        // Check if the current game state is 'Paused'
        if (CurrentState == GameState.Paused)
        {
            // If the game is currently paused, resume the game
            CurrentState = GameState.Playing;
            Time.timeScale = 1f; // Resume time
        }
        else if (CurrentState == GameState.Playing)
        {
            // If the game is currently playing, pause the game
            CurrentState = GameState.Paused;
            Time.timeScale = 0f; // Pause time
        }
    }

    public void GameOver()
    {
        // Update game state and possibly load a game over scene or display a game over UI.
        CurrentState = GameState.GameOver;
        Debug.Log("Game Over!"); // Placeholder action.
        // SceneManager.LoadScene("GameOverScene"); // Uncomment and replace with your actual game over scene name if applicable.
    }

    public void ReturnToMainMenu()
    {
        // Update game state and load the main menu scene.
        CurrentState = GameState.MainMenu;
        SceneManager.LoadScene("MainMenuScene"); // Replace with your actual main menu scene name.
        Time.timeScale = 1f; // Ensure the game's time scale is reset.
    }

    public void RestartGame()
    {
        // Optionally reset any game data here.

        // Update game state and reload the game scene to restart the game.
        CurrentState = GameState.Playing;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
