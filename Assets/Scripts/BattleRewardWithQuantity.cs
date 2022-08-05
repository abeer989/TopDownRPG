using UnityEngine;

[System.Serializable]
public class BattleRewardWithQuantity
{
    [SerializeField] ItemScriptable item;
    [SerializeField] int quantity;

    public ItemScriptable Item
    {
        get { return item; }
    }

    public int Quantity
    {
        get { return quantity; }
    }
}
