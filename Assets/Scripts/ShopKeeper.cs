using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] GameObject interactCanvas;
    [SerializeField] List<ItemDetailsHolder> itemsForSale;

    [SerializeField] string playerTag;

    bool canActivate;

    private void Update()
    {
        if (canActivate && Input.GetKeyDown(key: KeyCode.E) && PlayerController.instance.CanMove && !ShopManager.instance.ShopPanel.activeInHierarchy)
        {
            ShopManager.instance.itemsForSale = itemsForSale;
            ShopManager.instance.OpenShop();
        }
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