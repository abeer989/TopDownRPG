using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerStats[] playerStatsList;

    int gold = 20;

    [Header("INVENTORY SYSTEM")]
    [SerializeField] GameObject itemPrefab;
    [SerializeField] List<ItemDetailsHolder> itemsHeldDetails;
    [SerializeField] List<int> quantitiesOfItemsHeld;


    [Space]
    public bool gameMenuOpen;
    public bool switchingScenes;
    public bool dialogActive;

    public int Gold
    {
        get { return gold; }
        set { gold = value; }
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
        if (gameMenuOpen || switchingScenes || dialogActive)
            PlayerController.instance.CanMove = false;

        else
            PlayerController.instance.CanMove = true;
    }

    #region Inventory Management
    public void AddItemToInventory(ItemDetailsHolder itemToAddDetails, int itemQuantity)
    {
        // setting up local vars:
        bool itemAlreadyInInventory = false;
        int itemDetailsIndex = 0;

        if (itemsHeldDetails.Count > 0)
        {
            foreach (ItemDetailsHolder id in itemsHeldDetails)
            {
                // when the item is added, its itemDetails get passed into the func.
                // We check if the item already exists in the inventory, so we don't have to create
                // another entry for it in the itemsHeldDetails (inventory) list:
                if (id.itemName == itemToAddDetails.itemName)
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

    public void UseItemInInvetory(int charToUseOnIndex, ItemDetailsHolder itemToUseDetails, int quantityToUse)
    {
        // caching selectedCharacter using the charToUseOnIndex arg.:
        PlayerStats selectedCharacter = playerStatsList[charToUseOnIndex];

        switch (itemToUseDetails.itemType)
        {
            // applying the effect of the item depending on its type:
            case Item.ItemType.none:
                break;

            case Item.ItemType.hp_potion:
                selectedCharacter.currentHP += itemToUseDetails.itemAdditionFactor;

                if (selectedCharacter.currentHP >= selectedCharacter.maxHP)
                    selectedCharacter.currentHP = selectedCharacter.maxHP;
                break;

            case Item.ItemType.mp_potion:
                selectedCharacter.currentMP += itemToUseDetails.itemAdditionFactor;

                if (selectedCharacter.currentMP >= selectedCharacter.maxMP)
                    selectedCharacter.currentMP = selectedCharacter.maxMP;
                break;

            case Item.ItemType.weapon:
                // if a weapon is already equipped, it's going
                // to be added back to the inventory:
                if (!string.IsNullOrEmpty(selectedCharacter.equippedWeapon.itemName))
                    AddItemToInventory(itemToAddDetails: selectedCharacter.equippedWeapon, itemQuantity: 1);

                selectedCharacter.equippedWeapon = itemToUseDetails;
                selectedCharacter.weaponPower = itemToUseDetails.weaponPower;
                break;

            case Item.ItemType.armor:
                // if an armor set is already equipped, it's going
                // to be added back to the inventory:
                if (!string.IsNullOrEmpty(selectedCharacter.equippedArmor.itemName))
                    AddItemToInventory(itemToAddDetails: selectedCharacter.equippedArmor, itemQuantity: 1);

                selectedCharacter.equippedArmor = itemToUseDetails;
                selectedCharacter.armorPower = itemToUseDetails.armorPower;
                break;

            case Item.ItemType.str_buff:
                selectedCharacter.strength += itemToUseDetails.itemAdditionFactor;
                break;

            case Item.ItemType.def_buff:
                selectedCharacter.defence += itemToUseDetails.itemAdditionFactor;
                break;

            default:
                break;
        }

        UIController.instance.CloseUseForWindow(); // close the "use for?" window
        DiscardItemFromInventory(itemToDeleteDetails: itemToUseDetails, quantitityToDelete: quantityToUse, calledFromUse: true); // and then discarding it from the inventory
    }

    public void DiscardItemFromInventory(ItemDetailsHolder itemToDeleteDetails, int quantitityToDelete, bool calledFromUse = false)
    {
        if (!string.IsNullOrEmpty(itemToDeleteDetails.itemName))
        {
            int itemIndex = 0;

            // looping through the items in inventory to match the name of the itemToDelete that was passed
            // into the func. as an argument:
            foreach (ItemDetailsHolder item in itemsHeldDetails)
            {
                // when an item of the same name (unique ID) is found
                // we record the index of it in the itemsHeldDetails (inventory) array:
                if (itemToDeleteDetails.itemName == item.itemName)
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

                    if (!calledFromUse)
                    {
                        GameObject itemDropped = Instantiate(itemPrefab, PlayerController.instance.transform.position, Quaternion.identity);
                        Item itemComp = itemDropped.GetComponent<Item>();

                        if (itemComp)
                            itemComp.SetUpItem(itemToDeleteDetails);
                    }

                    // updating quanitity on the corresponding UI button too:
                    UIController.instance.DeleteCorrespondingInventoryButton(buttonToDelete: itemToDeleteDetails.itemName, quantityToDeleteOnButton: quantitiesOfItemsHeld[itemIndex]);

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