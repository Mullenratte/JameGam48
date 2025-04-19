using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

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
        HighScoreManager.Instance.SetHighscore();
        //SceneManager.LoadSceneAsync("GameOverScene");
    }
}
