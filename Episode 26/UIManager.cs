using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InputSystem_Actions inputActions;
    public CanvasGroup managerMenu;

    [Header("Unit Info Panel")]
    public CanvasGroup unitInfoPanel;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI unitLevelText;
    public TextMeshProUGUI unitHealthText;
    public TextMeshProUGUI unitItemText;
    public Image unitHealthBar;
    public Image unitPortrait;

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
            OpenMenu(unitInfoPanel);
            DisplayUnitInfo();
        }
        else
        {
            CloseMenu(unitInfoPanel);
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

    public void DisplayUnitInfo()
    {
        Unit unit = Player.selectedUnit;
        unitNameText.text = unit.unitName;
        unitPortrait.sprite = unit.unitPortrait;
        unitLevelText.text = $"Lvl {unit.level}";
        unitHealthText.text = $"{unit.health} / {unit.maxHealth}";
        unitHealthBar.fillAmount = (float)unit.health / unit.maxHealth;
    }
}
