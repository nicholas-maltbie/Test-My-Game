using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level9 : LevelBase
{
    [SerializeField]
    private GameObject door;

    bool leverWasActuated = false;

    public void OnLeverActuation()
    {
        if( !leverWasActuated )
        {
            leverWasActuated = true;
            CompleteLevel();
        }
    }

    protected override void OnFinishCompletionVoiceLines()
    {
        // Depend on player hitting trophy to spawn door.
        base.OnFinishCompletionVoiceLines();
    }
}