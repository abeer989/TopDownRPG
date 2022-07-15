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

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        dialogText.SetText(dialogLines[currentLineIndex]);
        currentLineIndex++;
    }

    private void Update()
    {
        if (dialogBox.activeInHierarchy)
        {
            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.E) || Input.GetMouseButtonUp(0))
            {
                if (currentLineIndex < dialogLines.Length)
                {
                    dialogText.SetText(dialogLines[currentLineIndex]);
                    currentLineIndex++;
                }

                else
                {
                    dialogBox.SetActive(false);
                    currentLineIndex = 0;
                }
            }
        }
    }
}
