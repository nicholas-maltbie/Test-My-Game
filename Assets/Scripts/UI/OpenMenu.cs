
using UnityEngine;
using UnityEngine.InputSystem;

class OpenMenu : MonoBehaviour
{
    public InputActionReference openMenu;
    public GameObject menuObject;

    public void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        openMenu.action.performed += this.OnOpenMenu;
    }

    public void OnDestroy()
    {
        openMenu.action.performed -= this.OnOpenMenu;
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        ToggleMenu();
    }

    public void ToggleMenu()
    {
        // toggle menu object
        menuObject.SetActive(!menuObject.activeInHierarchy);

        if (menuObject.activeInHierarchy)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
