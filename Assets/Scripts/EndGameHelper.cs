using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameHelper : MonoBehaviour
{
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
