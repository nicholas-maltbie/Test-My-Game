
using UnityEngine;

public class PlayerJumpFromStoryFlag : MonoBehaviour
{
    public const int JumpsWhenFixed = 1;
    public const int JumpsWhenBroken = 5;

    public void Awake()
    {
        var player = GetComponent<BasicPlayer>();
        if (StoryFlags.DoubleJumpFixed)
        {
            player.jumpLimit = JumpsWhenFixed;
        }
        else
        {
            player.jumpLimit = JumpsWhenBroken;
        }
    }
}
