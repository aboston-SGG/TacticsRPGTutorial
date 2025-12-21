using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
    public int maxSlots = 5;
    public List<BaseItem> items = new();

    // Adds an item to the inventory
    public bool AddItem(BaseItem item)
    {
        if (items.Count >= maxSlots) return false;

        items.Add(item); 
        return true;
    }

    // Removes an item from the inventory
    public void RemoveItem(BaseItem item)
    {
        items.Remove(item);
    }

    // Retrieves item information
    public BaseItem GetItem(int index)
    {
        if (index < 0 || index >= items.Count) return null;
        return items[index];
    }
}
