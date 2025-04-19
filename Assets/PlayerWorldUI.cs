using TMPro;
using UnityEngine;

public class PlayerWorldUI : MonoBehaviour
{
    public enum ScoreQuality {
        Bad,
        Normal,
        Good,
        Awesome,
        Legendary
    }

    ScoreQuality quality;
    [SerializeField] GameObject gainedScoreUIObjectPrefab;

    private void Start() {
        PlayerTongueController.OnScoreObjectEaten += PlayerTongueController_OnScoreObjectEaten;
    }

    private void PlayerTongueController_OnScoreObjectEaten(int score) {
        GameObject spawnedObj = Instantiate(gainedScoreUIObjectPrefab, this.transform);

        if (score <= 50) {
            quality = ScoreQuality.Bad;
        } else if (score <= 100) {
            quality = ScoreQuality.Normal;
        } else if (score <= 200) {
            quality = ScoreQuality.Good;
        } else if (score <= 500) {
            quality = ScoreQuality.Awesome;
        } else {
            quality = ScoreQuality.Legendary;
        }

        TextMeshProUGUI textObj = spawnedObj.GetComponentInChildren<TextMeshProUGUI>();

        string ending = "";
        Color qualityColor = Color.white;
        float textSize = 2f;
        switch (quality) {
            case ScoreQuality.Bad:
                qualityColor = Color.gray;
                textSize = 1f;
                break;
            case ScoreQuality.Normal:
                break;
            case ScoreQuality.Good:
                ending = "!";
                qualityColor = Color.green;
                textSize = 2.6f;
                break;
            case ScoreQuality.Awesome:
                ending = "!!";
                qualityColor = Color.cyan;
                textSize = 3.8f;
                break;
            case ScoreQuality.Legendary:
                ending = "!!!";
                qualityColor = new Color(0.75f, 0.1f, 0.85f, 1f);
                textSize = 6f;
                break;
            default:
                break;
        }

        textObj.color = qualityColor;
        textObj.text = "+ " + score.ToString() + ending;
        textObj.fontSize = textSize;
    }
}
