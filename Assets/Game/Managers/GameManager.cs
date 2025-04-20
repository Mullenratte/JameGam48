using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public event Action<int> OnGameOver;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start() {
        HighScoreManager.Instance.LoadHighscore();

    }

    public void EndGame()
    {
        Time.timeScale = 0;

        // prompt player to input name

        //string playerName = "Player123"; //  get from input field
        //int finalScore = HighScoreManager.Instance.Score;

        //HighScoreManager.Instance.SetHighscoreName(playerName);
        //HighScoreManager.Instance.SetHighscore();
        //HighScoreManager.Instance.AddToHighscoreList(playerName, finalScore);

        OnGameOver?.Invoke(HighScoreManager.Instance.Score);
        //HighScoreManager.Instance.SetHighscore();
        //SceneManager.LoadSceneAsync("MainMenu");
        //SceneManager.LoadSceneAsync("GameOverScene");
    }
}
