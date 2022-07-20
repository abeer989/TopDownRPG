using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public Image ButtonImage;
    public TextMeshProUGUI ItemQuantity;
    public ItemDetailsHolder ItemDetails;

    public void OnItemButtonPressed() => UIController.instance.SelectItem(itemDetails: ItemDetails);
}
