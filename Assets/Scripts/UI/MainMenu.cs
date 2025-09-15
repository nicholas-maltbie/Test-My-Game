
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    public SceneField level1;

    public void StartGame()
    {
        Transition.ToScene(level1.SceneName);
    }
}
