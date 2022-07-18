using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerStats[] playerStatsList;

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

    public void AddItemToInventory(Sprite _sprite, string item, int itemQuantity)
    {
        if (!itemsHeld.Contains(item))
        {
            itemsHeld.Add(item);
            quantitiesOfItemsHeld.Add(itemQuantity);
            UIController.instance.CreateInventoryItemButtons(_sprite, itemQuantity);
        }

        else
        {
            int index = itemsHeld.IndexOf(item);
            if (index < quantitiesOfItemsHeld.Count)
            {
                quantitiesOfItemsHeld[index] += itemQuantity;
                UIController.instance.CreateInventoryItemButtons(_sprite, quantitiesOfItemsHeld[index]);
            }
        }
    }
}