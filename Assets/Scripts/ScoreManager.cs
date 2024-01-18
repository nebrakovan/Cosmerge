using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TMP_Text scoreText; // ��������� ���� ��� ����������� �����
    public TMP_Text highScoreText; // ��������� ���� ��� ����������� �������
    public TMP_Text tempScoreText; // ����� ��� ���������� �����

    private Animator tempScoreAnimator; // �������� ��� ���������� �����
    private int score = 0;
    private int highScore = 0;
    private int comboMultiplier = 1; // ��������� ��� �����
    private int pendingScore = 0; // ����������� ����, ��������� ���������� � �����
    private float comboTimer = 0f; // ������ ��� �����
    private float comboTimeLimit = 2f; // ����� ��� ����� (� ��������)
    [SerializeField] private int maxComboMultiplier = 10; // ������������ �������� ��������� �����

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

        // ��������� �������� ��� ���������� �����
        tempScoreAnimator.SetTrigger("AddScore");

        // ��������� ��������, ������� �������� ��������� ��������
        StartCoroutine(WaitForAnimationToEnd());

        pendingScore += points * comboMultiplier; // �������� ���� �� ������� ���������
        comboMultiplier = Mathf.Min(comboMultiplier * 2, maxComboMultiplier); // �������� ��������� � ������������
        comboTimer = comboTimeLimit; // ����� ������� �����
    }

    private IEnumerator WaitForAnimationToEnd()
    {
        // �������� ���������� ��������
        yield return new WaitWhile(() => tempScoreAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);

        // �������� ����� ����� ���������� ��������
        tempScoreText.gameObject.SetActive(false);
    }

    private void ApplyComboScore()
    {
        score += pendingScore; // ��������� ����������� ���� � �����
        UpdateScoreText();

        // �������� �� ����� ������
        if (score > highScore)
        {
            highScore = score;
            UpdateHighScoreText();
            SaveHighScore();
        }

        pendingScore = 0; // ����� ����������� �����
        comboMultiplier = 1; // ����� ��������� �����
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
