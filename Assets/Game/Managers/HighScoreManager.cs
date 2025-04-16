using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;
    private int score;
    private int highScore;

    public int Score
    {
        get { return score; }
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score);
    }

    public void SetHighscore()
    {
        if (score > highScore)
        {
            highScore = score;
            Debug.Log("New Highscore: " + highScore);
        }
    }
}
