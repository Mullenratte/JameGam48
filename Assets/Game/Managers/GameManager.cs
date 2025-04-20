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

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void OnDestroy() {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene) {
        if (newScene == SceneManager.GetSceneByName("Game")) {
            Time.timeScale = 1;
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
