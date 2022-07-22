using UnityEngine;

[System.Serializable]
public class Quest
{
    [SerializeField] string questDescription;
    public bool isQuestComplete;

    public string QuestDescription
    {
        get { return questDescription; }
    }
}