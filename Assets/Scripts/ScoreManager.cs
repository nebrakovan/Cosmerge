using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Текст для отображения счета")]
    [SerializeField] private TMP_Text scoreText; // Текст счета
    [SerializeField] private TMP_Text highScoreText; // Текста для редактора

    [Header("Префаб временного счета и место, где будет храниться его пул")]
    [SerializeField] private TMP_Text tempScoreTextPrefab; // Префаб временного счета
    [SerializeField] private GameObject canvasPoolLocation; // Место храрнения пула в канвасе

    [Header("Максимальный множеитель для комбо")]
    [SerializeField] private int maxComboMultiplier = 10; // Максимальное значение множителя комбо

    private List<TMP_Text> tempScoreTextPool = new List<TMP_Text>(); // Пул текста временного счета

    private int score = 0; // Счет
    private int highScore = 0; // Рекорд
    private int comboMultiplier = 1; // Множитель комбо
    private float comboTimer = 0f; // Таймер для комбо
    private float comboTimeLimit = 2f; // Время для комбо

    void Awake()
    {
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
                comboMultiplier = 1; 
            }
        }
    }

    public void AddScore(int points, Vector3 position) // Добавить счет
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

        score += points * comboMultiplier;
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

    private IEnumerator WaitForAnimationToEnd(TMP_Text tempScoreText, Animator tempScoreAnimator) // Дождаться конца анимации
    {
        // Ожидание завершения анимации
        yield return new WaitWhile(() => tempScoreAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);

        tempScoreAnimator.SetBool("Enlarge", false);

        // Скрываем текст после завершения анимации
        tempScoreText.gameObject.SetActive(false);
    }

    private TMP_Text GetAvailableTempScoreText() // Получите доступный текст временного счета
    {
        foreach (TMP_Text text in tempScoreTextPool)
        {
            if (!text.gameObject.activeInHierarchy)
            {
                return text;
            }
        }

        TMP_Text newText = Instantiate(tempScoreTextPrefab, canvasPoolLocation.transform);
        tempScoreTextPool.Add(newText);
        return newText;
    }

    private void UpdateScoreText() // Обновить текст счета
    {
        scoreText.text = "Score: " + score;
    }

    private void UpdateHighScoreText() // Обновить текст рекорда
    {
        highScoreText.text = "High Score: " + highScore;
    }

    private void LoadHighScore() // Загрузить рекорд
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreText();
    }

    private void SaveHighScore() // Сохранить рекорд
    {
        PlayerPrefs.SetInt("HighScore", highScore);
    }
}
