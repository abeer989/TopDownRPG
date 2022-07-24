using UnityEngine;

public class QuestMarker : MonoBehaviour
{
    [SerializeField] string playerTag;
    [SerializeField] string questToMark;

    [Space]
    [SerializeField] bool markComplete;

    [Space]
    [SerializeField] bool markImmediately;
    [SerializeField] bool deactivateAfterMarking;

    bool canMarkOnAction;

    private void Update()
    {
        if (canMarkOnAction && Input.GetMouseButtonDown(0))
            MarkQuest();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (markImmediately)
                MarkQuest();

            else
                canMarkOnAction = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            canMarkOnAction = false;
    }

    void MarkQuest()
    {
        if (markComplete)
            QuestManager.instance.MarkQuestComplete(questToMark);

        else
            QuestManager.instance.MarkQuestIncomplete(questToMark);

        gameObject.SetActive(!deactivateAfterMarking);
    }
}
