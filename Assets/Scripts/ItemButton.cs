using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public Image ButtonImage;
    public TextMeshProUGUI ItemQuantity;
    public ItemScriptable ItemDetails;

    private void OnEnable() => GetComponent<Button>().onClick.AddListener(UIController.instance.PlayButtonSFX);

    public void OnItemButtonPressed() => UIController.instance.SelectItem(itemDetails: ItemDetails);
}
