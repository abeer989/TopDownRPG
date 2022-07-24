using UnityEngine;

public class DialogActivator : MonoBehaviour
{
    [SerializeField] GameObject interactCanvas;

    [SerializeField] SpeakerType speakerType;

    [SerializeField] string playerTag;
    [SerializeField] string[] NPCLines;

    [Header("Quest Marking")]
    [SerializeField] bool shouldMarkQuest;

    [Space]
    [SerializeField] string questName;
    [SerializeField] bool markComplete;

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
            DialogManager.instance.MarkQuestAtEndOfDialog(_questName: questName, _shouldMarkQuest: shouldMarkQuest, _markComplete: markComplete);
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
