using UnityEngine;

public class PlayerLights : MonoBehaviour {
    [SerializeField] Light headlight, backlight;
    float headlightBaseRange, backlightBaseRange;
    float headlightBaseOuterAngle;
    float headlightBaseIntensity;
    [SerializeField] float headlightRangeIncreasePer100;
    [SerializeField] float headlightRadiusIncreasePer100;
    [SerializeField] float headlightIntensityIncreasePer100;
    [SerializeField] float headlightMaxRange, headlightMaxRadius, headlightMaxIntensity;

    [SerializeField] float backlightRangeIncreasePer100;
    [SerializeField] float backlightMaxRange;


    private void Start() {
        HighScoreManager.Instance.OnScoreChange += HighScoreManager_OnScoreChange;

        headlightBaseRange = headlight.range;
        backlightBaseRange = backlight.range;
        headlightBaseOuterAngle = headlight.spotAngle;
        headlightBaseIntensity = headlight.intensity;
    }

    private void OnDestroy() {
        HighScoreManager.Instance.OnScoreChange -= HighScoreManager_OnScoreChange;
    }

    private void HighScoreManager_OnScoreChange(int score) {
        headlight.range = Mathf.Min(headlightBaseRange + (score / 100) * headlightRangeIncreasePer100, headlightMaxRange);
        headlight.spotAngle = Mathf.Min(headlightBaseOuterAngle + (score / 100) * headlightRadiusIncreasePer100, headlightMaxRadius);
        headlight.intensity = Mathf.Min(headlightBaseIntensity + (score / 100) * headlightIntensityIncreasePer100, headlightMaxIntensity);

        backlight.range = Mathf.Min(backlightBaseRange + (score / 100) * backlightRangeIncreasePer100, backlightMaxRange);
    }
}
