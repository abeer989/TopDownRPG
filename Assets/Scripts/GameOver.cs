using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] int mainMenuSceneIndex;
    [SerializeField] int loadSceneIndex;

    private void Start()
    {
        ClearSingletons();
        AudioManager.instance.PlayMusic(4);
    }

    public void MainMenu()
    {
        ClearSingletons();
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    public void LoadLastSave()
    {
        ClearSingletons();
        SceneManager.LoadScene(loadSceneIndex);
    }

    void ClearSingletons()
    {
        if(PlayerController.instance != null)
        {
            Destroy(PlayerController.instance.gameObject);
            PlayerController.instance = null;
        }

        if (GameManager.instance != null)
        {
            Destroy(GameManager.instance.gameObject);
            GameManager.instance = null;
        }

        //if (!calledAtStart)
        //{
        //    if (AudioManager.instance != null)
        //    {
        //        Destroy(AudioManager.instance.gameObject);
        //        AudioManager.instance = null;
        //    } 
        //}

        if (BattleManager.instance != null)
        {
            Destroy(BattleManager.instance.gameObject);
            BattleManager.instance = null;
        }

        if (UIController.instance != null)
        {
            Destroy(UIController.instance.gameObject);
            UIController.instance = null;
        }
    }
}