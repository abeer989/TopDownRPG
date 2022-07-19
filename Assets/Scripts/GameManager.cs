using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerStats[] playerStatsList;

    [Header("INVENTORY SYSTEM")]
    public List<string> itemsHeld;
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

    public void AddItemToInventory(Sprite itemSprite, string itemName, string itemDesc, int itemQuantity)
    {
        if (!itemsHeld.Contains(itemName))
        {
            itemsHeld.Add(itemName);
            quantitiesOfItemsHeld.Add(itemQuantity);
            UIController.instance.CreateInventoryItemButtons(buttonSprite: itemSprite,
                                                             nameOnButton: itemName,
                                                             descOnButton: itemDesc,
                                                             quantityOnButton: itemQuantity);
        }

        else
        {
            int index = itemsHeld.IndexOf(itemName);

            if (index < quantitiesOfItemsHeld.Count)
            {
                quantitiesOfItemsHeld[index] += itemQuantity;
                // send updated quantity to button:
                UIController.instance.CreateInventoryItemButtons(buttonSprite: itemSprite,
                                                                 nameOnButton: itemName,
                                                                 descOnButton: itemDesc,
                                                                 quantityOnButton: quantitiesOfItemsHeld[index]);
            }
        }
    }
}