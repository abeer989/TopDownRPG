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
        yield return new WaitForSecondsRealtime(loadTime);

        if (PlayerPrefs.HasKey("current_scene_index"))
        {
            int sceneIndex = PlayerPrefs.GetInt("current_scene_index");

            SceneManager.LoadScene(sceneIndex);

            GameManager.instance.LoadPlayerData();
            QuestManager.instance.LoadQuestData();
        }
    }
}
