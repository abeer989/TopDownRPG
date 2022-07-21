using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Space]
    public List<ItemDetailsHolder> itemsForSale;

    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject buyWindow;
    [SerializeField] GameObject sellWindow;
    [SerializeField] TextMeshProUGUI shopGoldText;

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
    }

    void Update()
    {
        //if (Input.GetKeyDown(key: KeyCode.K) && !shopPanel.activeInHierarchy)
        //    OpenShop();
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
        shopPanel.SetActive(false);
        GameManager.instance.shopActive = false;
    }

    public void OpenBuyWindow()
    {
        buyWindow.SetActive(true);
        sellWindow.SetActive(false);
    }    
    
    public void OpenSellWindow()
    {
        sellWindow.SetActive(true);
        buyWindow.SetActive(false);
    }
}
