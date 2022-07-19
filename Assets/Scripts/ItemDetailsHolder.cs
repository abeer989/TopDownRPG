using UnityEngine;

[System.Serializable]
public struct ItemDetailsHolder
{
    public ItemDetailsHolder(Item.ItemType _itemType, Sprite _itemSprite, string _itemName, string _description,
                             int _sellWorth, int _itemAdditionFactor, int _weaponPower, int _armorPower)
    {
        itemType = _itemType;
        itemSprite = _itemSprite;
        itemName = _itemName;
        description = _description;
        sellWorth = _sellWorth;
        itemAdditionFactor = _itemAdditionFactor;
        weaponPower = _weaponPower;
        armorPower = _armorPower;
    }

    // item details will be cached here:
    public Item.ItemType itemType { get; private set; }
    public Sprite itemSprite { get; private set; }
    public string itemName { get; private set; }
    public string description { get; private set; }
    public int sellWorth { get; private set; }
    public int itemAdditionFactor { get; private set; }
    public int weaponPower { get; private set; }
    public int armorPower { get; private set; }
}
