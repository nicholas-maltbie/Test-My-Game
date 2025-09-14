using UnityEngine;

public class Level7 : LevelBase
{
    public GameObject door;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        Physics2D.gravity = Vector2.down * 0.75f;
    }

    protected override void OnFinishCompletionVoiceLines()
    {
        base.OnFinishCompletionVoiceLines();
        door.SetActive(true);
    }
}
