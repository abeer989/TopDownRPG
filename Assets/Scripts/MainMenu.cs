using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject continueButton;

    [Space]
    [SerializeField] int newGameSceneIndex;
    [SerializeField] int loadSceneIndex;

    void Start()
    {
        if (PlayerPrefs.HasKey("current_scene_index"))
            continueButton.SetActive(true);

        else
            continueButton.SetActive(false);
    }

    public void NewGame() => SceneManager.LoadScene(newGameSceneIndex);

    public void ContinueGame() => SceneManager.LoadScene(loadSceneIndex);

    public void ExitGame() => Application.Quit();
}
