using TMPro;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    [SerializeField] TMP_Text scoreText;
    [SerializeField] float speedIncreasPer100Points = 0.01f; // 0.01 increase per 100 points

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
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void AddScore(int points)
    {
        score += points;
        PlayerMovement.Instance.MoveSpeed += (points / 100) * speedIncreasPer100Points; // increase per 100 points
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
