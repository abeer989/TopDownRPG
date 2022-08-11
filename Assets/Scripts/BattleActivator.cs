using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleActivator : MonoBehaviour
{
    [SerializeField] string playerTag;

    [Space]
    [SerializeField] BattleActivationType battleActivationType;
    [SerializeField] float timeBetweenBattles;

    [Space]
    [SerializeField] bool bossBattle;
    [SerializeField] bool deactivateOnBattleStart;

    [Header("Quest Completion")]
    [SerializeField] string questName;
    [SerializeField] bool shouldMarkQuest;

    [Space]
    [SerializeField] List<BattleInfo> potentialBattles;

    bool inBattleZone;
    float battleCounter;

    public enum BattleActivationType
    {
        activate_on_enter, 
        activate_on_stay, 
        activate_on_exit, 
    }

    private void Start() => battleCounter = Random.Range(timeBetweenBattles * .5f, timeBetweenBattles * 1.5f);

    private void Update()
    {
        if (battleActivationType == BattleActivationType.activate_on_stay)
        {
            if (inBattleZone && PlayerController.instance.CanMove)
            {
                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                    battleCounter -= Time.deltaTime;

                if (battleCounter <= 0)
                {
                    battleCounter = Random.Range(timeBetweenBattles * .5f, timeBetweenBattles * 1.5f);
                    StartCoroutine(StartBattleCR());
                }
            } 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (battleActivationType == BattleActivationType.activate_on_enter)
                StartCoroutine(StartBattleCR());

            else
                inBattleZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (battleActivationType == BattleActivationType.activate_on_exit)
            StartCoroutine(StartBattleCR());

        else
            inBattleZone = false;
    }

    IEnumerator StartBattleCR()
    {
        if (potentialBattles.Count > 0)
        {
            // when the player enters battle zone:

            //PlayerController.instance.CanMove = false;
            GameManager.instance.battleActive = true;
            UIController.instance.FadeToBlack();

            // set up quest to mark complete:
            if (shouldMarkQuest)
                BattleRewardManager.instance.SetQuestToMark_BRM(qtm: questName);

            // select a random battle from list:
            int randomBattleIndex = Random.Range(0, potentialBattles.Count);
            BattleInfo battle = potentialBattles[randomBattleIndex];

            // setup rewards that the player will get at the end of the battle:
            BattleManager.instance.SetupBattleRewards(_expToReward_BM: battle.EXPtoAward, _battleRewards_BM: battle.RewardItemsWithQuantity);

            yield return new WaitForSeconds(1.5f);

            // start the battle:
            BattleManager.instance.BattleStart(enemiesToSpawn: battle.Enemies, isBoss: bossBattle);
            UIController.instance.FadeFromBlack();

            if (deactivateOnBattleStart)
                gameObject.SetActive(false); 
        }

        yield break;
    }
}
