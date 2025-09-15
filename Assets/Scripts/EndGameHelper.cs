using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameHelper : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
