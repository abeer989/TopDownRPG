using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [SerializeField] List<Quest> quests;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
    }

    void Start()
    {
        // initialize the questMarkerComplete to have the same number of elements as
        // the questMarkerNames list:
        //questMarkerComplete = new List<bool>(new bool[questMarkerNames.Count]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Debug.Log(CheckIfQuestComplete("hello"));        
        
        if (Input.GetKeyDown(KeyCode.M))
            MarkQuestComplete("test test");        
        
        if (Input.GetKeyDown(KeyCode.K))
            MarkQuestIncomplete("test test");
    }

    /// <summary>
    /// This function will take the quest name as input and return its
    /// number in the quests list:
    /// </summary>
    /// <param name="questToGetIndexOf"></param>
    /// <returns> number of quest in the array </returns>
    public int GetQuestIndex(string questToGetIndexOf)
    {
        for (int i = 0; i < quests.Count; i++)
        {
            // iterate through the list and check every name against the passed
            // questToGetIndexOf parameter:
            if (quests[i].QuestDescription == questToGetIndexOf)
                return i;
        }

        Debug.LogError("Quest: " + questToGetIndexOf + " doesn't exist!");
        return -1; // error code
    }

    /// <summary>
    /// return quest completion status by returning its isQuestComplete bool:
    /// </summary>
    /// <param name="questToCheck"></param>
    /// <returns></returns>
    public bool CheckIfQuestComplete(string questToCheck)
    {
        if (GetQuestIndex(questToCheck) != -1)
        {
            foreach (Quest questItem in quests)
            {
                if (questItem.QuestDescription == questToCheck)
                    return questItem.isQuestComplete;
            } 
        }

        return false;
    }

    /// <summary>
    /// mark the specified quest complete/incomplete by setting its isQuestComplete true/false:
    /// </summary>
    /// <param name="questToMark"></param>
    public void MarkQuestComplete(string questToMark)
    {
        quests.ForEach(quest =>
        {
            if (quest.QuestDescription == questToMark)
            {
                quest.isQuestComplete = true;
                return;
            }
        });
    }

    public void MarkQuestIncomplete(string questToMark)
    {
        quests.ForEach(quest =>
        {
            if (quest.QuestDescription == questToMark)
            {
                quest.isQuestComplete = false;
                return;
            }
        });
    }
}
