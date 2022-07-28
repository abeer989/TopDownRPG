using UnityEngine;

[System.Serializable]
public class BattleMove
{
    [SerializeField] AttackFX attackFX;

    [Space]
    [SerializeField] string movePower;
    [SerializeField] int moveDamage;
    [SerializeField] int moveCost;

    public AttackFX AttackFX { get { return attackFX; } }
    public string MoveName { get { return movePower; } }
    public int MoveCost { get { return moveCost; } }
    public int MovePower { get { return moveDamage; } }
}
