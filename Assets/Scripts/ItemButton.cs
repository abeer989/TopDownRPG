using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    //public int ID;
    public Image ButtonImage;
    public string ItemName;
    public string ItemDesc;
    public Item.ItemType ItemType;
    public TextMeshProUGUI ItemQuantity;

    public void OnItemButtonPressed()
    {
        UIController.instance.SelectItem(itemName: ItemName, ItemDesc);
    }
}
