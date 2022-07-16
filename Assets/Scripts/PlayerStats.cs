using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public string characterName;
    public int characterLevel = 1;
    public int currentEXP;
    public int maxLevel = 100;
    public int baseEXP = 1000;
    
    [Space]
    public int[] EXPLevelThresholds;

    [Space]
    public int currentHP;
    public int maxHP = 100;
    public int currentMP;
    public int maxMP = 30;
    public int strength = 15;
    public int defence = 10;
    public int weaponPower;
    public int armorPower;
    public string equippedWeapon;
    public string equippedArmor;

    [Space]
    public Sprite playerSprite;

    void Start()
    {
        currentHP = maxHP;
        currentMP = maxMP;

        EXPLevelThresholds = new int[maxLevel];
        EXPLevelThresholds[1] = baseEXP;

        // setting up different thresholds of EXP needed to reach the next level:
        for (int i = 2; i < EXPLevelThresholds.Length; i++)
            EXPLevelThresholds[i] = Mathf.FloorToInt(EXPLevelThresholds[i - 1] * 1.05f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            AddEXP(1000);
    }

    public void AddEXP(int expToAdd)
    {
        currentEXP += expToAdd;

        // level up only if the current level is lesser than max level:
        if (characterLevel < maxLevel)
        {
            // if the character's current exp is >= expThreshold corresponding to their level, they level up and
            // the level has to be lesser than the max reachable level:
            if (currentEXP >= EXPLevelThresholds[characterLevel])
            {
                // everytime the char. levels up, the exp from the prev. level is going to be subtracted:
                currentEXP -= EXPLevelThresholds[characterLevel];
                characterLevel++;

                // adding to str and def stats as the player level goes up, depending upon
                // the level being an even/odd number:
                if (characterLevel % 2 == 0)
                    strength++;

                else
                    defence++;

                // also increasing the player's max health:
                maxHP = Mathf.FloorToInt(maxHP * 1.05f);
                currentHP = maxHP;

                // the max MP is going to be incremented every 3 levels:
                if (characterLevel % 3 == 0)
                {
                    maxMP += 3;
                    currentMP = maxMP;
                }
            }
        }

        if (characterLevel >= maxLevel)
            currentEXP = 0;
    }
}
