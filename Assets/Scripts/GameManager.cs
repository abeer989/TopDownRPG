using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerStats[] playerStatsList;

    [Header("INVENTORY SYSTEM")]
    public List<ItemDetailsHolder> itemsHeldDetails;
    public List<int> quantitiesOfItemsHeld;

    [Space]
    public bool gameMenuOpen;
    public bool switchingScenes;
    public bool dialogActive;

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

    private void Update()
    {
        if (gameMenuOpen || switchingScenes || dialogActive)
            PlayerController.instance.CanMove = false;

        else
            PlayerController.instance.CanMove = true;
    }

    public void AddItemToInventory(ItemDetailsHolder itemDetails, int itemQuantity)
    {
        bool itemAlreadyInInventory = false;
        int itemDetailsIndex = 0;

        if (itemsHeldDetails.Count > 0)
        {
            foreach (ItemDetailsHolder id in itemsHeldDetails)
            {
                if (id.itemName == itemDetails.itemName)
                {
                    itemAlreadyInInventory = true;
                    itemDetailsIndex = itemsHeldDetails.IndexOf(id);
                    break;
                }
            } 
        }

        if (!itemAlreadyInInventory)
        {
            itemsHeldDetails.Add(itemDetails);
            quantitiesOfItemsHeld.Add(itemQuantity);
            UIController.instance.CreateInventoryItemButtons(itemDetailsOnButton: itemDetails, quantityOnButton: itemQuantity);
        }

        else
        {
            if (itemDetailsIndex < quantitiesOfItemsHeld.Count)
            {
                quantitiesOfItemsHeld[itemDetailsIndex] += itemQuantity;
                // send updated quantity to button:
                UIController.instance.CreateInventoryItemButtons(itemDetailsOnButton: itemDetails, quantityOnButton: quantitiesOfItemsHeld[itemDetailsIndex]);
            }
        }
    }

    public void DiscardItemFromInventory (string itemToDelete, int quantitityToDelete)
    {
        if (!string.IsNullOrEmpty(itemToDelete))
        {
            int itemIndex = 0;

            foreach (ItemDetailsHolder item in itemsHeldDetails)
            {
                if (itemToDelete == item.itemName)
                {
                    itemIndex = itemsHeldDetails.IndexOf(item);
                    break;
                }
            }

            if (itemIndex < quantitiesOfItemsHeld.Count)
            {


                if (quantitiesOfItemsHeld[itemIndex] > 0)
                {
                    quantitiesOfItemsHeld[itemIndex] -= quantitityToDelete;
                    UIController.instance.DeleteInventoryItemButtons(itemToDelete, quantitiesOfItemsHeld[itemIndex]);

                    if (quantitiesOfItemsHeld[itemIndex] == 0)
                    {
                        //quantitiesOfItemsHeld[itemIndex] = 0;
                        UIController.instance.DeleteInventoryItemButtons(itemToDelete, 0);
                        itemsHeldDetails.RemoveAt(itemIndex);
                        quantitiesOfItemsHeld.RemoveAt(itemIndex);
                        return;
                    }
                }

                //UIController.instance.DeleteInventoryItemButtons(itemToDelete, quantitiesOfItemsHeld[itemIndex]);
            }
        } 
    }
}