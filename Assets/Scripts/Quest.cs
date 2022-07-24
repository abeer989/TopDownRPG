using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Quest
{
    [SerializeField] string questDescription;
    public bool isQuestComplete;

    [Space]
    public List<QuestObject> linkedObjects;

    public string QuestDescription
    {
        get { return questDescription; }
    }
}