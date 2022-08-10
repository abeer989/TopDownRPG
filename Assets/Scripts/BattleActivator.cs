using UnityEngine;
using System.Collections.Generic;

public class BattleActivator : MonoBehaviour
{
    [SerializeField] List<BattleInfo> potentialBattles;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int randomBattleIndex = Random.Range(0, potentialBattles.Count);
            BattleInfo battle = potentialBattles[randomBattleIndex];

            Debug.Log("battle index: " + randomBattleIndex);

            BattleManager.instance.SetupBattleRewards(_expToReward_BM: battle.EXPtoAward, _battleRewards_BM: battle.RewardItemsWithQuantity);
            BattleManager.instance.BattleStart(enemiesToSpawn: battle.Enemies);
        }
    }
}
