using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [HideInInspector] public bool gameMenuOpen, switchingScenes, dialogActive, shopActive;

    [SerializeField] int maxNumberOfItems = 99;
    [SerializeField] int gold = 100;

    [Space]
    [SerializeField] PlayerStats[] playerStatsList;

    [Header("INVENTORY SYSTEM")]
    [SerializeField] GameObject itemPrefab;
    [SerializeField] List<ItemScriptable> itemsHeldDetails;
    [SerializeField] List<int> quantitiesOfItemsHeld;

    public int Gold
    {
        get { return gold; }
        set { gold = value; }
    }        
    
    public int MaxNumberOfItems
    {
        get { return maxNumberOfItems; }
    }    
    
    public List<ItemScriptable> ItemsHeldDetails
    {
        get { return itemsHeldDetails; }
    }    
    
    public List<int> QuantitiesOfItemsHeld
    {
        get { return quantitiesOfItemsHeld; }
    }    
    
    public PlayerStats[] PlayerStatsList
    {
        get { return playerStatsList; }
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

    private void Update()
    {
        if (gameMenuOpen || switchingScenes || dialogActive || shopActive)
            PlayerController.instance.CanMove = false;

        else
            PlayerController.instance.CanMove = true;
    }

    #region Inventory Management
    public int GetItemQuantity(ItemScriptable item)
    {
        int index = 0;
        int itemQuantityInInventory = 0;

        foreach (ItemScriptable i in itemsHeldDetails)
        {
            if (i.ItemName == item.ItemName)
            {
                index = itemsHeldDetails.IndexOf(i);
                break;
            }
        }

        if (index < quantitiesOfItemsHeld.Count)
            itemQuantityInInventory = quantitiesOfItemsHeld[index];

        return itemQuantityInInventory;
    }

    public void AddItemToInventory(ItemScriptable itemToAddDetails, int itemQuantity)
    {
        // setting up local vars:
        bool itemAlreadyInInventory = false;
        int itemDetailsIndex = 0;

        if (itemsHeldDetails.Count > 0)
        {
            foreach (ItemScriptable id in itemsHeldDetails)
            {
                // when the item is added, its itemDetails get passed into the func.
                // We check if the item already exists in the inventory, so we don't have to create
                // another entry for it in the itemsHeldDetails (inventory) list:
                if (id.ItemName == itemToAddDetails.ItemName)
                {
                    itemAlreadyInInventory = true;
                    itemDetailsIndex = itemsHeldDetails.IndexOf(id); // if it exixts, we cache its index in the itemsHeldDetails list
                    break;
                }
            }
        }

        // if the item doesn't already exist in the list, we add it to the list,
        // we add the quantity that was also passed as an arg. in the quantitiesOfItemsHeld list at the same index
        // and finally, create a UI button to reflect its presence in the inv.:
        if (!itemAlreadyInInventory)
        {
            itemsHeldDetails.Add(itemToAddDetails);
            quantitiesOfItemsHeld.Add(itemQuantity);
            UIController.instance.CreateOrUpdateCorrespondingInventoryButton(itemDetailsOnButton: itemToAddDetails, quantityOnButton: itemQuantity);
        }

        else
        {
            // else if the item doesn't already exist in the inventory,
            // we just increase its quantity in the quantitiesOfItemsHeld list:
            if (itemDetailsIndex < quantitiesOfItemsHeld.Count)
            {
                quantitiesOfItemsHeld[itemDetailsIndex] += itemQuantity;
                // send updated quantity to button:
                UIController.instance.CreateOrUpdateCorrespondingInventoryButton(itemDetailsOnButton: itemToAddDetails, quantityOnButton: quantitiesOfItemsHeld[itemDetailsIndex]);
            }
        }
    }

    public void UseItemInInvetory(int charToUseOnIndex, ItemScriptable itemToUseDetails, int quantityToUse)
    {
        // caching selectedCharacter using the charToUseOnIndex arg.:
        PlayerStats selectedCharacter = playerStatsList[charToUseOnIndex];

        switch (itemToUseDetails.ItemType)
        {
            // applying the effect of the item depending on its type:
            case Item.ItemType.none:
                break;

            case Item.ItemType.hp_potion:
                selectedCharacter.currentHP += itemToUseDetails.ItemAdditionFactor;

                if (selectedCharacter.currentHP >= selectedCharacter.maxHP)
                    selectedCharacter.currentHP = selectedCharacter.maxHP;
                break;

            case Item.ItemType.mp_potion:
                selectedCharacter.currentMP += itemToUseDetails.ItemAdditionFactor;

                if (selectedCharacter.currentMP >= selectedCharacter.maxMP)
                    selectedCharacter.currentMP = selectedCharacter.maxMP;
                break;

            case Item.ItemType.weapon:
                // if a weapon is already equipped, it's going
                // to be added back to the inventory:
                if (selectedCharacter.equippedWeapon != null)
                {
                    if (!string.IsNullOrEmpty(selectedCharacter.equippedWeapon.ItemName))
                    {
                        AddItemToInventory(itemToAddDetails: selectedCharacter.equippedWeapon, itemQuantity: 1);
                        UIController.instance.CallShowReturnMessageCR(selectedCharacter.equippedWeapon.ItemName);
                    }
                }

                selectedCharacter.equippedWeapon = itemToUseDetails;
                selectedCharacter.weaponPower = itemToUseDetails.WeaponPower;
                break;

            case Item.ItemType.armor:
                // if an armor set is already equipped, it's going
                // to be added back to the inventory:
                if (selectedCharacter.equippedArmor != null)
                {
                    if (!string.IsNullOrEmpty(selectedCharacter.equippedArmor.ItemName))
                    {
                        AddItemToInventory(itemToAddDetails: selectedCharacter.equippedArmor, itemQuantity: 1);
                        UIController.instance.CallShowReturnMessageCR(selectedCharacter.equippedArmor.ItemName);
                    }
                }

                selectedCharacter.equippedArmor = itemToUseDetails;
                selectedCharacter.armorPower = itemToUseDetails.ArmorPower;
                break;

            case Item.ItemType.str_buff:
                selectedCharacter.strength += itemToUseDetails.ItemAdditionFactor;
                break;

            case Item.ItemType.def_buff:
                selectedCharacter.defence += itemToUseDetails.ItemAdditionFactor;
                break;

            default:
                break;
        }

        UIController.instance.CloseUseForWindow(); // close the "use for?" window
        DiscardItemFromInventory(itemToDeleteDetails: itemToUseDetails, quantitityToDelete: quantityToUse, calledFromUseOrShop: true); // and then discarding it from the inventory
    }

    public void DiscardItemFromInventory(ItemScriptable itemToDeleteDetails, int quantitityToDelete, bool calledFromUseOrShop = false)
    {
        if (!string.IsNullOrEmpty(itemToDeleteDetails.ItemName))
        {
            int itemIndex = 0;

            // looping through the items in inventory to match the name of the itemToDelete that was passed
            // into the func. as an argument:
            foreach (ItemScriptable item in itemsHeldDetails)
            {
                // when an item of the same name (unique ID) is found
                // we record the index of it in the itemsHeldDetails (inventory) array:
                if (itemToDeleteDetails.ItemName == item.ItemName)
                {
                    itemIndex = itemsHeldDetails.IndexOf(item);
                    break;
                }
            }

            // now coming the quantitiesOfItemsHeld array:
            if (itemIndex < quantitiesOfItemsHeld.Count)
            {
                if (quantitiesOfItemsHeld[itemIndex] > 0)
                {
                    // decresing the quanitity of the item in inv.
                    // if the quantity is alread greater than 0:
                    quantitiesOfItemsHeld[itemIndex] -= quantitityToDelete;

                    if (!calledFromUseOrShop)
                    {
                        GameObject itemDropped = Instantiate(itemPrefab, PlayerController.instance.transform.position, Quaternion.identity);
                        Item itemComp = itemDropped.GetComponent<Item>();

                        if (itemComp)
                            itemComp.SetUpItem(itemToDeleteDetails);
                    }

                    // updating quanitity on the corresponding UI button too:
                    UIController.instance.DeleteCorrespondingInventoryButton(buttonToDelete: itemToDeleteDetails.ItemName, quantityToDeleteOnButton: quantitiesOfItemsHeld[itemIndex]);

                    // if the quantity <= 0
                    if (quantitiesOfItemsHeld[itemIndex] <= 0)
                    {
                        quantitiesOfItemsHeld[itemIndex] = 0; // set to 0 to avoid errors
                        itemsHeldDetails.RemoveAt(itemIndex); // remove item from inventory
                        quantitiesOfItemsHeld.RemoveAt(itemIndex); // remove its quantitiy entry completely
                        UIController.instance.ClearSelectedItem(); // clear the selected item in the UIController
                        return;
                    }
                }
            }
        }
    }
    #endregion
}