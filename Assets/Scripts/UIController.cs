using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField] Image fadeImage;
    [SerializeField] GameObject menuPanel;

    [Space]
    [SerializeField] GameObject[] menuWindows;
    [SerializeField] GameObject[] characterInfoHolders;

    [Header("Windows Content")]
    [Header("STATS WINDOW")]
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

    [Header("ITEM WINDOW")]
    [SerializeField] Transform itemsParent;
    [SerializeField] GameObject itemButtonPrefab;
    public List<ItemButton> itemButtons;

    [Space]
    [SerializeField] float fadeSpeed;

    PlayerStats[] stats;

    bool shouldFadeToBlack;
    bool shouldFadeFromBlack;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuPanel.activeInHierarchy)
            {
                menuPanel.SetActive(true);
                Time.timeScale = 0;
                //UpdateInventoryItems();
                UpdateStats();
                GameManager.instance.gameMenuOpen = true;
            }

            else if (menuPanel.activeInHierarchy)
                CloseMenu();
        }

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


    public void CreateInventoryItemButtons(Sprite sprite, int quantity)
    {
        bool buttonAlreadyInList = false;
        int buttonIndex = 0;

        foreach (ItemButton item in itemButtons)
        {
            if (item.buttonImage.sprite == sprite)
            {
                buttonAlreadyInList = true;
                buttonIndex = itemButtons.IndexOf(item);
                break;
            }
        }

        if (!buttonAlreadyInList)
        {
            GameObject button = Instantiate(itemButtonPrefab, itemsParent);
            ItemButton itemButtonComp = button.GetComponent<ItemButton>();

            if (itemButtonComp)
            {
                itemButtonComp.buttonImage.sprite = sprite;
                itemButtonComp.itemQuantity.SetText(quantity.ToString());
            }

            itemButtons.Add(itemButtonComp);
        }

        else
            itemButtons[buttonIndex].itemQuantity.SetText(quantity.ToString());
    }

    void UpdateStats()
    {
        // copying over the playerStatsList from the GameManager which is a list of
        // the PlayerStats components attached to the diff. characters in-game: 
        stats = GameManager.instance.playerStatsList;
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
    }

    // function to update the values in the stats window.
    // This function will update values in the stats window from the stats array:
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
                if (!string.IsNullOrEmpty(stats[statsIndex].equippedWeapon))
                    statsValueTexts[5].SetText(stats[statsIndex].equippedWeapon);

                else
                    statsValueTexts[5].SetText("NONE");
            }

            {
                if (!string.IsNullOrEmpty(stats[statsIndex].equippedArmor))
                    statsValueTexts[7].SetText(stats[statsIndex].equippedArmor);

                else
                    statsValueTexts[7].SetText("NONE");
            }

            statsValueTexts[6].SetText(stats[statsIndex].weaponPower.ToString());
            statsValueTexts[8].SetText(stats[statsIndex].armorPower.ToString());
            string expNeeded = (stats[statsIndex].EXPLevelThresholds[stats[statsIndex].characterLevel] - stats[statsIndex].currentEXP).ToString();
            statsValueTexts[9].SetText(expNeeded);
        }
    }

    public void ToggleWindow(int windowIndex)
    {
        UpdateStats();

        for (int i = 0; i < menuWindows.Length; i++)
        {
            if (i == windowIndex)
                menuWindows[i].SetActive(!menuWindows[i].activeInHierarchy);

            else
                menuWindows[i].SetActive(false);
        }
    }

    public void CloseMenu()
    {
        foreach (GameObject window in menuWindows) window.SetActive(false);
        menuPanel.SetActive(false);
        Time.timeScale = 1;
        GameManager.instance.gameMenuOpen = false;
    }

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
}
