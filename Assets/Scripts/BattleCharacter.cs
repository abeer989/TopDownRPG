using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    [SerializeField] string characterName;
    [SerializeField] string[] movesAvailable;
    [SerializeField] int currentHP, maxHP, currentMP, maxMP, str, def, wpnPower, armrPower;
    [SerializeField] bool isDead;

    public enum BattleCharacterType
    {
        player,
        enemy
    }
}
