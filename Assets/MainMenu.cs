using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void LoadGameScene() {
        SceneManager.LoadSceneAsync("Game");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
