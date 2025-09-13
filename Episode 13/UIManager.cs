using UnityEngine;

public class UIManager : MonoBehaviour
{
    public InputSystem_Actions inputActions;
    public CanvasGroup managerMenu;

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
}
