using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleRewardUIElement : MonoBehaviour
{
    [SerializeField] Image rewardItemImage;
    [SerializeField] TextMeshProUGUI rewardItemName;
    [SerializeField] TextMeshProUGUI rewardItemQuantity;

    public void Setup(Sprite sprite, string itemName, string quantity)
    {
        rewardItemImage.sprite = sprite;
        rewardItemName.SetText(itemName);
        rewardItemQuantity.SetText(quantity);
    }
}
