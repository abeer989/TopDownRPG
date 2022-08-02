using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleItemButton : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemQuantity;
    ItemScriptable itemDetails;

    public void Setup(Sprite _itemSprite, string _itemName, string _itemQuantity, ItemScriptable _itemDetails)
    {
        itemImage.sprite = _itemSprite;
        itemName.SetText(_itemName);
        itemQuantity.SetText(_itemQuantity);
        itemDetails = _itemDetails;
    }

    public void OnItemButtonPressed() => BattleManager.instance.SelectItem(itemDetails: itemDetails);
}
