using UnityEngine;

public class Level10 : LevelBase
{
    [SerializeField] protected GameObject door;

    protected override void OnFinishCompletionVoiceLines()
    {
        base.OnFinishCompletionVoiceLines();
        door.SetActive(true);

    }
}
