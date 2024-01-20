using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("����� ��� ����������� �����")]
    [SerializeField] private TMP_Text scoreText; // ����� �����
    [SerializeField] private TMP_Text highScoreText; // ������ ��� ���������

    [Header("������ ���������� ����� � �����, ��� ����� ��������� ��� ���")]
    [SerializeField] private TMP_Text tempScoreTextPrefab; // ������ ���������� �����
    [SerializeField] private GameObject canvasPoolLocation; // ����� ��������� ���� � �������

    [Header("������������ ���������� ��� �����")]
    [SerializeField] private int maxComboMultiplier = 10; // ������������ �������� ��������� �����

    private List<TMP_Text> tempScoreTextPool = new List<TMP_Text>(); // ��� ������ ���������� �����

    private int score = 0; // ����
    private int highScore = 0; // ������
    private int comboMultiplier = 1; // ��������� �����
    private float comboTimer = 0f; // ������ ��� �����
    private float comboTimeLimit = 2f; // ����� ��� �����

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

    public void AddScore(int points, Vector3 position) // �������� ����
    {
        TMP_Text tempScoreText = GetAvailableTempScoreText();   

        tempScoreText.text = "+" + points * comboMultiplier;
        tempScoreText.transform.position = Camera.main.WorldToScreenPoint(position);
        tempScoreText.gameObject.SetActive(true);

        Animator tempScoreAnimator = tempScoreText.GetComponent<Animator>();
        // ��������� �������� ��� ���������� �����
        tempScoreAnimator.SetBool("Enlarge", true);

        // ��������� ��������, ������� �������� ��������� ��������
        StartCoroutine(WaitForAnimationToEnd(tempScoreText, tempScoreAnimator));

        score += points * comboMultiplier;
        UpdateScoreText();

        // �������� �� ����� ������
        if (score > highScore)
        {
            highScore = score;
            UpdateHighScoreText();
            SaveHighScore();
        }

        comboMultiplier = Mathf.Min(comboMultiplier * 2, maxComboMultiplier); // �������� ��������� � ������������
        comboTimer = comboTimeLimit; // ����� ������� �����
    }

    private IEnumerator WaitForAnimationToEnd(TMP_Text tempScoreText, Animator tempScoreAnimator) // ��������� ����� ��������
    {
        // �������� ���������� ��������
        yield return new WaitWhile(() => tempScoreAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);

        tempScoreAnimator.SetBool("Enlarge", false);

        // �������� ����� ����� ���������� ��������
        tempScoreText.gameObject.SetActive(false);
    }

    private TMP_Text GetAvailableTempScoreText() // �������� ��������� ����� ���������� �����
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

    private void UpdateScoreText() // �������� ����� �����
    {
        scoreText.text = "Score: " + score;
    }

    private void UpdateHighScoreText() // �������� ����� �������
    {
        highScoreText.text = "High Score: " + highScore;
    }

    private void LoadHighScore() // ��������� ������
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreText();
    }

    private void SaveHighScore() // ��������� ������
    {
        PlayerPrefs.SetInt("HighScore", highScore);
    }
}
