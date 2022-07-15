using UnityEngine;

public class DialogActivator : MonoBehaviour
{
    [SerializeField] GameObject interactCanvas;

    [SerializeField] SpeakerType speakerType;

    [SerializeField] string playerTag;
    [SerializeField] string[] NPCLines;

    bool canActivate;

    public enum SpeakerType
    {
        none,
        npc,
        sign
    }

    private void Update()
    {
        if (canActivate && Input.GetKeyDown(key: KeyCode.E) && !DialogManager.instance.DialogBox.activeInHierarchy 
            && NPCLines.Length > 0 && interactCanvas.activeInHierarchy)
        {
            DialogManager.instance.ShowDialog(_NPCLines: NPCLines, _speakerType: speakerType);
            interactCanvas.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            canActivate = true;
            interactCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            canActivate = false;
            interactCanvas.SetActive(false);
        }
    }
}
