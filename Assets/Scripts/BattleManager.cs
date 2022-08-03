using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    #region Var./Ref.
    public static BattleManager instance;

    [SerializeField] GameObject battleScene;
    [SerializeField] GameObject actionsMenu;
    [SerializeField] BattleNotification battleNotif;

    [Header("Target Menu")]
    [SerializeField] GameObject targetAttackMenu;
    [SerializeField] Transform targetButtonsParent;
    [SerializeField] Button attackButton;
    List<Button> targetAttackButtons = new List<Button>();

    [Header("Magic Menu")]
    [SerializeField] GameObject magicMenu;
    [SerializeField] Transform magicButtonsParent;
    [SerializeField] Button magicButton;
    List<MagicButton> magicButtons = new List<MagicButton>();    
    
    [Header("Items Menu")]
    [SerializeField] GameObject itemsMenu;
    [SerializeField] Transform itemButtonsParent;
    [SerializeField] Button itemButton;
    List<BattleItemButton> itemButtons = new List<BattleItemButton>();

    [Space]
    [SerializeField] GameObject useForMenu;
    [SerializeField] Button useButton1;
    [SerializeField] Button useButton2;
    [SerializeField] Button useButton3;
    [SerializeField] List<TextMeshProUGUI> useForWindowButtonLabels;

    [Header("Flee")]
    [SerializeField] int fleeChance;

    [Space]
    [SerializeField] ShowDamageNumbers damageNumberCanvas;

    [Space]
    [Header("Texts")]
    [SerializeField] TextMeshProUGUI currentTurnText;
    [SerializeField] TextMeshProUGUI playerAttackText;
    [SerializeField] TextMeshProUGUI enemyAttackText;

    [Header("Stats UI Texts")]
    [SerializeField] List<TextMeshProUGUI> playerNameTexts;
    [SerializeField] List<TextMeshProUGUI> playerHPTexts;
    [SerializeField] List<TextMeshProUGUI> playerMPTexts;

    [Header("Prefabs")]
    [SerializeField] Button targetButtonPrefab;
    [SerializeField] MagicButton magicButtonPrefab;
    [SerializeField] BattleItemButton battleItemButtonPrefab;
    [SerializeField] List<BattleCharacter> playerPrefabs;
    [SerializeField] List<BattleCharacter> enemyPrefabs;

    [Header("Positions")]
    [SerializeField] List<Transform> playerPositions;
    [SerializeField] List<Transform> enemyPositions;

    [Space]
    [SerializeField] List<BattleMove> moveList;
    [SerializeField] List<GameObject> FX;

    List<BattleCharacter> activeBattleCharacters = new List<BattleCharacter>();

    ItemScriptable selectedItemDetails;
    MainCameraController cam;
    Vector3 cameraPos;

    int currentTurn;
    bool waitingForNextTurn;

    public List<BattleCharacter> ActiveBattleCharacters
    {
        get { return activeBattleCharacters; }
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

    private void Start()
    {
        AssignListenersToButtons();
        cam = FindObjectOfType<MainCameraController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            BattleStart(new List<string> { "skell", "spider", "eyeball" });

        if (Input.GetKeyDown(KeyCode.N))
            StartNextTurn();

        if (GameManager.instance.battleActive)
        {
            if (waitingForNextTurn)
            {
                if (activeBattleCharacters[currentTurn].CharacterType == BattleCharacter.BattleCharacterType.player)
                {
                    actionsMenu.SetActive(true);

                    //if (Input.GetKeyDown(key: KeyCode.V))
                    //    PlayerAttack("flame", 3);
                }

                else
                {
                    actionsMenu.SetActive(false);
                    StartCoroutine(EnemyAttackDelayCR());
                }
            }
        }
    }
    #endregion

    #region Battle Func.
    /// <summary>
    /// start battle at any point in the game:
    /// </summary>
    /// <param name="enemiesToSpawn"></param>
    public void BattleStart(List<string> enemiesToSpawn)
    {
        if (!GameManager.instance.battleActive)
        {
            cam = FindObjectOfType<MainCameraController>();

            if (cam != null)
            {
                // set bools and ints:
                GameManager.instance.battleActive = true;
                waitingForNextTurn = true;
                currentTurn = 0;

                // position the battle scene at wherever the camera is:
                cameraPos = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, battleScene.transform.localPosition.z);
                battleScene.transform.localPosition = cameraPos;
                battleScene.SetActive(true);
                AudioManager.instance.PlayMusic(0); // play battle music

                // spawning players at the correct positions in battle:
                for (int i = 0; i < playerPositions.Count; i++)
                {
                    if (GameManager.instance.PlayerStatsList[i].gameObject.activeInHierarchy)
                    {
                        for (int j = 0; j < playerPrefabs.Count; j++)
                        {
                            if (playerPrefabs[j].CharacterName == GameManager.instance.PlayerStatsList[i].characterName)
                            {
                                BattleCharacter newPlayer = Instantiate(playerPrefabs[i], playerPositions[i].position, playerPositions[i].rotation, playerPositions[i]);

                                PlayerStats stats = GameManager.instance.PlayerStatsList[i];
                                newPlayer.SetUpBattleCharacter(_stats: stats, _isDead: false);

                                activeBattleCharacters.Add(newPlayer);
                            }
                        }
                    }
                }

                // spawning enemies at the correct positions in battle:
                for (int i = 0; i < enemiesToSpawn.Count; i++)
                {
                    if (!string.IsNullOrEmpty(enemiesToSpawn[i]))
                    {
                        for (int j = 0; j < enemyPrefabs.Count; j++)
                        {
                            if (enemyPrefabs[j].CharacterName.Trim().ToLower().Contains(enemiesToSpawn[i]))
                                activeBattleCharacters.Add(Instantiate(enemyPrefabs[i], enemyPositions[i].position, enemyPositions[i].rotation, enemyPositions[i]));
                        }
                    }
                }

                UpdateUIStats();
                SetCurrentTurnText(activeBattleCharacters[currentTurn].CharacterName); // show whose turn it is currently on UI 
            }
        }
    }

    public void Flee()
    {
        int rand = Random.Range(0, 101);

        if (rand < fleeChance)
        {
            GameManager.instance.battleActive = false;
            battleScene.SetActive(false);
        }

        else
        {
            StartNextTurn();
            battleNotif.Activate("COULDN'T ESCAPE!");
        }
    }

    void EnemyAttack()
    {
        // locking on to target:
        List<int> playerIndices = new List<int>(); // an indices list to get the indices of all players in the activeBattleCharacters list

        activeBattleCharacters.ForEach(ab =>
        {
            // loop through the activeBattleCharacters list and adding the index of every character who's a player
            // in the playerIndices list:
            if (ab.CharacterType == BattleCharacter.BattleCharacterType.player)
                playerIndices.Add(activeBattleCharacters.IndexOf(ab));
        });

        // now that we have the player indices, we select a reandom one and take away x amount of HP from them:
        int targetIndex = playerIndices[Random.Range(0, playerIndices.Count)];

        // choose a random attack (string -- name of attack) from the activeBC's movesAvailable list:
        int attackIndex = Random.Range(0, activeBattleCharacters[currentTurn].MovesAvailable.Length);

        // cache:
        BattleMove selectedMove = null;

        // loop through the moveList and check if an attack of the same name exists in there
        if (activeBattleCharacters[currentTurn].MovesAvailable.Length > 0)
        {
            for (int i = 0; i < moveList.Count; i++)
            {
                // if it doesn, cache it in the selectedAttack var:
                if (moveList[i].MoveName.ToLower().Contains(activeBattleCharacters[currentTurn].MovesAvailable[attackIndex].Trim()))
                {
                    selectedMove = moveList[i];
                    break;
                }
            }
        }

        else
            Debug.LogError("NO MOVES AVAILABLE FOR: " + activeBattleCharacters[currentTurn].CharacterName);

        //Debug.LogError(selectedMove);
        if (selectedMove != null)
        {
            // inflict status effects from that selectedAttack:
            activeBattleCharacters[targetIndex].CurrentHP -= DealDamage(targetIndex, selectedMove.MoveDamage); // apply damage to targeted player
            activeBattleCharacters[currentTurn].CurrentMP -= selectedMove.MoveCost; // apply MP cost to the enemy itself
            Instantiate(selectedMove.AttackFX, activeBattleCharacters[targetIndex].transform.position + new Vector3(0, .7f, 0), Quaternion.identity); // instantiate attack FX on targeted player 

            string msg = activeBattleCharacters[currentTurn].CharacterName
                         + " used "
                         + selectedMove.MoveName
                         + " on "
                         + activeBattleCharacters[targetIndex].CharacterName
                         + ".";

            Instantiate(FX[0], activeBattleCharacters[currentTurn].transform.position, Quaternion.identity);
            UpdateUIStats();
            StartCoroutine(ShowAttackTextCR(text: enemyAttackText, _msg: msg, activeTime: 2));
        }
    }

    /// <summary>
    /// increment the turn counter and initiate the next turn:
    /// </summary>
    void StartNextTurn()
    {
        currentTurn++;

        if (currentTurn >= activeBattleCharacters.Count)
            currentTurn = 0;

        waitingForNextTurn = true;

        UpdateBattle();
        UpdateUIStats();
        targetAttackMenu.SetActive(false);
        attackButton.interactable = true;
        SetCurrentTurnText(activeBattleCharacters[currentTurn].CharacterName);
    }

    /// <summary>
    /// update battle func. will be called and it'll check everything regarding player and enemy status
    /// and set the battle state, accordingly. For example, if all enemies are dead, the battle will be won and vice versa:
    /// </summary>
    void UpdateBattle()
    {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (int i = 0; i < activeBattleCharacters.Count; i++)
        {
            if (activeBattleCharacters[i].CurrentHP < 0)
                activeBattleCharacters[i].CurrentHP = 0;

            if (activeBattleCharacters[i].CurrentHP == 0)
            {
                // handle dead battler
            }

            else
            {
                if (activeBattleCharacters[i].CharacterType == BattleCharacter.BattleCharacterType.player)
                    allPlayersDead = false;

                else if (activeBattleCharacters[i].CharacterType == BattleCharacter.BattleCharacterType.enemy)
                    allEnemiesDead = false;
            }
        }

        if (allEnemiesDead || allPlayersDead)
        {
            battleScene.gameObject.SetActive(false);
            GameManager.instance.battleActive = false;

            if (allPlayersDead)
            {
                // battle failure
            }

            else
            {
                // victory
            }
        }

        else
        {
            // if, for e.g.: a single char. dies in battle, their turn will be skipped over
            // and this is going to keep happening for as long as dead char.s (currenHP == 0) are met in the activeBattleCharacters list:
            while (activeBattleCharacters[currentTurn].CurrentHP == 0)
            {
                currentTurn++;

                if (currentTurn >= activeBattleCharacters.Count)
                    currentTurn = 0;
            }
        }
    }

    int DealDamage(int _targetIndex, int movePower)
    {
        float attackPower = activeBattleCharacters[currentTurn].Str + activeBattleCharacters[currentTurn].WpnPower;
        float targetDefense = activeBattleCharacters[_targetIndex].Def + activeBattleCharacters[_targetIndex].ArmrPower;
        int damage = Mathf.RoundToInt((attackPower / targetDefense) * movePower * Random.Range(.9f, 1.1f));

        Debug.Log(activeBattleCharacters[currentTurn] + " dealt " + damage + " damage to " + activeBattleCharacters[_targetIndex]);
        Instantiate(damageNumberCanvas, activeBattleCharacters[_targetIndex].transform.position, activeBattleCharacters[_targetIndex].transform.rotation).SetDamageValue(damage);
        UpdateUIStats();

        return damage;
    }

    IEnumerator PlayerAttack(string moveName, int targetIndex)
    {
        targetAttackMenu.SetActive(false);
        attackButton.interactable = false;

        yield return new WaitForSecondsRealtime(.5f);

        BattleMove selectedMove = null;
        bool enoughMana = false;

        for (int i = 0; i < moveList.Count; i++)
        {
            if (moveList[i].MoveName.ToLower().Contains(moveName.Trim()))
            {
                selectedMove = moveList[i];
                break;
            }
        }

        if (selectedMove != null)
        {
            if (activeBattleCharacters[currentTurn].CurrentMP > selectedMove.MoveCost)
            {
                enoughMana = true;

                // inflict status effects from that selectedMove:
                activeBattleCharacters[targetIndex].CurrentHP -= DealDamage(targetIndex, selectedMove.MoveDamage); // apply damage to targeted player
                activeBattleCharacters[currentTurn].CurrentMP -= selectedMove.MoveCost; // apply MP cost to the enemy itself
                Instantiate(selectedMove.AttackFX, activeBattleCharacters[targetIndex].transform.position + new Vector3(0, .7f, 0), Quaternion.identity); // instantiate attack FX on targeted player 

                string msg = activeBattleCharacters[currentTurn].CharacterName
                             + " used "
                             + selectedMove.MoveName
                             + " on "
                             + activeBattleCharacters[targetIndex].CharacterName
                             + ".";

                Instantiate(FX[0], activeBattleCharacters[currentTurn].transform.position, Quaternion.identity);
                UpdateUIStats();
                StartCoroutine(ShowAttackTextCR(text: playerAttackText, _msg: msg, activeTime: 2));
            }

            else
            {
                enoughMana = false;
                attackButton.interactable = true;
                string msg = "NOT ENOUGH MANA FOR " + selectedMove.MoveName + "!";
                battleNotif.Activate(msg);
                //StartCoroutine(ShowAttackTextCR(text: playerAttackText, _msg: msg, activeTime: 2));
            }
        }

        else
            Debug.LogError(moveName + ": MOVE NOT FOUND!");

        if (enoughMana)
            StartNextTurn();

        yield break;
    }

    IEnumerator EnemyAttackDelayCR()
    {
        waitingForNextTurn = false;
        yield return new WaitForSeconds(1);
        EnemyAttack();
        yield return new WaitForSeconds(2);
        StartNextTurn();

        yield break;
    }
    #endregion

    #region UI
    /// <summary>
    /// this function will update player stats on the battle UI after every turn.
    /// </summary>
    public void UpdateUIStats()
    {
        // loop through playerNameTexts list:
        for (int i = 0; i < playerNameTexts.Count; i++)
        {
            if (i < activeBattleCharacters.Count)
            {
                // activate a stat element when the character is a player and fill in their stats (HP, MP, name):
                if (activeBattleCharacters[i].CharacterType == BattleCharacter.BattleCharacterType.player)
                {
                    BattleCharacter playerData = activeBattleCharacters[i];

                    playerNameTexts[i].gameObject.SetActive(true);
                    playerNameTexts[i].SetText(playerData.CharacterName);
                    playerHPTexts[i].SetText(Mathf.Clamp(playerData.CurrentHP, 0, int.MaxValue) + "/" + playerData.MaxHP);
                    playerMPTexts[i].SetText(Mathf.Clamp(playerData.CurrentMP, 0, int.MaxValue) + "/" + playerData.MaxMP);

                    // to update player stats as well in our GameManager:
                    UpdateCorrespondingPlayerStats(battleCharacter: playerData);
                }

                // deactivate if its an enemy:
                else
                    playerNameTexts[i].gameObject.SetActive(false);
            }

            else
                playerNameTexts[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// open magic window:
    /// </summary>
    public void OpenMagicWindow()
    {
        magicMenu.SetActive(true);

        if (magicButtons.Count > 0)
        {
            magicButtons.ForEach(mb => Destroy(mb.gameObject));
            magicButtons.Clear();
        }

        if (activeBattleCharacters[currentTurn].CharacterType == BattleCharacter.BattleCharacterType.player)
        {
            if (activeBattleCharacters[currentTurn].MovesAvailable != null)
            {
                if (activeBattleCharacters[currentTurn].MovesAvailable.Length > 0)
                {
                    for (int i = 0; i < moveList.Count; i++)
                    {
                        for (int j = 0; j < activeBattleCharacters[currentTurn].MovesAvailable.Length; j++)
                        {
                            if (moveList[i].MoveName.ToLower().Contains(activeBattleCharacters[currentTurn].MovesAvailable[j].Trim()))
                            {
                                MagicButton magicButton = Instantiate(magicButtonPrefab, magicButtonsParent);
                                magicButton.Setup(_spellName: moveList[i].MoveName, _spellCostText: moveList[i].MoveCost.ToString() + " MP");
                                //magicButton.spellNameText.SetText(moveList[i].MoveName);
                                //magicButton.spellCostText.SetText(moveList[i].MoveCost.ToString() + " MP");

                                string spellNameArg = activeBattleCharacters[currentTurn].MovesAvailable[j].Trim();
                                magicButton.GetComponent<Button>()?.onClick.RemoveAllListeners();
                                magicButton.GetComponent<Button>()?.onClick.AddListener(() => OpenAttackWindow(attack: false, spellName: spellNameArg));
                                magicButton.GetComponent<Button>()?.onClick.AddListener(CloseMagicWindow);

                                magicButtons.Add(magicButton);
                            }
                        }
                    }
                } 
            }
        }
    }

    /// <summary>
    /// open target select window:
    /// </summary>
    public void OpenBattleItemsWindow()
    {
        itemsMenu.SetActive(true);
        PopulateBattleItemsWindow();
    }

    public void SelectItem(ItemScriptable itemDetails) => selectedItemDetails = itemDetails;

    public void ClearSelectedItem() => selectedItemDetails = null;

    public void CloseUseForWindow()
    {
        useForMenu.SetActive(false);
        ClearSelectedItem();
    }

    /// <summary>
    /// open target select window:
    /// </summary>
    void OpenAttackWindow(bool attack = true, string spellName = null)
    {
        targetAttackMenu.SetActive(true);

        if (targetAttackButtons.Count > 0)
        {
            targetAttackButtons.ForEach(tb => Destroy(tb.gameObject));
            targetAttackButtons.Clear();
        }

        // loop through activeBattleCharacters
        for (int i = 0; i < activeBattleCharacters.Count; i++)
        {
            if (activeBattleCharacters[i].CharacterType == BattleCharacter.BattleCharacterType.enemy)
            {
                // if the character on the i-th index is an enemy, instantiate a targetButton for that enemy
                // and set its text to be their name and add the PlayerAttack() func. listener:
                int index = i;
                Button targetButton = Instantiate(targetButtonPrefab, targetButtonsParent);
                targetButton.GetComponentInChildren<TextMeshProUGUI>()?.SetText(activeBattleCharacters[i].CharacterName);

                targetButton.onClick.RemoveAllListeners();

                if (attack)
                    targetButton.onClick.AddListener(() => StartCoroutine(PlayerAttack(moveName: "slash", targetIndex: index)));

                else if (!attack && !string.IsNullOrEmpty(spellName))
                    targetButton.onClick.AddListener(() => StartCoroutine(PlayerAttack(moveName: spellName, targetIndex: index)));

                targetAttackButtons.Add(targetButton);
            }
        }
    }

    void PopulateBattleItemsWindow()
    {
        if (itemButtons.Count > 0)
        {
            itemButtons.ForEach(ib => Destroy(ib.gameObject));
            itemButtons.Clear();
        }

        for (int i = 0; i < GameManager.instance.ItemsHeldDetails.Count; i++)
        {
            ItemScriptable itemData = GameManager.instance.ItemsHeldDetails[i];

            if (itemData.ItemType != Item.ItemType.armor && itemData.ItemType != Item.ItemType.weapon)
            {
                BattleItemButton battleItemButton = Instantiate(battleItemButtonPrefab, itemButtonsParent);
                battleItemButton.Setup(_itemSprite: itemData.ItemSprite, _itemName: itemData.ItemName,
                                       _itemQuantity: GameManager.instance.GetItemQuantity(itemData).ToString(),
                                       _itemDetails: itemData);

                battleItemButton.GetComponent<Button>()?.onClick.RemoveAllListeners();
                battleItemButton.GetComponent<Button>()?.onClick.AddListener(OpenUseForWindow);

                itemButtons.Add(battleItemButton);
            }
        }
    }

    void OpenUseForWindow()
    {
        if (selectedItemDetails)
        {
            useForMenu.SetActive(true);

            for (int i = 0; i < useForWindowButtonLabels.Count; i++)
            {
                // the "Use For?" window has buttons that apply the selected item's effect on the corresponding player/
                // Here, we're setting those buttons' texts to show corresponding player names:
                useForWindowButtonLabels[i].SetText(GameManager.instance.PlayerStatsList[i].characterName);
                useForWindowButtonLabels[i].transform.parent.gameObject.SetActive(GameManager.instance.PlayerStatsList[i].gameObject.activeInHierarchy);
            } 
        }
    }

    void CloseMagicWindow() => magicMenu.SetActive(false);

    void SetCurrentTurnText(string name) => currentTurnText.SetText("CURRENT TURN: " + name);

    void UseBattleItem(int _charToUseOnIndex, int _quantityToUse)
    {
        Debug.Log("called. _charToUseOnIndex: " + _charToUseOnIndex);
        GameManager.instance.UseItemInInvetory(charToUseOnIndex: _charToUseOnIndex,
                                               itemToUseDetails: selectedItemDetails, quantityToUse: _quantityToUse);

        PopulateBattleItemsWindow();
        UpdateUIStats();
    }

    void AssignListenersToButtons()
    { 
        attackButton.onClick.RemoveAllListeners();
        useButton1.onClick.RemoveAllListeners();
        useButton2.onClick.RemoveAllListeners();
        useButton3.onClick.RemoveAllListeners();

        attackButton.onClick.AddListener(() => OpenAttackWindow()); // simple slash attack  
        useButton1.onClick.AddListener(() => UseBattleItem(_charToUseOnIndex: 0, _quantityToUse: 1));
        useButton2.onClick.AddListener(() => UseBattleItem(_charToUseOnIndex: 1, _quantityToUse: 1));
        useButton3.onClick.AddListener(() => UseBattleItem(_charToUseOnIndex: 2, _quantityToUse: 1));
    }

    void UpdateCorrespondingPlayerStats(BattleCharacter battleCharacter)
    {
        for (int j = 0; j < GameManager.instance.PlayerStatsList.Length; j++)
        {
            if (battleCharacter.CharacterName == GameManager.instance.PlayerStatsList[j].characterName)
            {
                GameManager.instance.PlayerStatsList[j].currentHP = battleCharacter.CurrentHP;
                GameManager.instance.PlayerStatsList[j].currentMP = battleCharacter.CurrentMP;
                GameManager.instance.PlayerStatsList[j].maxHP = battleCharacter.MaxHP;
                GameManager.instance.PlayerStatsList[j].maxMP = battleCharacter.MaxMP;
                GameManager.instance.PlayerStatsList[j].strength = battleCharacter.Str;
                GameManager.instance.PlayerStatsList[j].defence = battleCharacter.Def;
                GameManager.instance.PlayerStatsList[j].weaponPower = battleCharacter.WpnPower;
                GameManager.instance.PlayerStatsList[j].armorPower = battleCharacter.ArmrPower;
            }
        }
    }

    IEnumerator ShowAttackTextCR(TextMeshProUGUI text, string _msg, float activeTime = 1)
    {
        text.text = _msg;

        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(activeTime);
        text.gameObject.SetActive(false);

        yield break;
    }
    #endregion
}
