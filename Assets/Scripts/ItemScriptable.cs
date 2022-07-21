using UnityEngine;

[CreateAssetMenu(fileName = "Item Scriptable", menuName = "Create New Item Scriptable")]
public class ItemScriptable : ScriptableObject
{
    [SerializeField] Item.ItemType itemType;
    [SerializeField] Sprite itemSprite;

    [SerializeField] string itemName;
    [SerializeField] string description;
    [SerializeField] int sellWorth;
    [SerializeField] int itemAdditionFactor;
    [SerializeField] int weaponPower;
    [SerializeField] int armorPower;

    public Item.ItemType ItemType { get { return itemType; } }
    public Sprite ItemSprite { get { return itemSprite; } }

    public string ItemName { get { return itemName; } }
    public string Description { get { return description; } }
    public int SellWorth { get { return sellWorth; } }
    public int ItemAdditionFactor { get { return itemAdditionFactor; } }
    public int WeaponPower { get { return weaponPower; } }
    public int ArmorPower { get { return armorPower; } }
}

