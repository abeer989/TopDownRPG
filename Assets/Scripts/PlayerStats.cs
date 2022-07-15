using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public string characterName;
    public int characterLevel;
    public int currentEXP;
    public int[] expToNextLevel;
    public int maxLevel = 100;
    public int baseEXP = 1000;

    public int HP;
    public int maxHP = 100;
    public int MP;
    public int maxMP = 30;
    public int strength;
    public int defence;
    public int weaponPower;
    public int armorPower;
    public string eqippedWeapon;
    public string eqippedArmor;

    public Sprite playerSprite;

    void Start()
    {
        expToNextLevel = new int[maxLevel];

        for (int i = 1; i < expToNextLevel.Length; i++)
            expToNextLevel[i] = expToNextLevel[i - 1] + 500;
    }

    void Update()
    {
        
    }
}
