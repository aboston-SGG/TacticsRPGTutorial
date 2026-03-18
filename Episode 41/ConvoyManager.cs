using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ConvoyManager : MonoBehaviour
{
    [Header("Convoy Inventory")]
    public Inventory inventory;

    // Retrieves all items of a specific type from the convoy inventory
    public List<ItemInstance> GetItemsOfType<T>() where T : BaseItem
    {
        return inventory.items.Where(i => i?.definition is T).ToList();
    }

    // Retrieves all weapons from the convoy inventory
    public List<ItemInstance> GetWeapons()
    {
        return GetItemsOfType<BaseWeapon>();
    }

    // Retrieves all consumables from the convoy inventory
    public List<ItemInstance> GetConsumables()
    {
        return GetItemsOfType<BaseConsumable>();
    }
}
