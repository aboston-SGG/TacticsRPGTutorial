using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public TextMeshProUGUI itemNameDisplay;
    public TextMeshProUGUI durabilityDisplay;
    public Image itemIcon;

    public ItemInstance item;

    // Displays the item values in the UI
    public void DisplayValues()
    {
        itemNameDisplay.text = item.definition.itemName;
        durabilityDisplay.text = $"{item.currentDurability}/{item.definition.maxDurability}";
        itemIcon.color = Color.white;
        itemIcon.sprite = item.definition.icon;
    }

    // Clears the display values when the item is removed from the inventory
    public void ClearValues()
    {
        itemNameDisplay.text = string.Empty;
        durabilityDisplay.text = string.Empty;
        itemIcon.color = new Color(1, 1, 1, 0); 
    }

}
