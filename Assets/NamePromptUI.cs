using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NamePromptUI : MonoBehaviour {
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI scoreText;

    private int finalScore;

    private void Start() {
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void OnDestroy() {
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
    }

    private void GameManager_OnGameOver(int score) {
        Show(score);
    }

    public void Show(int score) {
        finalScore = score;
        panel.SetActive(true);
        nameInputField.text = "";
        scoreText.text = "Score: " + finalScore.ToString();
        nameInputField.Select();
        nameInputField.ActivateInputField();
    }

    public void OnSubmitButton() {
        string playerName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(playerName)) {
            playerName = "Player"; // Default fallback
        }

        HighScoreManager.Instance.SetHighscoreName(playerName);
        HighScoreManager.Instance.SetHighscore();
        HighScoreManager.Instance.AddToHighscoreList(playerName, finalScore);

        SceneManager.LoadSceneAsync("MainMenu");
    }
}
