using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InputSystem_Actions inputActions;
    public CanvasGroup managerMenu;
    public CanvasGroup unitActionMenu;
    public Unit lastSelectedUnit;

    [Header("Unit Info Panel")]
    public CanvasGroup unitInfoPanel;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI unitLevelText;
    public TextMeshProUGUI unitHealthText;
    public TextMeshProUGUI unitItemText;
    public Image unitHealthBar;
    public Image unitPortrait;

    [Header("Hover Info Panel")]
    public CanvasGroup hoverInfoPanel;
    public TextMeshProUGUI hoverNameText;
    public TextMeshProUGUI hoverLevelText;
    public Image hoverHealthBar;
    public Image hoverPortrait;

    [Header("Stat Window")]
    public Image healthBar;
    public Image XPBar;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI XPText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI perceptionText;
    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI enduranceText;
    public TextMeshProUGUI luckText;
    public TextMeshProUGUI intellectText;
    public TextMeshProUGUI spiritText;
    public TextMeshProUGUI masteryText;

    [Header("Inventory Menu")]
    public CanvasGroup inventoryMenu;
    public TextMeshProUGUI[] itemNames;
    public TextMeshProUGUI[] itemDurabilities;
    private const int SLOT_COUNT = 5;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputActions.Player.ManagerMenu.WasPressedThisFrame())
        {
            if (CheckIfMenuOpen(managerMenu))
            {
                CloseMenu(managerMenu);
            }
            else
            {
                OpenMenu(managerMenu);
            }
        }

        if(Player.selectedUnit && !CheckIfMenuOpen(managerMenu))
        {
            if(!Player.selectedUnit.isMoving)
            {
                OpenMenu(unitActionMenu);
            }
            else
            {
                CloseMenu(unitActionMenu);
            }

            OpenMenu(unitInfoPanel);
            DisplayUnitInfo();
        }
        else
        {
            CloseMenu(unitInfoPanel);
            CloseMenu(unitActionMenu);
        }

        if (Player.hoverUnit && !CheckIfMenuOpen(managerMenu) && Player.hoverUnit != Player.selectedUnit)
        {
            OpenMenu(hoverInfoPanel);
            DisplayHoverInfo();
        }
        else
        {
            CloseMenu(hoverInfoPanel);
        }

        if(Player.selectedUnit != lastSelectedUnit)
        {
            if (Player.selectedUnit)
            {
                PopulateInventory();
                lastSelectedUnit = Player.selectedUnit;
            }
            else
            {
                CloseMenu(inventoryMenu);
                lastSelectedUnit = null;
            }
        }
    }

    // Makes a target menu invisible and not interactable
    public void CloseMenu(CanvasGroup menu)
    {
        menu.alpha = 0;
        menu.interactable = false;
        menu.blocksRaycasts = false;
    }

    // Makes a target menu visible and interactable
    public void OpenMenu(CanvasGroup menu)
    {
        menu.alpha = 1;
        menu.interactable = true;
        menu.blocksRaycasts = true;
    }

    // Checks if a given menu is open or closed
    public bool CheckIfMenuOpen(CanvasGroup menu)
    {
        if(menu.alpha == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Displays the selected unit's information on the UI panel
    public void DisplayUnitInfo()
    {
        Unit unit = Player.selectedUnit;
        unitNameText.text = unit.unitName;
        unitPortrait.sprite = unit.unitPortrait;
        unitLevelText.text = $"Lvl {unit.level}";
        unitHealthText.text = $"{unit.health} / {unit.maxHealth}";
        unitHealthBar.fillAmount = (float)unit.health / unit.maxHealth;
        unitItemText.text = unit.equippedWeapon ? unit.equippedWeapon.itemName : string.Empty;
    }

    // Called when the Wait button is pressed
    public void WaitButton()
    {
        if (Player.selectedUnit)
        {
            Player.selectedUnit.ExpendUnit();
            Player.selectedUnit = null;
        }
    }

    // Displays the hovered unit's information on the UI panel
    public void DisplayHoverInfo()
    {
        Unit hoverUnit = Player.hoverUnit;
        hoverNameText.text = hoverUnit.unitName;
        hoverPortrait.sprite = hoverUnit.unitPortrait;
        hoverLevelText.text = $"Lvl {hoverUnit.level}";
        hoverHealthBar.fillAmount = (float)hoverUnit.health / hoverUnit.maxHealth;
    }

    // Populates the stat window with the selected unit's stats
    public void PopulateStatWindow()
    {
        Unit unit = Player.selectedUnit;

        healthBar.fillAmount = (float)unit.health / unit.maxHealth;
        XPBar.fillAmount = (float)unit.experience / unit.experienceToLevel;
        nameText.text = $"{unit.unitName}: Level {unit.level} {unit.unitClass}";
        healthText.text = $"HP: {unit.health} / {unit.maxHealth}";
        XPText.text = $"XP: {unit.experience} / {unit.experienceToLevel}";
        speedText.text = $"Speed: {unit.stats.speed}";
        perceptionText.text = $"Perception: {unit.stats.perception}";
        strengthText.text = $"Strength: {unit.stats.strength}";
        enduranceText.text = $"Endurance: {unit.stats.endurance}";
        luckText.text = $"Luck: {unit.stats.luck}";
        intellectText.text = $"Intellect: {unit.stats.intellect}";
        spiritText.text = $"Spirit: {unit.stats.spirit}";
        masteryText.text = $"Mastery: {unit.stats.mastery}";
    }

    // Displays inventory information
    private void PopulateInventory()
    {
        Unit unit = Player.selectedUnit;
        Inventory inventory = unit.inventory;

        for(int i = 0; i < SLOT_COUNT; i++)
        {
            if(i > inventory.items.Count - 1)
            {
                itemNames[i].text = string.Empty;
                itemDurabilities[i].text = string.Empty;
                continue;
            }

            if (inventory.items[i] != null)
            {
                BaseItem item = inventory.GetItem(i);

                itemNames[i].text = item.itemName;
                itemDurabilities[i].text = $"{item.durability}/{item.maxDurability}";

                if (unit.equippedWeapon == item)
                {
                    itemNames[i].text += " (E)";
                    itemNames[i].GetComponentInParent<Button>().interactable = false;
                }
                else
                {
                    itemNames[i].GetComponentInParent<Button>().interactable = true;
                }
            }
            else
            {
                itemNames[i].text = string.Empty;
                itemDurabilities[i].text = string.Empty;
            }
        }
    }

    // Opens and closes the inventory
    public void ToggleInventory()
    {
        if (CheckIfMenuOpen(inventoryMenu))
        {
            CloseMenu(inventoryMenu);
        }
        else
        {
            OpenMenu(inventoryMenu);
            PopulateInventory();
        }
    }

    // Uses an item from the inventory based on the given index
    public void UseItemSlot(int index)
    {
        Unit unit = Player.selectedUnit;

        if(unit.inventory.items.Count > index)
        {
            BaseItem item = unit.inventory.GetItem(index);

            if(item != null)
            {
                item.UseItem(unit);

                if(item.durability <= 0)
                {
                    unit.inventory.RemoveItem(item);
                }

                PopulateInventory();
                DisplayUnitInfo();
            }
        }
    }
}
