using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            HighScoreManager.Instance.LoadHighscore();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void EndGame()
    {
        Debug.Log("GAME OVER");
        Time.timeScale = 0;
        HighScoreManager.Instance.SetHighscore();
    }
}
