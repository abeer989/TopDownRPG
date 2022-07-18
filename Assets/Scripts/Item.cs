using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] ItemType itemType;

    [Space]
    [SerializeField] Sprite itemSprite;
    [SerializeField] string itemName;
    [SerializeField] string description;

    [Space]
    [SerializeField] int sellWorth;
    [SerializeField] int itemAdditionFactor;
    [SerializeField] int weaponPower;
    [SerializeField] int armorPower;

    [Space]
    [SerializeField] string playerTag;

    SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        gameObject.name = itemName;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (itemSprite)
            spriteRenderer.sprite = itemSprite;
    }

    public ItemType Item_Type
    {
        get { return itemType; }
    }    
    
    public string ItemName
    {
        get { return itemName; }
    }

    public string Description
    {
        get { return description; }
    }

    public int SellWorth
    {
        get { return sellWorth; }
    }

    public int StatAdditionAmount
    {
        get { return itemAdditionFactor; }
    }

    public enum ItemType
    {
        none,
        hp_potion,
        mp_potion,
        str_buff,
        def_buff,
        weapon,
        armor,
        collectable
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Destroy(gameObject);

            switch (itemType)
            {
                case ItemType.hp_potion:
                    Debug.Log("healed");
                    GameManager.instance.AddItemToInventory(_sprite: itemSprite, item: gameObject.name, itemQuantity: 1);
                    break;

                case ItemType.mp_potion:
                    break;

                case ItemType.str_buff:
                    break;

                case ItemType.def_buff:
                    break;

                case ItemType.weapon:
                    break;

                case ItemType.armor:
                    break;

                case ItemType.collectable:
                    break;

                case ItemType.none:
                    break;

                default:
                    break;
            } 
        }
    }
}
