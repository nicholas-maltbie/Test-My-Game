using System;
using System.Collections;
using UnityEngine;

public class Level13 : LevelBase
{
    [SerializeField] CaptionedAudio winLevel, loseLevel;
    [SerializeField] TMPro.TMP_Text timer;
    [SerializeField] SceneField loseScene, winScene;

    bool hasWon = false;
    bool hasLost = false;

    public override void Start()
    {
        base.Start();
        StartCoroutine(Timer());
    }

    public void LoseLevel()
    {
        if( hasWon )
        {
            return;
        }

        hasLost = true;
        completionVoiceLines.Add( loseLevel );

        CompleteLevel();
    }

    public void WinLevel()
    {
        if( hasLost )
        {
            return;
        }

        hasWon = true;
        completionVoiceLines.Add( winLevel );

        CompleteLevel();
    }

    protected override void OnFinishCompletionVoiceLines()
    {
        base.OnFinishCompletionVoiceLines();
        if( hasWon || !hasLost )
        {
            Transition.ToScene(winScene.SceneName);
        }
        else // has lost
        {
            Transition.ToScene(loseScene.SceneName);
        }
    }

    public IEnumerator Timer()
    {
        float timeRemaining = 30;
        while( timeRemaining > 0 )
        {
            timeRemaining -= Time.deltaTime;
            timer.text = timeRemaining.ToString("F2");
            yield return null;
        }

        LoseLevel();
    }
}
