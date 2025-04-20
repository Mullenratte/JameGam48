using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenuObj;
    [SerializeField] GameObject settingsMenuObj;
    [SerializeField] GameObject highscoreMenuObj;
    public void LoadGameScene() {
        SceneManager.LoadSceneAsync("Game");
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ShowMainMenu() {
        mainMenuObj.SetActive(true);
    }

    public void HideMainMenu() {
        mainMenuObj.SetActive(false);
    }

    public void ShowSettingsMenu() {
        settingsMenuObj.SetActive(true);
    }

    public void HideSettingsMenu() {
        settingsMenuObj.SetActive(false);
    }

    public void ShowHighscoreMenu() {
        highscoreMenuObj.SetActive(true);
    }

    public void HideHighscoreMenu() {
        highscoreMenuObj.SetActive(false);
    }
}
