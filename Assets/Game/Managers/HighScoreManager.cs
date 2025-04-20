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


    private const string HighscoreListKey = "HighscoreList";

    public void AddToHighscoreList(string name, int score) {
        var currentList = LoadHighscoreList();

        // Add new entry
        var newList = new System.Collections.Generic.List<HighscoreEntry>(currentList);
        newList.Add(new HighscoreEntry { playerName = name, score = score });

        // Sort descending
        newList.Sort((a, b) => b.score.CompareTo(a.score));

        // Optionally limit to top 10
        if (newList.Count > 10)
            newList.RemoveRange(10, newList.Count - 10);

        // Save
        SaveHighscoreList(newList.ToArray());
    }

    public HighscoreEntry[] LoadHighscoreList() {
        string json = PlayerPrefs.GetString(HighscoreListKey, "");
        if (string.IsNullOrEmpty(json)) return new HighscoreEntry[0];

        HighscoreList loaded = JsonUtility.FromJson<HighscoreList>(json);
        return loaded.entries ?? new HighscoreEntry[0];
    }

    private void SaveHighscoreList(HighscoreEntry[] entries) {
        HighscoreList wrapper = new HighscoreList { entries = entries };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(HighscoreListKey, json);
        PlayerPrefs.Save();
    }
}

