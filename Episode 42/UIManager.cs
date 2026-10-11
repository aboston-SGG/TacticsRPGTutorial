using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml.Serialization;

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
    public List<InventoryItem> inventorySlots = new();
    private const int SLOT_COUNT = 5;
    public GameObject inventorySlotPrefab;
    public Transform inventoryContent;

    [Header("Convoy")]
    public CanvasGroup convoyMenu;
    public List<InventoryItem> convoySlots = new();
    public ConvoyManager convoyManager;
    public Transform convoyContent;

    public enum ConvoyTab
    {
        All, 
        Weapons,
        Consumables
    }

    private ConvoyTab currentTab = ConvoyTab.All;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        InitializeConvoyInventory();
        InitializeUnitInventory();
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
        unitItemText.text = unit.equippedWeapon.definition != null ? unit.equippedWeapon.definition.itemName : "None";
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
    public void PopulateInventory()
    {
        Unit unit = Player.selectedUnit;
        Inventory inventory = unit.inventory;

        for(int i = 0; i < SLOT_COUNT; i++)
        {
            if(i > inventory.items.Count - 1)
            {
                inventorySlots[i].item = null;
                inventorySlots[i].ClearValues();
                continue;
            }

            if (inventory.items[i] != null)
            {
                ItemInstance item = unit.inventory.GetItem(i);
                BaseItem itemData = item.definition;

                inventorySlots[i].item = item;
                inventorySlots[i].DisplayValues();

                if (unit.equippedWeapon == item)
                {
                    inventorySlots[i].itemNameDisplay.text += " (E)";
                    inventorySlots[i].GetComponent<Button>().interactable = false;
                }
                else
                {
                    inventorySlots[i].GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                inventorySlots[i].item = null;
                inventorySlots[i].ClearValues();
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
            ItemInstance item = unit.inventory.GetItem(index);

            if(item != null)
            {
                item.UseItem(item, unit);

                if(item.currentDurability <= 0)
                {
                    unit.inventory.RemoveItem(item);
                }

                PopulateInventory();
                DisplayUnitInfo();
            }
        }
    }

    // Initializes the convoy inventory by creating UI slots for each item in the convoy inventory
    public void InitializeConvoyInventory()
    {
        convoyManager.inventory.InitializeInventory();
        RefreshConvoy();
    }

    // Refreshes the Convoy GUI
    public void RefreshConvoy()
    {
        if(convoySlots.Count < convoyManager.inventory.items.Count)
        {
            int slotsToAdd = convoyManager.inventory.items.Count - convoySlots.Count;

            for(int i = 0; i <slotsToAdd; i++)
            {
                GameObject slot = Instantiate(inventorySlotPrefab, convoyContent);
                InventoryItem convoyItem = slot.GetComponent<InventoryItem>();
                convoySlots.Add(convoyItem);
                convoyItem.item = convoyManager.inventory.GetItem(i);
                convoyItem.DisplayValues();
            }
        }
    }

    // Initializes the unit inventory by creating empty UI slots for the unit's inventory
    public void InitializeUnitInventory()
    {
        for(int i = 0; i <SLOT_COUNT; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryContent);
            inventorySlots.Add(slot.GetComponent<InventoryItem>());
            int buttonIndex = i;
            slot.GetComponent<Button>().onClick.AddListener(() => UseItemSlot(buttonIndex));
        }
    }

    // Filters the convoy inventory based on the selected tab and updates the UI to show only the relevant items
    public void FilterConvoy(int tab)
    {
        currentTab = (ConvoyTab)tab;

        List<ItemInstance> filteredItems = new();

        switch (currentTab)
        {
            case ConvoyTab.All:
                filteredItems.AddRange(convoyManager.inventory.items);
                break;
            case ConvoyTab.Weapons:
                filteredItems.AddRange(convoyManager.GetWeapons());
                break;
            case ConvoyTab.Consumables:
                filteredItems.AddRange(convoyManager.GetConsumables());
                break;
        }

        foreach(InventoryItem slot in convoySlots)
        {
            if(filteredItems.Contains(slot.item))
            {
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }
}
