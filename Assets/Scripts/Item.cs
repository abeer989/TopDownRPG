using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] ItemType itemType;

    [Space]
    [SerializeField] GameObject interactCanvas;

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
    bool canPickup;

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

    private void Update()
    {
        if (canPickup && Input.GetKeyDown(key: KeyCode.E) && PlayerController.instance.CanMove)
        {
            ItemDetailsHolder itemDetails = new ItemDetailsHolder(_itemType: itemType, _itemSprite: itemSprite,
                                                                  _itemName: itemName, _description: description,
                                                                  _sellWorth: sellWorth,
                                                                  _itemAdditionFactor: itemAdditionFactor,
                                                                  _weaponPower: weaponPower, _armorPower: armorPower);

            GameManager.instance.AddItemToInventory(itemToAddDetails: itemDetails, itemQuantity: 1);
            Destroy(); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            canPickup = true;
            interactCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            canPickup = false;
            interactCanvas.SetActive(false);
        }
    }

    void Destroy()
    {
        gameObject.SetActive(false);
        Destroy(gameObject, .5f);
    }

    public void SetUpItem(ItemDetailsHolder itemToSetUpDetails)
    {
        itemType = itemToSetUpDetails.itemType;
        itemSprite = itemToSetUpDetails.itemSprite;
        itemName = itemToSetUpDetails.itemName;
        description = itemToSetUpDetails.description;
        sellWorth = itemToSetUpDetails.sellWorth;
        itemAdditionFactor = itemToSetUpDetails.itemAdditionFactor;
        weaponPower = itemToSetUpDetails.weaponPower;
        armorPower = itemToSetUpDetails.weaponPower;

        spriteRenderer.sprite = itemSprite;
        gameObject.name = itemName;
    }
}
