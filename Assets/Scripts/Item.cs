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
            ItemDetailsHolder itemDetails = new ItemDetailsHolder(_itemType: itemType, _itemSprite: itemSprite,
                                                                  _itemName: itemName, _description: description,
                                                                  _sellWorth: sellWorth,
                                                                  _itemAdditionFactor: itemAdditionFactor,
                                                                  _weaponPower: weaponPower, _armorPower: armorPower);

            GameManager.instance.AddItemToInventory(itemDetails: itemDetails, itemQuantity: 1);
            Destroy();
        }
    }

    void Destroy()
    {
        gameObject.SetActive(false);
        Destroy(gameObject, .5f);
    }
}
