using UnityEngine;

public class BaseItem : ScriptableObject
{
    public enum ItemType
    {
        Weapon,
        Consumable
    }

    public string itemName;
    public ItemType itemType;
    public int durability;
    public int maxDurability;
    public Sprite icon;

    // Method to use the item on a unit
    public virtual void UseItem(Unit unit)
    {
        durability--;
    }
}
