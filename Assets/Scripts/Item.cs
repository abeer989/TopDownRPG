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
            GameManager.instance.AddItemToInventory(itemSprite: itemSprite, itemName: gameObject.name, itemDesc: description, itemQuantity: 1);
            Destroy(gameObject);
        }
    }
}
