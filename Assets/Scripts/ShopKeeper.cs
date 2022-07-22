using UnityEngine;
using System.Collections.Generic;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] GameObject interactCanvas;
    [SerializeField] List<ItemScriptable> itemsForSale;
    //public List<int> itemQuantities;

    [Space]
    [SerializeField] string playerTag;

    bool canActivate;

    private void Update()
    {
        if (canActivate && Input.GetKeyDown(key: KeyCode.E) && PlayerController.instance.CanMove && !ShopManager.instance.ShopPanel.activeInHierarchy)
        {
            // send the items for sale that this shopkeeper has to the ShopManager:
            ShopManager.instance.GetItemsForSale(itemsFromShopKeeper: itemsForSale);
            ShopManager.instance.OpenShop();
            //ShopManager.instance.itemQuantities = itemQuantities;
            //ShopManager.instance.UpdateActiveShopkeeper(this);
        }

        else if (canActivate && Input.GetKeyDown(KeyCode.Escape) && ShopManager.instance.ShopPanel.activeInHierarchy)
            ShopManager.instance.CloseShop();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            canActivate = true;
            interactCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            canActivate = false;
            interactCanvas.SetActive(false);
        }
    }
}
