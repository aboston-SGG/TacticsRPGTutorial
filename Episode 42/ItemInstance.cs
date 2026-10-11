using UnityEngine;

[System.Serializable]
public class ItemInstance 
{
    public BaseItem definition;

    public int currentDurability;

    public bool droppable;

    // Method to use the item on a unit
    public void UseItem(ItemInstance instance, Unit unit)
    {
        definition.UseItem(this, unit);
    }
}
