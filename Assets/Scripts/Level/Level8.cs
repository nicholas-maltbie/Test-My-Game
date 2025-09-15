using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level8 : LevelBase
{
    [SerializeField]
    private List<CaptionedAudio> introVoiceLines;

    public override void Start()
    {
        base.Start();
        Physics2D.gravity = Vector2.down * 0.75f;
        StartCoroutine(PlayIntroVoiceLines());
    }

    private IEnumerator PlayIntroVoiceLines()
    {
        foreach( var voiceLine in introVoiceLines )
        {
            yield return new WaitForSeconds(initialDelay);
            float elapsed = 0;
            currentState = State.Reading;
            voiceover.PlayCaptionedAudio(voiceLine);
            voiceover.OnPlaybackCompleted.AddListener(ResetState);

            while( currentState == State.Reading && elapsed < voiceLine.ClipLength )
            {
                yield return null;
                elapsed += Time.deltaTime;
            }
        }
    }

}
