using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TMP_Text scoreText; // Текстовое поле для отображения счета
    public TMP_Text highScoreText; // Текстовое поле для отображения рекорда

    public GameObject canvas; // Ссылка на Canvas


    public TMP_Text tempScoreTextPrefab; // Префаб для временного счета
    [SerializeField]private List<TMP_Text> tempScoreTextPool = new List<TMP_Text>();

    private int score = 0;
    private int highScore = 0;
    private int comboMultiplier = 1; // Множитель для комбо
    private int pendingScore = 0; // Накопленные очки, ожидающие добавления к счету
    private float comboTimer = 0f; // Таймер для комбо
    private float comboTimeLimit = 2f; // Время для комбо (в секундах)
    [SerializeField] private int maxComboMultiplier = 10; // Максимальное значение множителя комбо

    void Awake()
    {
        // Проверка на существование других экземпляров ObjectPool
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
        LoadHighScore();
    }

    void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                pendingScore = 0; // Сброс накопленных очков
                comboMultiplier = 1; // Сброс множителя комбо
            }
        }
    }

    public void AddScore(int points, Vector3 position)
    {
        TMP_Text tempScoreText = GetAvailableTempScoreText();   

        tempScoreText.text = "+" + points * comboMultiplier;
        tempScoreText.transform.position = Camera.main.WorldToScreenPoint(position);
        tempScoreText.gameObject.SetActive(true);

        Animator tempScoreAnimator = tempScoreText.GetComponent<Animator>();
        // Запускаем анимацию для временного счета
        tempScoreAnimator.SetBool("Enlarge", true);

        // Запускаем корутину, которая дождется окончания анимации
        StartCoroutine(WaitForAnimationToEnd(tempScoreText, tempScoreAnimator));

        pendingScore += points * comboMultiplier; // Умножаем очки на текущий множитель
        score += pendingScore;
        UpdateScoreText();

        // Проверка на новый рекорд
        if (score > highScore)
        {
            highScore = score;
            UpdateHighScoreText();
            SaveHighScore();
        }

        comboMultiplier = Mathf.Min(comboMultiplier * 2, maxComboMultiplier); // Удвоение множителя с ограничением
        comboTimer = comboTimeLimit; // Сброс таймера комбо
    }

    private IEnumerator WaitForAnimationToEnd(TMP_Text tempScoreText, Animator tempScoreAnimator)
    {
        // Ожидание завершения анимации
        yield return new WaitWhile(() => tempScoreAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);

        Debug.Log("WaitForAnimationToEnd started");

        tempScoreAnimator.SetBool("Enlarge", false);

        // Скрываем текст после завершения анимации
        tempScoreText.gameObject.SetActive(false);
    }

    private TMP_Text GetAvailableTempScoreText()
    {
        foreach (TMP_Text text in tempScoreTextPool)
        {
            if (!text.gameObject.activeInHierarchy)
            {
                return text;
            }
        }

        // Все экземпляры активны, создаем новый
        TMP_Text newText = Instantiate(tempScoreTextPrefab, canvas.transform);
        tempScoreTextPool.Add(newText);
        return newText;
    }

   /* private void ApplyComboScore()
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
    }*/

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
