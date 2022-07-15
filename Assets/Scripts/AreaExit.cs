using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] AreaEntrance areaEntrance;

    [SerializeField] string exitName;
    [SerializeField] string playerTag;
    [SerializeField] int sceneToLoadIndex;

    private void OnEnable() => areaEntrance.SetCorrespondingExitName(exitName);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerController.instance.LastExitUsed = exitName;
            SceneManager.LoadScene(sceneToLoadIndex);
        }
    }
}