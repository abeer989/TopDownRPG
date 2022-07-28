using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    [SerializeField] BattleCharacterType characterType;
    [SerializeField] string characterName;

    [Space]
    [SerializeField] string[] movesAvailable;
    [SerializeField] int currentHP, maxHP, currentMP, maxMP, str, def, wpnPower, armrPower;
    
    [Space]
    [SerializeField] bool isDead;

    public BattleCharacterType CharacterType { get { return characterType; } }
    public string CharacterName { get { return characterName; } }

    public int CurrentHP
    {
        get { return currentHP; } 
        set { currentHP = value; }
    }

    public int MaxHP { get { return MaxHP; } }
    public int CurrentMP { get { return currentMP; } }
    public int MaxMP { get { return maxMP; } }

    public enum BattleCharacterType
    {
        player,
        enemy
    }

    public void SetUpBattleCharacter(PlayerStats _stats, string[] _movesAvailable, bool _isDead = false)
    {
        characterName = _stats.characterName;
        movesAvailable = _movesAvailable;
        currentHP = _stats.currentHP;
        maxHP = _stats.maxHP;
        currentMP = _stats.currentMP;
        maxMP = _stats.maxMP;
        str = _stats.strength;
        def = _stats.defence;
        wpnPower = _stats.weaponPower;
        armrPower = _stats.armorPower;
        isDead = _isDead;
    }
}