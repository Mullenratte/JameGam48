using System;
using TMPro;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    [SerializeField] TMP_Text scoreText;
    public event Action<int> OnScoreChange; 
    private int score;
    private int highScore;
    private string highScoreName;


    public int Score
    {
        get { return score; }
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            LoadHighscore();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ResetScore();
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) {
            AddScore(500);
        }
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void AddScore(int points)
    {
        score += points;
        OnScoreChange?.Invoke(score);
    }

    public bool SetHighscore()
    {
        if (score > highScore)
        {
            highScore = score;
            SaveHighscore();
            return true;
        }
        return false;
    }

    public int GetHighscore()
    {
        return highScore;
    }

    public void SetHighscoreName(string name)
    {
        highScoreName = name;
    }

    public void LoadHighscore()
    {
        highScore = PlayerPrefs.GetInt("Highscore", 0);
        highScoreName = PlayerPrefs.GetString("HighscoreName", "<no name>");
    }

    public void SaveHighscore()
    {
        PlayerPrefs.SetInt("Highscore", highScore);
        PlayerPrefs.SetString("HighscoreName", highScoreName);
        PlayerPrefs.Save();
    }
}
