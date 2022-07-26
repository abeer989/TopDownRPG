using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    [Space]
    [SerializeField] float loadTime;

    private void Start() => StartCoroutine(LoadCR());

    IEnumerator LoadCR()
    {
        //yield return new WaitForSecondsRealtime(loadTime);

        if (PlayerPrefs.HasKey("current_scene_index"))
        {
            int sceneIndex = PlayerPrefs.GetInt("current_scene_index");

            SceneManager.LoadScene(sceneIndex);

            bool a() => GameManager.instance != null;
            yield return new WaitUntil(a);

            //if (GameManager.instance != null)
            //{
            Debug.Log(GameManager.instance.name);
                GameManager.instance.LoadPlayerData();
                QuestManager.instance.LoadQuestData(); 
            //}
        }

        else
            yield return new WaitForSecondsRealtime(loadTime);
    }
}
