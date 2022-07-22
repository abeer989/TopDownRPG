using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public Image ButtonImage;
    public TextMeshProUGUI ItemQuantity;
    public ItemScriptable ItemDetails;

    public void OnItemButtonPressed() => ShopManager.instance.SelectItem(item: ItemDetails);
}
