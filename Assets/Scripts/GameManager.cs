using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables/Ref.
    public static GameManager instance;

    [HideInInspector] public bool gameMenuOpen, switchingScenes, dialogActive, shopActive, battleActive;

    [SerializeField] int maxNumberOfItems = 99;
    [SerializeField] int gold;
    [SerializeField] int defaultGoldAmount = 100;

    [Space]
    [SerializeField] PlayerStats[] playerStatsList;

    [Header("INVENTORY SYSTEM")]
    [SerializeField] GameObject itemPrefab;
    [SerializeField] List<ItemScriptable> itemsHeldDetails;
    [SerializeField] List<int> quantitiesOfItemsHeld;

    [Space]
    [SerializeField] List<ItemScriptable> referenceScriptables;

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
    #endregion

    #region Unity Func.
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
        if (gameMenuOpen || switchingScenes || dialogActive || shopActive || battleActive)
            PlayerController.instance.CanMove = false;

        else
            PlayerController.instance.CanMove = true;

        //if (Input.GetKeyDown(KeyCode.Q))
        //    SavePlayerData();

        //if (Input.GetKeyDown(KeyCode.L))
        //    LoadPlayerData();

        //if (Input.GetKeyDown(KeyCode.C))
        //    ClearInventoryCompletely();
    }
    #endregion

    #region Inventory Management
    /// <summary>
    /// adds an item to the inventory (e.g.: picking up and item in the world, buying from a vendor, etc.)
    /// </summary>
    /// <param name="itemToAddDetails"></param>
    /// <param name="itemQuantity"></param>
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
            UIController.instance.CreateOrUpdateCorrespondingInventoryButton(itemDetailsOnButton: itemToAddDetails,
                                                                             quantityOnButton: itemQuantity);
        }

        else
        {
            // else if the item doesn't already exist in the inventory,
            // we just increase its quantity in the quantitiesOfItemsHeld list:
            if (itemDetailsIndex < quantitiesOfItemsHeld.Count)
            {
                quantitiesOfItemsHeld[itemDetailsIndex] += itemQuantity;
                // send updated quantity to button:
                UIController.instance.CreateOrUpdateCorrespondingInventoryButton(itemDetailsOnButton: itemToAddDetails,
                                                                                 quantityOnButton: quantitiesOfItemsHeld[itemDetailsIndex]);
            }
        }
    }

    /// <summary>
    /// use item from inv. on a specified character:
    /// </summary>
    /// <param name="charToUseOnIndex"></param>
    /// <param name="itemToUseDetails"></param>
    /// <param name="quantityToUse"></param>
    public void UseItemInInvetory(int charToUseOnIndex, ItemScriptable itemToUseDetails, int quantityToUse)
    {
        AudioManager.instance.PlaySFX(sfxIndex: 7, adjust: true);

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

        if(battleActive)
            UpdateCorrespondingBattleCharacter(selectedCharacter);

        UIController.instance.CloseUseForWindow(); // close the "use for?" window
        DiscardItemFromInventory(itemToDeleteDetails: itemToUseDetails, quantitityToDelete: quantityToUse, dropItem: false); // and then discarding it from the inventory
    }

    /// <summary>
    /// discard item from inventory and delete it completely when quantity reached 0:
    /// </summary>
    /// <param name="itemToDeleteDetails"></param>
    /// <param name="quantitityToDelete"></param>
    /// <param name="dropItem"></param>
    public void DiscardItemFromInventory(ItemScriptable itemToDeleteDetails, int quantitityToDelete, bool dropItem = true)
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

                    if (dropItem)
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

    /// <summary>
    /// clear out inventory completely with their quantities by running the discard func. again & again
    /// till everything has been cleared into oblivion:
    /// </summary>
    public void ClearInventoryCompletely()
    {
        int whileBreaker = 0;

        while (itemsHeldDetails.Count > 0 && whileBreaker < 100)
        {
            DiscardItemFromInventory(itemToDeleteDetails: itemsHeldDetails[0],
                                     quantitityToDelete: quantitiesOfItemsHeld[0], dropItem: false);
            whileBreaker++;
        }
    }
    #endregion

    #region Saving/Loading
    /// <summary>
    /// save player data:
    /// 1. last active scene
    /// 2. position in world
    /// 3. everything in inventory (with quan.)
    /// </summary>
    public void SavePlayerData()
    {
        #region Saving Scene, Pos. & Gold
        // saving the current active scene & player pos.:
        PlayerPrefs.SetInt("current_scene_index", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetString("current_player_pos", PlayerController.instance.transform.position.ToString());

        // saving current amount of Gold:
        PlayerPrefs.SetInt("current_gold", gold);
        #endregion

        #region Saving Stats
        // saving player stats & if they're active:
        for (int i = 0; i < playerStatsList.Length; i++)
        {
            if (playerStatsList[i].gameObject.activeInHierarchy)
                PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_active", 1);

            else
                PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_active", 0);

            PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_level", playerStatsList[i].characterLevel);
            PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_currentEXP", playerStatsList[i].currentEXP);
            PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_currentHP", playerStatsList[i].currentHP);
            PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_maxHP", playerStatsList[i].maxHP);
            PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_currentMP", playerStatsList[i].currentMP);
            PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_maxMP", playerStatsList[i].maxMP);
            PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_str", playerStatsList[i].strength);
            PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_def", playerStatsList[i].defence);

            if (playerStatsList[i].equippedWeapon != null)
            {
                PlayerPrefs.SetString("player_" + playerStatsList[i].characterName + "_eqpd_wpn", playerStatsList[i].equippedWeapon.ItemName);
                PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_wpn_pwr", playerStatsList[i].weaponPower);
            }

            if (playerStatsList[i].equippedArmor != null)
            {
                PlayerPrefs.SetString("player_" + playerStatsList[i].characterName + "_eqpd_armr", playerStatsList[i].equippedArmor.ItemName);
                PlayerPrefs.SetInt("player_" + playerStatsList[i].characterName + "_armr_pwr", playerStatsList[i].armorPower);
            }
        }
        #endregion

        #region Saving Inventory
        // saving inventory data:

        if (PlayerPrefs.HasKey("items_held"))
        {
            int itemsHeldPrev = PlayerPrefs.GetInt("items_held");
            PlayerPrefs.DeleteKey("items_held");

            for (int i = 0; i < itemsHeldPrev; i++)
            {
                if(PlayerPrefs.HasKey("item_in_inventory_" + i) && PlayerPrefs.HasKey("amount_of_item_" + i))
                {
                    PlayerPrefs.DeleteKey("item_in_inventory_" + i);
                    PlayerPrefs.DeleteKey("amount_of_item_" + i);
                }
            }
        }

        PlayerPrefs.SetInt("items_held", itemsHeldDetails.Count);

        for (int i = 0; i < itemsHeldDetails.Count; i++)
        {
            PlayerPrefs.SetString("item_in_inventory_" + i, itemsHeldDetails[i].ItemName);
            PlayerPrefs.SetInt("amount_of_item_" + i, quantitiesOfItemsHeld[i]);
        } 
        #endregion
    }    
    
    /// <summary>
    /// load all the saved data:
    /// </summary>
    public void LoadPlayerData()
    {
        #region Loading Scene, Pos. & Gold
        // loading the last active scene & player pos.:
        int sceneToLoadIndex = 0;
        Vector3 loadedPosition = Vector3.zero;

        if (PlayerPrefs.HasKey("current_scene_index"))
            sceneToLoadIndex = PlayerPrefs.GetInt("current_scene_index");

        if (PlayerPrefs.HasKey("current_player_pos"))
        {
            string posString = PlayerPrefs.GetString("current_player_pos");
            loadedPosition = StringToVector3(posString);
        }

        SceneManager.LoadScene(sceneToLoadIndex);
        PlayerController.instance.transform.position = loadedPosition;

        // loading last saved amount of Gold:
        if (PlayerPrefs.HasKey("current_gold"))
            gold = PlayerPrefs.GetInt("current_gold");

        else
            gold = defaultGoldAmount; 
        #endregion

        #region Loading Stats
        // loading player data:
        for (int i = 0; i < playerStatsList.Length; i++)
        {
            // loading player active status:
            if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_active"))
            {
                int activeValue = PlayerPrefs.GetInt("player_" + playerStatsList[i].characterName + "_active");

                if (activeValue == 0)
                    playerStatsList[i].gameObject.SetActive(false);

                else
                    playerStatsList[i].gameObject.SetActive(true);
            }

            // loading player stats:
            if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_level"))
                playerStatsList[i].characterLevel = PlayerPrefs.GetInt("player_" + playerStatsList[i].characterName + "_level");

            if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_currentEXP"))
                playerStatsList[i].currentEXP = PlayerPrefs.GetInt("player_" + playerStatsList[i].characterName + "_currentEXP");

            if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_currentHP"))
                playerStatsList[i].currentHP = PlayerPrefs.GetInt("player_" + playerStatsList[i].characterName + "_currentHP");

            if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_maxHP"))
                playerStatsList[i].maxHP = PlayerPrefs.GetInt("player_" + playerStatsList[i].characterName + "_maxHP");

            if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_currentMP"))
                playerStatsList[i].currentMP = PlayerPrefs.GetInt("player_" + playerStatsList[i].characterName + "_currentMP");

            if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_maxMP"))
                playerStatsList[i].maxMP = PlayerPrefs.GetInt("player_" + playerStatsList[i].characterName + "_maxMP");

            if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_str"))
                playerStatsList[i].strength = PlayerPrefs.GetInt("player_" + playerStatsList[i].characterName + "_str");

            if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_def"))
                playerStatsList[i].defence = PlayerPrefs.GetInt("player_" + playerStatsList[i].characterName + "_def");

            if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_eqpd_wpn"))
            {
                string weaponName = PlayerPrefs.GetString("player_" + playerStatsList[i].characterName + "_eqpd_wpn");

                for (int j = 0; j < referenceScriptables.Count; j++)
                {
                    if (referenceScriptables[j].ItemName == weaponName)
                    {
                        playerStatsList[i].equippedWeapon = referenceScriptables[j];
                        break;
                    }
                }

                if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_wpn_pwr"))
                    playerStatsList[i].weaponPower = PlayerPrefs.GetInt("player_" + playerStatsList[i].characterName + "_wpn_pwr");
            }

            if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_eqpd_armr"))
            {
                string armorName = PlayerPrefs.GetString("player_" + playerStatsList[i].characterName + "_eqpd_armr");

                for (int j = 0; j < referenceScriptables.Count; j++)
                {
                    if (referenceScriptables[j].ItemName == armorName)
                    {
                        playerStatsList[i].equippedArmor = referenceScriptables[j];
                        break;
                    }
                }

                if (PlayerPrefs.HasKey("player_" + playerStatsList[i].characterName + "_armr_pwr"))
                    playerStatsList[i].armorPower = PlayerPrefs.GetInt("player_" + playerStatsList[i].characterName + "_armr_pwr");
            }
        }
        #endregion

        #region Loading Inventory
        if (PlayerPrefs.HasKey("items_held"))
        {
            if (itemsHeldDetails.Count > 0)
                ClearInventoryCompletely();

            int itemsHeld = PlayerPrefs.GetInt("items_held");

            if (itemsHeld > 0)
            {
                for (int i = 0; i < itemsHeld; i++)
                {
                    if (PlayerPrefs.HasKey("item_in_inventory_" + i) && PlayerPrefs.HasKey("amount_of_item_" + i))
                    {
                        string itemName = PlayerPrefs.GetString("item_in_inventory_" + i);
                        int quantity = PlayerPrefs.GetInt("amount_of_item_" + i);

                        for (int j = 0; j < referenceScriptables.Count; j++)
                        {
                            if (referenceScriptables[j].ItemName == itemName)
                            {
                                // clear out the inventory first, so the quantity doesn't keep on accumulating when the AddItemToInventory()
                                // func. is called:
                                AddItemToInventory(itemToAddDetails: referenceScriptables[j], itemQuantity: quantity);
                            }
                        }
                    }
                } 
            }
        }
        #endregion
    }
    #endregion

    #region Helpers
    /// <summary>
    /// returns an item's quantity in the inventory:
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
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

    /// <summary>
    /// a helper function that converts a properly formatted string to a Vector3:
    /// </summary>
    /// <param name="sVector"></param>
    /// <returns></returns>
    private Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            sVector = sVector.Substring(1, sVector.Length - 2);

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    /// <summary>
    /// this function sends the most updated info of the overworld player to its BattleCharacter counterpart
    /// so, health, MP, str, def, etc. all remain synced for both:
    /// </summary>
    /// <param name="playerStats"></param>
    void UpdateCorrespondingBattleCharacter(PlayerStats playerStats)
    {
        for (int i = 0; i < BattleManager.instance.ActiveBattleCharacters.Count; i++)
        {
            if (BattleManager.instance.ActiveBattleCharacters[i].CharacterType == BattleCharacter.BattleCharacterType.player)
            {
                if (playerStats.characterName == BattleManager.instance.ActiveBattleCharacters[i].CharacterName)
                    BattleManager.instance.ActiveBattleCharacters[i].SetUpBattleCharacter(playerStats);
            }
        }
    }
    #endregion
}