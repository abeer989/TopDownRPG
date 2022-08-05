using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;

    [Space]
    [SerializeField] BattleCharacterType characterType;
    [SerializeField] string characterName;

    [Space]
    [SerializeField] string[] movesAvailable;
    [SerializeField] int currentHP, maxHP, currentMP, maxMP, str, def, wpnPower, armrPower;

    [Space]
    [SerializeField] float fadeSpeed;
    
    //[Space]
    //[SerializeField] bool isDead;

    SpriteRenderer spriteRenderer;
    bool shouldFade = false;

    // public properties:
    public BattleCharacterType CharacterType { get { return characterType; } }
    public string CharacterName { get { return characterName; } }
    public string[] MovesAvailable { get { return movesAvailable; } }

    public int CurrentHP
    {
        get { return currentHP; } 
        set { currentHP = value; }
    }    
    
    public int MaxHP { get { return maxHP; } }

    public int CurrentMP
    {
        get { return currentMP; }
        set { currentMP = value; }
    }

    public int MaxMP { get { return maxMP; } }
    public int Str { get { return str; } }
    public int Def { get { return def; } }
    public int WpnPower { get { return wpnPower; } }
    public int ArmrPower { get { return armrPower; } }

    public enum BattleCharacterType
    {
        player,
        enemy
    }

    private void Start() => spriteRenderer = GetComponent<SpriteRenderer>();

    private void Update()
    {
        if (shouldFade)
        {
            spriteRenderer.color = new Color(r: Mathf.MoveTowards(spriteRenderer.color.r, 1, fadeSpeed * Time.deltaTime),
                                             g: Mathf.MoveTowards(spriteRenderer.color.g, 0, fadeSpeed * Time.deltaTime),
                                             b: Mathf.MoveTowards(spriteRenderer.color.b, 0, fadeSpeed * Time.deltaTime),
                                             a: Mathf.MoveTowards(spriteRenderer.color.a, 0, fadeSpeed * Time.deltaTime));

            if (spriteRenderer.color.a <= 0)
            {
                shouldFade = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void SetUpBattleCharacter(PlayerStats _stats)
    {
        characterName = _stats.characterName;
        currentHP = _stats.currentHP;
        maxHP = _stats.maxHP;
        currentMP = _stats.currentMP;
        maxMP = _stats.maxMP;
        str = _stats.strength;
        def = _stats.defence;
        wpnPower = _stats.weaponPower;
        armrPower = _stats.armorPower;
        //isDead = _isDead;
    }

    public void SetState(bool dead = true)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (sprites != null && sprites.Length > 0)
        {
            if (dead)
                spriteRenderer.sprite = sprites[1]; // dead sprite

            else
                spriteRenderer.sprite = sprites[0]; // alive sprite 
        }
    }

    public void EnemyFade() => shouldFade = true;
}