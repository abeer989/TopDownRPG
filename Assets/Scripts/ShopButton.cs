using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public Image ButtonImage;
    public TextMeshProUGUI ItemQuantity;
    public ItemScriptable ItemDetails;

    private void OnEnable() => GetComponent<Button>().onClick.AddListener(UIController.instance.PlayButtonSFX);

    public void OnItemButtonPressed() => ShopManager.instance.SelectItem(item: ItemDetails);
}
