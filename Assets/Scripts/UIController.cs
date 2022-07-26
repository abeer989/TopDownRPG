using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    #region Variables/Ref.
    public static UIController instance;

    [SerializeField] Image fadeImage;
    [SerializeField] float fadeSpeed;
    [SerializeField] GameObject menuPanel;
    [SerializeField] TextMeshProUGUI gameSavedText;
    [SerializeField] TextMeshProUGUI goldAmountText;

    [Space]
    [SerializeField] float messagTextFadeSpeed;
    [SerializeField] float messageTextActiveTime;

    [Space]
    [SerializeField] int mainMenuSceneIndex;

    [Space]
    [SerializeField] GameObject[] menuWindows;
    [SerializeField] GameObject[] characterInfoHolders;

    [Header("WINDOWS CONTENT")]
    [Header("Stats Window")]
    [SerializeField] Image statsPlayerImage;
    [SerializeField] Button[] statsButtons;
    [SerializeField] TextMeshProUGUI[] statsValueTexts; // [0] => name
                                                        // [1] => HP       
                                                        // [2] => MP
                                                        // [3] => STR
                                                        // [4] => DEF
                                                        // [5] => EQPD WPN
                                                        // [6] => WPN PWR
                                                        // [7] => EQPD ARMR
                                                        // [8] => ARMR PWR
                                                        // [9] => EXP

    [Space]
    [SerializeField] Image[] characterSprites;
    [SerializeField] TextMeshProUGUI[] nameTexts;
    [SerializeField] TextMeshProUGUI[] HPTexts;
    [SerializeField] TextMeshProUGUI[] MPTexts;
    [SerializeField] TextMeshProUGUI[] EXPTexts;
    [SerializeField] TextMeshProUGUI[] LVLTexts;
    [SerializeField] Slider[] EXPSliders;


    [Space]
    [Header("Items Window")]
    [SerializeField] Transform itemsParent;
    [SerializeField] GameObject itemButtonPrefab;

    [Space]
    [SerializeField] Button useForWindowButton;
    [SerializeField] Button useOrEquipP1Button;
    [SerializeField] Button useOrEquipP2Button;
    [SerializeField] Button useOrEquipP3Button;
    [SerializeField] Button discardButton;

    [Space]
    [SerializeField] TextMeshProUGUI returnMessageText;
    [SerializeField] TextMeshProUGUI useOrEquipButtonText;
    [SerializeField] TextMeshProUGUI itemWindowNameText;
    [SerializeField] TextMeshProUGUI itemWindowDescText;

    [Space]
    [SerializeField] GameObject useForWindow;
    [SerializeField] List<TextMeshProUGUI> useForWindowButtonLabels;

    PlayerStats[] stats;
    List<ItemButton> itemButtons;

    ItemScriptable selectedItemDetails;

    bool shouldFadeToBlack;
    bool shouldFadeFromBlack;
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

        Time.timeScale = 1;
        itemButtons = new List<ItemButton>();
    }

    private void Start()
    {
        itemWindowDescText.SetText(string.Empty);
        itemWindowNameText.SetText(string.Empty);
        returnMessageText.SetText(string.Empty);
        AssignListenersToButtons();
    } 

    private void Update()
    {
        // toggling menu:
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuPanel.activeInHierarchy && !GameManager.instance.shopActive && !GameManager.instance.dialogActive && !GameManager.instance.switchingScenes)
            {
                menuPanel.SetActive(true);
                Time.timeScale = 0;
                UpdateInfoHolderStats();
                GameManager.instance.gameMenuOpen = true;
            }

            else if (menuPanel.activeInHierarchy)
                CloseMenu();

            AudioManager.instance.PlaySFX(5); // play menu SFX
        }        
        
        // toggling the items/inventory screen directly
        // without having to open the menu first:
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!menuPanel.activeInHierarchy && !GameManager.instance.shopActive && !GameManager.instance.dialogActive && !GameManager.instance.switchingScenes)
            {
                menuPanel.SetActive(true);
                Time.timeScale = 0;
                UpdateInfoHolderStats();
                ToggleWindow(0);
                GameManager.instance.gameMenuOpen = true;
            }

            else if (menuPanel.activeInHierarchy)
                ToggleWindow(0);
        }

        // fading screen to/from black:
        if (shouldFadeToBlack)
        {
            fadeImage.color = new Color(r: fadeImage.color.r, g: fadeImage.color.g, b: fadeImage.color.b,
                                        a: Mathf.MoveTowards(fadeImage.color.a, 1, fadeSpeed * Time.deltaTime));

            if (fadeImage.color.a == 1)
                shouldFadeToBlack = false;
        }

        else if (shouldFadeFromBlack)
        {
            fadeImage.color = new Color(r: fadeImage.color.r, g: fadeImage.color.g, b: fadeImage.color.b,
                                        a: Mathf.MoveTowards(fadeImage.color.a, 0, fadeSpeed * Time.deltaTime));

            if (fadeImage.color.a == 0)
                shouldFadeFromBlack = false;
        }
    }
    #endregion

    public void MainMenu()
    {

        Destroy(PlayerController.instance.gameObject);
        PlayerController.instance = null;
        
        Destroy(GameManager.instance.gameObject);
        GameManager.instance = null;
        
        Destroy(AudioManager.instance.gameObject);
        AudioManager.instance = null;

        SceneManager.LoadScene(mainMenuSceneIndex);

        Destroy(gameObject);
        instance = null;
    }

    public void PlayButtonSFX() => AudioManager.instance.PlaySFX(4);

    /// <summary>
    /// assign functions to relevant buttons:
    /// </summary>
    void AssignListenersToButtons()
    {
        discardButton.onClick.RemoveAllListeners();
        useOrEquipP1Button.onClick.RemoveAllListeners();
        useOrEquipP2Button.onClick.RemoveAllListeners();
        useOrEquipP3Button.onClick.RemoveAllListeners();

        discardButton.onClick.AddListener(() => GameManager.instance.DiscardItemFromInventory(itemToDeleteDetails: selectedItemDetails, quantitityToDelete: 1));
        useOrEquipP1Button.onClick.AddListener(() => GameManager.instance.UseItemInInvetory(charToUseOnIndex: 0, itemToUseDetails: selectedItemDetails, quantityToUse: 1));
        useOrEquipP2Button.onClick.AddListener(() => GameManager.instance.UseItemInInvetory(charToUseOnIndex: 1, itemToUseDetails: selectedItemDetails, quantityToUse: 1));
        useOrEquipP3Button.onClick.AddListener(() => GameManager.instance.UseItemInInvetory(charToUseOnIndex: 2, itemToUseDetails: selectedItemDetails, quantityToUse: 1));
    }

    /// <summary>
    /// whenever an inventory item is created, create/update relevant buttons on the UI
    /// to reflect them properly:
    /// </summary>
    /// <param name="itemDetailsOnButton"></param>
    /// <param name="quantityOnButton"></param>
    public void CreateOrUpdateCorrespondingInventoryButton(ItemScriptable itemDetailsOnButton, int quantityOnButton)
    {
        bool buttonAlreadyInList = false;
        int buttonIndex = 0;

        // check to see if the button for the item already exists in the list here:
        if (itemButtons.Count > 0)
        {
            foreach (ItemButton itemButton in itemButtons)
            {
                if (itemButton.ItemDetails.ItemName == itemDetailsOnButton.ItemName)
                {
                    buttonAlreadyInList = true;
                    buttonIndex = itemButtons.IndexOf(itemButton);
                    break;
                }
            }
        }

        // if it doesn't, assign the relevant sprite, quantity and details of the item associated: 
        if (!buttonAlreadyInList)
        {
            GameObject button = Instantiate(itemButtonPrefab, itemsParent);
            ItemButton itemButtonComp = button.GetComponent<ItemButton>();

            if (itemButtonComp)
            {
                itemButtonComp.ButtonImage.sprite = itemDetailsOnButton.ItemSprite;
                itemButtonComp.ItemQuantity.SetText(quantityOnButton.ToString());
                itemButtonComp.ItemDetails = itemDetailsOnButton;
            }

            itemButtons.Add(itemButtonComp);
        }

        // else, just increment the quantity on the button, because it already exists in the list:
        else
            itemButtons[buttonIndex].ItemQuantity.SetText(quantityOnButton.ToString());
    }

    /// <summary>
    /// whenever an inventory item is removed, delete/update relevant buttons on the UI
    /// to reflect them properly:
    /// </summary>
    /// <param name="buttonToDelete"></param>
    /// <param name="quantityToDeleteOnButton"></param>
    public void DeleteCorrespondingInventoryButton(string buttonToDelete, int quantityToDeleteOnButton)
    {
        int buttonIndex = 0;

        // finding the right button in the itemButtons array against the
        // buttonToDelete arg. that was passed:
        foreach (ItemButton button in itemButtons)
        {
            // when the button is found, record its index: 
            if (button.ItemDetails.ItemName == buttonToDelete)
            {
                buttonIndex = itemButtons.IndexOf(button);
                break;
            }
        }

        // cache the button in a variable:
        ItemButton buttonUC = itemButtons[buttonIndex];

        if (quantityToDeleteOnButton <= 0)
        {
            // if the quantity of the corresponding inv. item <= 0
            // the button will get destroyed and removed from the itemButtons list as well:
            Destroy(buttonUC.gameObject);
            itemButtons.Remove(buttonUC);
        }

        // else if the quantity of the corresponding inv. item > 0
        // just update the quantity on the button:
        else if (quantityToDeleteOnButton > 0)
            buttonUC.ItemQuantity.SetText(quantityToDeleteOnButton.ToString());
    }

    /// <summary>
    /// update player stats and reflect them on the main menu screen (char. info. holder boxes) properly:
    /// </summary>
    void UpdateInfoHolderStats()
    {
        // copying over the playerStatsList from the GameManager which is a list of
        // the PlayerStats components attached to the diff. characters in-game: 
        stats = GameManager.instance.PlayerStatsList;
        ShowCharacterStats();

        // iterating over the copied list and setting active the corresponding
        // character info holder object in the main menu panel. E.g.: if player 1 is active, their stats
        // will be reflected within the corresponding info holder box in the main menu panel (reflecting char. info on UI):
        for (int i = 0; i < stats.Length; i++)
        {
            // filling up the info holder with character stat info:
            if (stats[i].gameObject.activeInHierarchy)
            {
                characterInfoHolders[i].SetActive(true);
                statsButtons[i].gameObject.SetActive(true);

                characterSprites[i].sprite = stats[i].playerSprite; // setting the sprite
                nameTexts[i].SetText(stats[i].characterName); // setting the name of the char.
                HPTexts[i].SetText("HP: " + stats[i].currentHP + "/" + stats[i].maxHP); // setting HP
                MPTexts[i].SetText("MP: " + stats[i].currentMP + "/" + stats[i].maxMP); // setting MP
                EXPTexts[i].SetText(stats[i].currentEXP.ToString() + "/" + stats[i].EXPLevelThresholds[stats[i].characterLevel]); // setting exp/threshold
                                                                                                                                  // (exp needed to get to next lvl)
                EXPSliders[i].value = stats[i].currentEXP; // setting the slider to reflect current EXP
                EXPSliders[i].maxValue = stats[i].EXPLevelThresholds[stats[i].characterLevel]; // and EXP needed to get to next level
                LVLTexts[i].SetText("LVL: " + stats[i].characterLevel); // setting up current level
            }

            else
            {
                characterInfoHolders[i].SetActive(false);
                statsButtons[i].gameObject.SetActive(false);
            }
        }

        // reflect gold on UI:
        goldAmountText.SetText(GameManager.instance.Gold.ToString() + "g");
    }

    /// <summary>
    /// set the selected item that is to be used or equipped from the items menu:
    /// </summary>
    /// <param name="itemDetails"></param>
    public void SelectItem(ItemScriptable itemDetails)
    {
        CloseUseForWindow();

        selectedItemDetails = itemDetails;
        itemWindowNameText.SetText(itemDetails.ItemName);
        itemWindowDescText.SetText(itemDetails.Description);

        // show the use & discard buttons only when an item is selected:
        useForWindowButton.gameObject.SetActive(true);
        discardButton.gameObject.SetActive(true);

        // if the item is a consumable, change the use button's text to "USE":
        if (selectedItemDetails.ItemType != Item.ItemType.weapon && selectedItemDetails.ItemType != Item.ItemType.armor)
            useOrEquipButtonText.SetText("USE");

        // else if the item is a weapon/armor piece, change the use button's text to "EQUIP":
        else
            useOrEquipButtonText.SetText("EQUIP");
    }

    /// <summary>
    /// clear selected item:
    /// </summary>
    public void ClearSelectedItem()
    {
        selectedItemDetails = null;
        itemWindowNameText.SetText(string.Empty);
        itemWindowDescText.SetText(string.Empty);

        // don't show the use & discard buttons when selected item is cleared:
        useForWindowButton.gameObject.SetActive(false);
        discardButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Function to update the values in the stats window.
    /// This function will update values in the stats window from the stats array:
    /// </summary>
    /// <param name="statsIndex"></param>
    public void ShowCharacterStats(int statsIndex = 0)
    {
        if (stats.Length > 0)
        {
            // updating the stats window button texts to the character names:
            for (int i = 0; i < statsButtons.Length; i++)
                statsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = stats[i].characterName;

            statsPlayerImage.sprite = stats[statsIndex].playerSprite;
            statsValueTexts[0].SetText(stats[statsIndex].characterName);
            statsValueTexts[1].SetText(stats[statsIndex].currentHP.ToString() + "/" + stats[statsIndex].maxHP.ToString());
            statsValueTexts[2].SetText(stats[statsIndex].currentMP.ToString() + "/" + stats[statsIndex].maxMP.ToString());
            statsValueTexts[3].SetText(stats[statsIndex].strength.ToString());
            statsValueTexts[4].SetText(stats[statsIndex].defence.ToString());

            {
                if (stats[statsIndex].equippedWeapon != null)
                {
                    if (!string.IsNullOrEmpty(stats[statsIndex].equippedWeapon.ItemName))
                        statsValueTexts[5].SetText(stats[statsIndex].equippedWeapon.ItemName); 
                }

                else
                    statsValueTexts[5].SetText("NONE");
            }

            {
                if (stats[statsIndex].equippedArmor != null)
                {
                    if (!string.IsNullOrEmpty(stats[statsIndex].equippedArmor.ItemName))
                        statsValueTexts[7].SetText(stats[statsIndex].equippedArmor.ItemName); 
                }

                else
                    statsValueTexts[7].SetText("NONE");
            }

            statsValueTexts[6].SetText(stats[statsIndex].weaponPower.ToString());
            statsValueTexts[8].SetText(stats[statsIndex].armorPower.ToString());
            string expNeeded = (stats[statsIndex].EXPLevelThresholds[stats[statsIndex].characterLevel] - stats[statsIndex].currentEXP).ToString();
            statsValueTexts[9].SetText(expNeeded);
        }
    }

    /// <summary>
    /// toggle b/w items and stats windows:
    /// [0] => items window
    /// [1] => stats window
    /// </summary>
    /// <param name="windowIndex"></param>
    public void ToggleWindow(int windowIndex)
    {
        UpdateInfoHolderStats();
        CloseUseForWindow();

        for (int i = 0; i < menuWindows.Length; i++)
        {
            if (i == windowIndex)
                menuWindows[i].SetActive(!menuWindows[i].activeInHierarchy);

            else
                menuWindows[i].SetActive(false);
        }
    }

    /// <summary>
    /// opens the "Use For?" window that lets you use and item on a spec. character:
    /// </summary>
    public void OpenUseForWindow()
    {
        if (selectedItemDetails)
        {
            useForWindow.SetActive(true);

            for (int i = 0; i < useForWindowButtonLabels.Count; i++)
            {
                // the "Use For?" window has buttons that apply the selected item's effect on the corresponding player/
                // Here, we're setting those buttons' texts to show corresponding player names:
                useForWindowButtonLabels[i].SetText(GameManager.instance.PlayerStatsList[i].characterName);
                useForWindowButtonLabels[i].transform.parent.gameObject.SetActive(GameManager.instance.PlayerStatsList[i].gameObject.activeInHierarchy);
            } 
        }
    }

    /// <summary>
    /// Close the "Use For?" window and clear the selected item:
    /// </summary>
    public void CloseUseForWindow()
    {
        useForWindow.SetActive(false);
        ClearSelectedItem();
    }

    /// <summary>
    /// Close main menu:
    /// </summary>
    public void CloseMenu()
    {
        foreach (GameObject window in menuWindows) window.SetActive(false);
        menuPanel.SetActive(false);
        CloseUseForWindow();
        Time.timeScale = 1;
        GameManager.instance.gameMenuOpen = false;
    }

    /// <summary>
    /// save player data, quest data and show the "Game Saved!" msg.:
    /// </summary>
    public void SaveGame()
    {
        GameManager.instance.SavePlayerData();
        QuestManager.instance.SaveQuestData();
        PlayerPrefs.Save();
        CallGameSavedMessageCR();
    }

    /// <summary>
    /// bool setter functions that enable the fade in/out func when switching b/w scenes:
    /// </summary>
    public void FadeToBlack()
    {
        shouldFadeToBlack = true;
        shouldFadeFromBlack = false;
    }

    public void FadeFromBlack()
    {
        shouldFadeFromBlack = true;
        shouldFadeToBlack = false;
    }

    /// <summary>
    /// shows a message when an a weapon is equipped and the already equipped weapon
    /// is returned to the inventory (only for weapons/armor):
    /// </summary>
    /// <param name="itemName"></param>
    public void CallShowReturnMessageCR(string itemName)
    {
        string message = "Equipped " + itemName + " has been returned to inventory.";
        StartCoroutine(ShowMessageCR(returnMessageText, message));
    }

    /// <summary>
    /// shows a message when game is saved:
    /// </summary>
    public void CallGameSavedMessageCR() => StartCoroutine(ShowMessageCR(gameSavedText));

    /// <summary>
    /// generic coroutine for fading in/out a TextMeshProUGUI text oject:
    /// </summary>
    IEnumerator ShowMessageCR(TextMeshProUGUI text, string _message = "")
    {
        if (!string.IsNullOrEmpty(_message))
            text.SetText(_message); 

        while (text.color.a < 1)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.MoveTowards(text.color.a, 1, Time.unscaledDeltaTime * messagTextFadeSpeed));
            yield return null;
        }

        while (text.color.a == 1)
        {
            yield return new WaitForSecondsRealtime(messageTextActiveTime);
            break;
        }

        while (text.color.a > 0)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.MoveTowards(text.color.a, 0, Time.unscaledDeltaTime * messagTextFadeSpeed));
            yield return null;
        }
        
        yield break;
    }
}
