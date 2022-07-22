using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [SerializeField] Transform buyWindowItemsParent;
    [SerializeField] Transform sellWindowItemsParent;
    [SerializeField] GameObject shopItemButtonPrefab;

    [Space]
    public List<ItemScriptable> itemsForSale;
    //public List<int> itemQuantities;

    [Space]
    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject buyWindow;
    [SerializeField] GameObject sellWindow;
    [SerializeField] TextMeshProUGUI shopGoldText;

    [Space]
    [Header("BUY WINDOW")]
    [SerializeField] TextMeshProUGUI buyItemNameText;
    [SerializeField] TextMeshProUGUI buyItemDescText;
    [SerializeField] TextMeshProUGUI buyValueText;
    [SerializeField] TextMeshProUGUI buyMessageText;
    [SerializeField] TMP_InputField buyQuantityIPField;
    [SerializeField] Button buyButton;
    [SerializeField] Button cancelBuyButton;
    List<ShopButton> buyButtons;

    [Space]
    [Header("SELL WINDOW")]
    [SerializeField] TextMeshProUGUI sellItemNameText;
    [SerializeField] TextMeshProUGUI sellItemDescText;
    [SerializeField] TextMeshProUGUI sellValueText;
    [SerializeField] TextMeshProUGUI sellMessageText;
    [SerializeField] TextMeshProUGUI noItemsText;
    [SerializeField] TMP_InputField sellQuantityIPField;
    [SerializeField] Button sellButton;
    [SerializeField] Button cancelSellButton;
    List<ShopButton> sellButtons;

    [Space]
    [SerializeField] float messageFadeSpeed;
    [SerializeField] float messageActiveTime;

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

        sellButtons = buyButtons = new List<ShopButton>();
    }

    private void Start()
    {
        buyQuantityIPField.onValueChanged.AddListener(delegate { OnQuantityChanged(inputField: buyQuantityIPField, quantityString: buyQuantityIPField.text); });
        sellQuantityIPField.onValueChanged.AddListener(delegate { OnQuantityChanged(inputField: sellQuantityIPField, quantityString: sellQuantityIPField.text, buyWindow: false); });
    }

    #region General Shop Func.
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

        else if (!buyWindow.activeInHierarchy && sellWindow.activeInHierarchy)
        {
            sellItemNameText.text = item.ItemName;
            sellItemDescText.text = item.Description;
            sellValueText.text = "Value: " + (item.SellWorth).ToString() + "g";

            ToggleSellActionWindow(state: true);
            sellQuantityIPField.text = "" + 1;
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
    #endregion


    #region Buying
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
                shopButtonComp.ItemQuantity.gameObject.SetActive(false);
            }

            buyButtons.Add(shopButtonComp);
        }
    }

    void ToggleBuyActionWindow(bool state)
    {
        buyValueText.gameObject.SetActive(state);
        buyQuantityIPField.gameObject.SetActive(state);
        buyButton.gameObject.SetActive(state);
        cancelBuyButton.gameObject.SetActive(state);
    }

    IEnumerator ShowBuyMessageCR(string itemName = "", int itemQuantity = 0, bool notEnoughGold = false)
    {
        if (itemQuantity > 0 && !string.IsNullOrEmpty(itemName) && !notEnoughGold)
        {
            if (itemQuantity == 1)
                buyMessageText.SetText(itemName + " was added to the inventory.");

            else if (itemQuantity > 1)
                buyMessageText.SetText(itemQuantity.ToString() + " " + itemName + "s were added to the inventory.");

            else
                Debug.LogError("INVALID ITEM QUANTITY!");
        }

        else
            buyMessageText.SetText("You don't have enough gold for this purchase!");

        buyButton.interactable = false;

        while (buyMessageText.color.a < 1)
        {
            buyMessageText.color = new Color(buyMessageText.color.r, buyMessageText.color.g, buyMessageText.color.b, Mathf.MoveTowards(buyMessageText.color.a, 1, Time.unscaledDeltaTime * messageFadeSpeed));
            yield return null;
        }

        while (buyMessageText.color.a == 1)
        {
            yield return new WaitForSecondsRealtime(messageActiveTime);
            break;
        }

        while (buyMessageText.color.a > 0)
        {
            buyMessageText.color = new Color(buyMessageText.color.r, buyMessageText.color.g, buyMessageText.color.b, Mathf.MoveTowards(buyMessageText.color.a, 0, Time.unscaledDeltaTime * messageFadeSpeed));
            yield return null;
        }

        buyButton.interactable = true;

        yield break;
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
        if (selectedItem != null)
        {
            int quantity = int.Parse(buyQuantityIPField.text);
            int totalPrice = selectedItem.SellWorth * 2 * quantity;

            if (totalPrice <= GameManager.instance.Gold)
            {
                GameManager.instance.Gold -= totalPrice;
                shopGoldText.SetText(GameManager.instance.Gold.ToString() + "g");
                GameManager.instance.AddItemToInventory(itemToAddDetails: selectedItem, itemQuantity: quantity);
                StartCoroutine(ShowBuyMessageCR(itemName: selectedItem.ItemName, itemQuantity: quantity));

                ToggleBuyActionWindow(state: false);
                buyItemNameText.SetText(string.Empty);
                buyItemDescText.SetText(string.Empty);
            }

            else
                StartCoroutine(ShowBuyMessageCR(notEnoughGold: true));
        }
    }

    public void CancelBuyItem()
    {
        selectedItem = null;
        ToggleBuyActionWindow(state: false);
        buyItemNameText.SetText(string.Empty);
        buyItemDescText.SetText(string.Empty);
    }
    #endregion


    #region Selling
    void PopulateSellWindow()
    {
        List<ItemScriptable> playerInventoryItems = GameManager.instance.ItemsHeldDetails;
        List<int> playerInventoryQuantities = GameManager.instance.QuantitiesOfItemsHeld;

        if (playerInventoryItems.Count <= 0)
            noItemsText.gameObject.SetActive(true);

        else
            noItemsText.gameObject.SetActive(false);

        if (sellButtons.Count > 0)
        {
            sellButtons.ForEach(s => Destroy(s.gameObject));
            sellButtons.Clear();
        }

        for (int i = 0; i < playerInventoryItems.Count; i++)
        {
            GameObject sellButton = Instantiate(shopItemButtonPrefab, sellWindowItemsParent);
            ShopButton shopButtonComp = sellButton.GetComponent<ShopButton>();

            if (shopButtonComp)
            {
                shopButtonComp.ButtonImage.sprite = playerInventoryItems[i].ItemSprite;
                shopButtonComp.ItemDetails = playerInventoryItems[i];

                if (i < playerInventoryQuantities.Count)
                    shopButtonComp.ItemQuantity.SetText(playerInventoryQuantities[i].ToString());
            }

            sellButtons.Add(shopButtonComp);
        }
    }

    void ToggleSellActionWindow(bool state)
    {
        sellValueText.gameObject.SetActive(state);
        sellQuantityIPField.gameObject.SetActive(state);
        sellButton.gameObject.SetActive(state);
        cancelSellButton.gameObject.SetActive(state);
    }

    IEnumerator ShowSellMessageCR(string itemName = "", int itemQuantity = 0, bool sellQuantityInvalid = false)
    {
        if (itemQuantity > 0 && !string.IsNullOrEmpty(itemName) && !sellQuantityInvalid)
        {
            if (itemQuantity == 1)
                sellMessageText.SetText(itemName + " was sold.");

            else if (itemQuantity > 1)
                sellMessageText.SetText(itemQuantity.ToString() + " " + itemName + "s were sold.");

            else
                Debug.LogError("SELL QUANTITY INVALID!");
        }

        else
            sellMessageText.SetText("Sell quantity can't be greater than item quantity in inventory!");

        sellButton.interactable = false;

        while (sellMessageText.color.a < 1)
        {
            sellMessageText.color = new Color(sellMessageText.color.r, sellMessageText.color.g, sellMessageText.color.b, Mathf.MoveTowards(sellMessageText.color.a, 1, Time.unscaledDeltaTime * messageFadeSpeed));
            yield return null;
        }

        while (sellMessageText.color.a == 1)
        {
            yield return new WaitForSecondsRealtime(messageActiveTime);
            break;
        }

        while (sellMessageText.color.a > 0)
        {
            sellMessageText.color = new Color(sellMessageText.color.r, sellMessageText.color.g, sellMessageText.color.b, Mathf.MoveTowards(sellMessageText.color.a, 0, Time.unscaledDeltaTime * messageFadeSpeed));
            yield return null;
        }

        sellButton.interactable = true;

        yield break;
    }

    public void OpenSellWindow()
    {
        selectedItem = null;

        PopulateSellWindow();
        sellWindow.SetActive(true);
        sellItemNameText.text = string.Empty;
        sellItemDescText.text = string.Empty;
        ToggleSellActionWindow(state: false);

        buyWindow.SetActive(false);
    }

    public void SellItem()
    {
        if (selectedItem != null)
        {
            int sellQuantity = int.Parse(sellQuantityIPField.text);
            int totalPrice = selectedItem.SellWorth * sellQuantity;

            int itemIndex = 0;

            foreach (ItemScriptable item in GameManager.instance.ItemsHeldDetails)
            {
                if (item.ItemName == selectedItem.ItemName)
                {
                    itemIndex = GameManager.instance.ItemsHeldDetails.IndexOf(item);
                    break;
                }
            }

            int itemQuantityInInventory = GameManager.instance.QuantitiesOfItemsHeld[itemIndex];

            if (sellQuantity <= itemQuantityInInventory)
            {
                GameManager.instance.Gold += totalPrice;
                shopGoldText.SetText(GameManager.instance.Gold.ToString() + "g");
                GameManager.instance.DiscardItemFromInventory(itemToDeleteDetails: selectedItem, quantitityToDelete: sellQuantity, calledFromUseOrShop: true);
                StartCoroutine(ShowSellMessageCR(itemName: selectedItem.ItemName, itemQuantity: sellQuantity));

                ToggleSellActionWindow(state: false);
                sellItemNameText.SetText(string.Empty);
                sellItemDescText.SetText(string.Empty);

                PopulateSellWindow();
            }

            else
                StartCoroutine(ShowSellMessageCR(sellQuantityInvalid: true));
        }
    }

    public void CancelSellItem()
    {
        selectedItem = null;
        ToggleSellActionWindow(state: false);
        sellItemNameText.SetText(string.Empty);
        sellItemDescText.SetText(string.Empty);
    }
    #endregion

    #region Helper Func.
    void OnQuantityChanged(TMP_InputField inputField, string quantityString, bool buyWindow = true)
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

        if (buyWindow)
        {
            int totalValue = quantity * selectedItem.SellWorth * 2;
            buyValueText.SetText("Value: " + totalValue + "g"); 
        }

        else
        {
            int totalValue = quantity * selectedItem.SellWorth;
            sellValueText.SetText("Value: " + totalValue + "g");
        }
    }
    #endregion
}
