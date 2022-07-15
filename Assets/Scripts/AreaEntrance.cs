using UnityEngine;

public class AreaEntrance : MonoBehaviour
{
    string correspondingExitName = ".";

    private void Start()
    {
        if (!string.IsNullOrEmpty(PlayerController.instance.LastExitUsed))
        {
            if (PlayerController.instance.LastExitUsed == correspondingExitName)
                PlayerController.instance.transform.position = transform.position; 
        }
    }

    public void SetCorrespondingExitName(string name) => correspondingExitName = name;
}
