using UnityEngine; 
using System.Collections.Generic;

[System.Serializable]
public class BattleInfo
{
    [SerializeField] int expToAward;

    [Space]
    [SerializeField] List<string> enemies;

    [Space]
    [SerializeField] BattleRewardWithQuantity[] rewardItemsWithQuantity;

    public int EXPtoAward { get { return expToAward; } }
    public List<string> Enemies { get { return enemies; } }
    public BattleRewardWithQuantity[] RewardItemsWithQuantity { get { return rewardItemsWithQuantity; } }
}
