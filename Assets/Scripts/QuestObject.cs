using UnityEngine;

public class QuestObject : MonoBehaviour
{
    [SerializeField] bool stateAfterQuestCompletion;

    public bool StateAfterQuestCompletion
    {
        get { return stateAfterQuestCompletion; }
    }
}
