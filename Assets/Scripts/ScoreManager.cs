using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TMP_Text scoreText; // ��������� ���� ��� ����������� �����
    public TMP_Text highScoreText; // ��������� ���� ��� ����������� �������

    public GameObject canvas; // ������ �� Canvas


    public TMP_Text tempScoreTextPrefab; // ������ ��� ���������� �����
    [SerializeField]private List<TMP_Text> tempScoreTextPool = new List<TMP_Text>();

    private int score = 0;
    private int highScore = 0;
    private int comboMultiplier = 1; // ��������� ��� �����
    private int pendingScore = 0; // ����������� ����, ��������� ���������� � �����
    private float comboTimer = 0f; // ������ ��� �����
    private float comboTimeLimit = 2f; // ����� ��� ����� (� ��������)
    [SerializeField] private int maxComboMultiplier = 10; // ������������ �������� ��������� �����

    void Awake()
    {
        // �������� �� ������������� ������ ����������� ObjectPool
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
                pendingScore = 0; // ����� ����������� �����
                comboMultiplier = 1; // ����� ��������� �����
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
        // ��������� �������� ��� ���������� �����
        tempScoreAnimator.SetBool("Enlarge", true);

        // ��������� ��������, ������� �������� ��������� ��������
        StartCoroutine(WaitForAnimationToEnd(tempScoreText, tempScoreAnimator));

        pendingScore += points * comboMultiplier; // �������� ���� �� ������� ���������
        score += pendingScore;
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

    private IEnumerator WaitForAnimationToEnd(TMP_Text tempScoreText, Animator tempScoreAnimator)
    {
        // �������� ���������� ��������
        yield return new WaitWhile(() => tempScoreAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);

        Debug.Log("WaitForAnimationToEnd started");

        tempScoreAnimator.SetBool("Enlarge", false);

        // �������� ����� ����� ���������� ��������
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

        // ��� ���������� �������, ������� �����
        TMP_Text newText = Instantiate(tempScoreTextPrefab, canvas.transform);
        tempScoreTextPool.Add(newText);
        return newText;
    }

   /* private void ApplyComboScore()
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
