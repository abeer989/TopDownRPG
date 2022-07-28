using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    [SerializeField] GameObject battleScene;
    [SerializeField] GameObject actionsMenu;

    [Space]
    [SerializeField] List<BattleCharacter> playerPrefabs;
    [SerializeField] List<BattleCharacter> enemyPrefabs;
    [SerializeField] List<Transform> playerPositions;
    [SerializeField] List<Transform> enemyPositions;

    [Space]
    [SerializeField] List<GameObject> FX;

    [Space]
    [SerializeField] List<BattleCharacter> activeBattleCharacters;

    MainCameraController cam;
    Vector3 cameraPos;

    int currentTurn;
    bool waitingForNextTurn;
    //bool battleActive;

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

    /// <summary>
    /// start battle at any point in the game:
    /// </summary>
    /// <param name="enemiesToSpawn"></param>
    public void BattleStart(List<string> enemiesToSpawn)
    {
        if (!GameManager.instance.battleActive)
        {
            GameManager.instance.battleActive = true;
            waitingForNextTurn = true;
            currentTurn = 0;

            cameraPos = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, battleScene.transform.localPosition.z);
            battleScene.transform.localPosition = cameraPos;
            battleScene.SetActive(true);
            AudioManager.instance.PlayMusic(0);

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
                            newPlayer.SetUpBattleCharacter(_stats: stats, _movesAvailable: null, _isDead: false);

                            activeBattleCharacters.Add(newPlayer);
                        }
                    }
                }
            }

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
        }
    }

    /// <summary>
    /// increment the turn counter and initiate the next turn:
    /// </summary>
    /*public */
    void StartNextTurn()
    {
        currentTurn++;

        if (currentTurn >= activeBattleCharacters.Count)
            currentTurn = 0;

        waitingForNextTurn = true;

        UpdateBattle();
    }

    /// <summary>
    /// update battle func. will be called and it'll check everything regarding player and enemy status
    /// and set the battle state, accordingly. For example, if all enemies are dead, the battle will be won and vice versa:
    /// </summary>
    void UpdateBattle()
    {
        bool allEnemiesDead = false;
        bool allPlayersDead = false;

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
        activeBattleCharacters[targetIndex].CurrentHP -= 30;
        Instantiate(FX[0], activeBattleCharacters[targetIndex].transform.position + new Vector3(0, .7f, 0), Quaternion.identity);
    }

    IEnumerator EnemyAttackDelayCR()
    {
        waitingForNextTurn = false;
        yield return new WaitForSeconds(1);
        EnemyAttack();
        yield return new WaitForSeconds(1);
        StartNextTurn();

        yield break;
    }
}
