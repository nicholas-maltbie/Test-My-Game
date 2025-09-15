using UnityEngine;

public class Level10 : LevelBase
{
    [SerializeField] protected GameObject door;

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
