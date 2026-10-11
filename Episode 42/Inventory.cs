using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
    public int maxSlots = 5;
    public List<BaseItem> startingInventory = new();
    public List<ItemInstance> items = new();
    public bool[] droppableSlots;

    // Initializes the inventory with starting items
    public void InitializeInventory()
    {
        items.Clear();
        foreach(BaseItem item in startingInventory)
        {
            ItemInstance newItem = CreateItem(item);
            items.Add(newItem);

            if (droppableSlots.Length < items.Count) continue;

            if (droppableSlots[items.IndexOf(newItem)])
            {
                newItem.droppable = true;
            }
        }
    }

    // Helper method to create an ItemInstance from a BaseItem
    private static ItemInstance CreateItem(BaseItem item)
    {
        ItemInstance newItem = new()
        {
            definition = item,
            currentDurability = item.maxDurability
        };
         
        return newItem;
    }


    // Adds an item to the inventory
    public bool AddItem(BaseItem item)
    {
        if (items.Count >= maxSlots) return false;

        ItemInstance newItem = CreateItem(item);
        items.Add(newItem); 
        return true;
    }

    // Adds an item to the inventory
    public bool AddItem(ItemInstance item)
    {
        if (items.Count >= maxSlots)
        {
            return false;
        }
        items.Add(item);
        return true;
    }

    // Removes an item from the inventory
    public void RemoveItem(ItemInstance item)
    {
        items.Remove(item);
    }

    // Retrieves item information
    public ItemInstance GetItem(int index)
    {
        if (index < 0 || index >= items.Count) return null;
        return items[index];
    }
}
