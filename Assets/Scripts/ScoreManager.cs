using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TMP_Text scoreText; // Текстовое поле для отображения счета
    public TMP_Text highScoreText; // Текстовое поле для отображения рекорда
    public TMP_Text tempScoreText; // Текст для временного счета

    private Animator tempScoreAnimator; // Аниматор для временного счета
    private int score = 0;
    private int highScore = 0;
    private int comboMultiplier = 1; // Множитель для комбо
    private int pendingScore = 0; // Накопленные очки, ожидающие добавления к счету
    private float comboTimer = 0f; // Таймер для комбо
    private float comboTimeLimit = 2f; // Время для комбо (в секундах)
    [SerializeField] private int maxComboMultiplier = 10; // Максимальное значение множителя комбо

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScore();
        }
        else
        {
            Destroy(gameObject);
        }

        tempScoreAnimator = tempScoreText.GetComponent<Animator>();
    }

    void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ApplyComboScore();
            }
        }
    }

    public void AddScore(int points, Vector3 position)
    {
        tempScoreText.text = "+" + points * comboMultiplier;
        tempScoreText.transform.position = Camera.main.WorldToScreenPoint(position);
        tempScoreText.gameObject.SetActive(true);

        // Запускаем анимацию для временного счета
        tempScoreAnimator.SetTrigger("AddScore");

        // Запускаем корутину, которая дождется окончания анимации
        StartCoroutine(WaitForAnimationToEnd());

        pendingScore += points * comboMultiplier; // Умножаем очки на текущий множитель
        comboMultiplier = Mathf.Min(comboMultiplier * 2, maxComboMultiplier); // Удвоение множителя с ограничением
        comboTimer = comboTimeLimit; // Сброс таймера комбо
    }

    private IEnumerator WaitForAnimationToEnd()
    {
        // Ожидание завершения анимации
        yield return new WaitWhile(() => tempScoreAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);

        // Скрываем текст после завершения анимации
        tempScoreText.gameObject.SetActive(false);
    }

    private void ApplyComboScore()
    {
        score += pendingScore; // Добавляем накопленные очки к счету
        UpdateScoreText();

        // Проверка на новый рекорд
        if (score > highScore)
        {
            highScore = score;
            UpdateHighScoreText();
            SaveHighScore();
        }

        pendingScore = 0; // Сброс накопленных очков
        comboMultiplier = 1; // Сброс множителя комбо
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    private void UpdateHighScoreText()
    {
        highScoreText.text = "High Score: " + highScore;
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreText();
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
    }

    public void ResetScore()
    {
        score = 0;
        pendingScore = 0;
        comboMultiplier = 1;
        UpdateScoreText();
    }
}
