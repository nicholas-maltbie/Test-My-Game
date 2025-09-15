using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameHelper : MonoBehaviour
{
    public void ReturnToMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }
}
