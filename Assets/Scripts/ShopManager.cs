using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private const bool State = false;
    public static ShopManager instance;

    [SerializeField] Transform buyWindowItemsParent;
    [SerializeField] Transform sellWindowItemsParent;
    [SerializeField] GameObject shopItemButtonPrefab;

    [Space]
    public List<ItemScriptable> itemsForSale;
    public List<int> itemQuantities;

    [Space]
    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject buyWindow;
    [SerializeField] GameObject sellWindow;
    [SerializeField] TextMeshProUGUI shopGoldText;

    [Space]
    [Header("BUY")]
    [SerializeField] TextMeshProUGUI buyItemNameText;
    [SerializeField] TextMeshProUGUI buyItemDescText;
    [SerializeField] TextMeshProUGUI buyValueText;
    [SerializeField] TMP_InputField buyQuantityIPField;
    [SerializeField] Button buyButton;
    List<ShopButton> buyButtons;
    
    [Space]
    [Header("SELL")]
    [SerializeField] TextMeshProUGUI sellItemNameText;
    [SerializeField] TextMeshProUGUI sellItemDescText;
    [SerializeField] TextMeshProUGUI sellValueText;
    [SerializeField] Button sellButton;
    List<ShopButton> sellButtons;

    ItemScriptable selectedItem;

    public GameObject ShopPanel
    {
        get { return shopPanel; }
    }

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

        buyButtons = new List<ShopButton>();
    }

    private void Start()
    {
        buyQuantityIPField.onValueChanged.AddListener(delegate { OnBuyQuantityChanged(buyQuantityIPField, buyQuantityIPField.text); });
    }

    void OnBuyQuantityChanged(TMP_InputField inputField, string quantityString)
    {
        int quantity = 0;
         
        if (!string.IsNullOrEmpty(quantityString))
        {
            quantity = int.Parse(quantityString);

            if (quantity == 0)
            {
                quantity = 1;
                inputField.text = quantity.ToString();
            }
        }

        else if (string.IsNullOrEmpty(quantityString))
        {
            quantity = 1;
            inputField.text = quantity.ToString();
        }

        int totalValue = quantity * selectedItem.SellWorth * 2;
        buyValueText.SetText("Value: " + totalValue + "g");
    }

    void PopulateBuyWindow()
    {
        if (buyButtons.Count > 0)
        {
            buyButtons.ForEach(b => Destroy(b.gameObject));
            buyButtons.Clear(); 
        }

        for (int i = 0; i < itemsForSale.Count; i++)
        {
            GameObject buyButton = Instantiate(shopItemButtonPrefab, buyWindowItemsParent);
            ShopButton shopButtonComp = buyButton.GetComponent<ShopButton>();

            if (shopButtonComp)
            {
                shopButtonComp.ButtonImage.sprite = itemsForSale[i].ItemSprite;
                shopButtonComp.ItemDetails = itemsForSale[i];
            }

            buyButtons.Add(shopButtonComp);
        }
    }

    void ToggleBuyActionWindow(bool state)
    {
        buyValueText.gameObject.SetActive(state);
        buyQuantityIPField.gameObject.SetActive(state);
        buyButton.gameObject.SetActive(state);
    }

    public void SelectItem(ItemScriptable item)
    {
        selectedItem = item;

        if (buyWindow.activeInHierarchy && !sellWindow.activeInHierarchy)
        {
            buyItemNameText.text = item.ItemName;
            buyItemDescText.text = item.Description;
            buyValueText.text = "Value: " + (item.SellWorth * 2).ToString() + "g";

            ToggleBuyActionWindow(state: true);
            buyQuantityIPField.text = "" + 1;
        }

        else
        {
            sellItemNameText.text = item.ItemName;
            sellItemDescText.text = item.Description;
        }
    }

    public void OpenShop()
    {
        shopGoldText.SetText(GameManager.instance.Gold.ToString() + "g");
        shopPanel.SetActive(true);
        OpenBuyWindow();
        GameManager.instance.shopActive = true;
    }

    public void CloseShop()
    {
        selectedItem = null;
        shopPanel.SetActive(false);
        GameManager.instance.shopActive = false;
    }

    public void OpenBuyWindow()
    {
        selectedItem = null;

        PopulateBuyWindow();
        buyWindow.SetActive(true);
        buyItemNameText.text = string.Empty;
        buyItemDescText.text = string.Empty;
        ToggleBuyActionWindow(state: false);

        sellWindow.SetActive(false);
    }

    public void BuyItem()
    {
        int quantity = int.Parse(buyQuantityIPField.text);
        int totalPrice = selectedItem.SellWorth * 2 * quantity;

        if (selectedItem && totalPrice <= GameManager.instance.Gold)
        {
            GameManager.instance.Gold -= totalPrice;
            shopGoldText.SetText(GameManager.instance.Gold.ToString() + "g");
            GameManager.instance.AddItemToInventory(itemToAddDetails: selectedItem, itemQuantity: quantity);

            ToggleBuyActionWindow(state: false);
        }
    }

    public void OpenSellWindow()
    {
        selectedItem = null;
        sellWindow.SetActive(true);
        buyWindow.SetActive(false);
    }
}
