using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] AreaEntrance areaEntrance;

    PlayerController player;

    [SerializeField] string exitName;
    [SerializeField] string playerTag;
    [SerializeField] int sceneToLoadIndex;
    [SerializeField] float fadeInterval;

    private void OnEnable()
    {
        player = PlayerController.instance;
        areaEntrance.SetCorrespondingExitName(exitName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerController.instance.LastExitUsed = exitName;
            player.CanMove = false;
            player.Animator.enabled = false;
            StartCoroutine(ExitCR());
        }
    }

    IEnumerator ExitCR()
    {
        UIController.instance.FadeToBlack();
        yield return new WaitForSeconds(fadeInterval);
        SceneManager.LoadScene(sceneBuildIndex: sceneToLoadIndex);
        UIController.instance.FadeFromBlack();
        player.CanMove = true;
        player.Animator.enabled = true;
    }
}