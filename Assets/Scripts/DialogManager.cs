using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    [SerializeField] GameObject nameBox;
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI dialogText;

    [SerializeField] string[] dialogLines;

    int currentLineIndex = 0;

    public GameObject DialogBox
    {
        get { return dialogBox; }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        if (dialogBox.activeInHierarchy)
        {
            // locking player movement while the dialog box is active:
            GameManager.instance.dialogActive = true;
            PlayerController.instance.Animator.enabled = false;

            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.E) || Input.GetMouseButtonUp(0))
            {
                if (currentLineIndex < dialogLines.Length)
                {
                    CheckIfName();
                    dialogText.SetText(dialogLines[currentLineIndex]);
                    currentLineIndex++;
                }

                else
                {
                    dialogBox.SetActive(false);
                    GameManager.instance.dialogActive = false;
                    PlayerController.instance.Animator.enabled = true;
                    currentLineIndex = 0;
                }
            }
        }
    }

    public void ShowDialog(string[] _NPCLines, DialogActivator.SpeakerType _speakerType)
    {
        switch (_speakerType)
        {
            case DialogActivator.SpeakerType.none:
                break;

            case DialogActivator.SpeakerType.npc:
                nameBox.SetActive(true);
                break;

            case DialogActivator.SpeakerType.sign:
                nameBox.SetActive(false);
                break;

            default:
                break;
        }

        dialogLines = _NPCLines;
        currentLineIndex = 0;
        CheckIfName();
        dialogText.SetText(dialogLines[currentLineIndex]);
        dialogBox.SetActive(true);
    }

    // this func. checks if the string on the current index is a name key (e.g.: n-Nina, n-George, n-Player).
    // If it is, it changes the name text to the NPC/player name and increments the currentLineIndex, so that the
    // actual dialog gets displayed in the box:
    void CheckIfName()
    {
        if (dialogLines[currentLineIndex].StartsWith("n-"))
        {
            nameText.SetText(dialogLines[currentLineIndex].Replace("n-", ""));
            currentLineIndex++;
        }
    }
}
