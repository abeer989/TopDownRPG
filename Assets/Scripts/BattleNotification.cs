using TMPro;
using UnityEngine;

public class BattleNotification : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI notifText;

    [SerializeField] float activeTime;

    float activeCounter;

    private void Update()
    {
        if (activeCounter > 0)
        {
            activeCounter -= Time.deltaTime;

            if (activeCounter <= 0)
                gameObject.SetActive(false);
        }
    }

    public void Activate(string msg)
    {
        notifText.SetText(msg);
        gameObject.SetActive(true);
        activeCounter = activeTime;
    }
}
