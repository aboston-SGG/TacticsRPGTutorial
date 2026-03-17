using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Consumable", menuName = "Scriptable Objects/Items/Consumable")]
public class BaseConsumable : BaseItem
{
    public int healAmount;

    // Method to use the consumable item on a unit
    public override void UseItem(ItemInstance instance, Unit unit)
    {
        if(unit.health >= unit.maxHealth)
        {
            Debug.Log("Unit health is already full.");
            return;
        }

        instance.currentDurability--;
        unit.Heal(healAmount);
    }
}
