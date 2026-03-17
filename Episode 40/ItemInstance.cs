using UnityEngine;

[System.Serializable]
public class ItemInstance 
{
    public BaseItem definition;

    public int currentDurability;

    // Method to use the item on a unit
    public void UseItem(ItemInstance instance, Unit unit)
    {
        definition.UseItem(this, unit);
    }
}
