using UnityEngine;

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
        Debug.Log("GAME OVER");
        Time.timeScale = 0;
        HighScoreManager.Instance.SetHighscore();
    }
}
