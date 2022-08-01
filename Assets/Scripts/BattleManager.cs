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
    [SerializeField] List<BattleCharacter> playerPrefabs;
    [SerializeField] List<BattleCharacter> enemyPrefabs;

    [Header("Positions")]
    [SerializeField] List<Transform> playerPositions;
    [SerializeField] List<Transform> enemyPositions;

    [Space]
    [SerializeField] List<BattleMove> moveList;
    [SerializeField] List<GameObject> FX;

    List<BattleCharacter> activeBattleCharacters = new List<BattleCharacter>();

    MainCameraController cam;
    Vector3 cameraPos;

    int currentTurn;
    bool waitingForNextTurn;
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

    private void OnEnable() => cam = FindObjectOfType<MainCameraController>();

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

                    if (Input.GetKeyDown(key: KeyCode.V))
                        PlayerAttack("flame", 3);
                }

                else
                {
                    actionsMenu.SetActive(false);

                    // enemy attack:
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

    /// <summary>
    /// player attack function:
    /// </summary>
    /// <param name="moveName"></param>
    /// <param name="targetIndex"></param>
    IEnumerator PlayerAttack(string moveName, int targetIndex)
    {
        targetAttackMenu.SetActive(false);
        attackButton.interactable = false;

        yield return new WaitForSecondsRealtime(.5f);

        BattleMove selectedMove = null;

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

        StartNextTurn();

        yield break;
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
    /// open target select window:
    /// </summary>
    public void OpenAttackWindow()
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
                targetButton.onClick.AddListener(() => StartCoroutine(PlayerAttack(moveName: "slash", targetIndex: index)));

                targetAttackButtons.Add(targetButton);
            }
        }
    }

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
                                magicButton.spellNameText.SetText(moveList[i].MoveName);
                                magicButton.spellCostText.SetText(moveList[i].MoveCost.ToString() + " MP");
                                magicButtons.Add(magicButton);
                            }
                        }
                    }
                } 
            }
        }
    }

    void SetCurrentTurnText(string name) => currentTurnText.SetText("CURRENT TURN: " + name);

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
