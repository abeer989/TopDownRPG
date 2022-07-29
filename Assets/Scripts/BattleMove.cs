using UnityEngine;

[System.Serializable]
public class BattleMove
{
    [SerializeField] AttackFX attackFX;

    [Space]
    [SerializeField] string moveName;
    [SerializeField] int moveDamage;
    [SerializeField] int moveCost;

    public AttackFX AttackFX { get { return attackFX; } }
    public string MoveName { get { return moveName; } }
    public int MoveDamage { get { return moveDamage; } }
    public int MoveCost { get { return moveCost; } }
}
