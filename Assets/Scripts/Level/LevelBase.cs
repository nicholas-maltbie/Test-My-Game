using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;

public class LevelBase : MonoBehaviour
{
    public enum State
    {
        ReadingIntro,
        Reading,
        Interrupted,
        Waiting,
    }

    protected State currentState = State.ReadingIntro;

    bool isCompleted = false;

    public float initialDelay = 0.5f;

    [SerializeField]
    public CaptionedAudio levelIntro;

    [SerializeField]
    private List<CaptionedAudio> completionVoiceLines;

    protected VoiceoverPlayback voiceover;

    public virtual void Start()
    {
        // Initially, play the audio for the level 1 voiceover
        this.voiceover = GameObject.FindFirstObjectByType<VoiceoverPlayback>();
        StartCoroutine(PlayInitialAudio());
        voiceover.OnPlaybackCompleted.AddListener(this.OnAudioCompleted);
    }

    public virtual void OnDestroy()
    {
        voiceover.OnPlaybackCompleted.RemoveListener(this.OnAudioCompleted);
    }

    public virtual void TransitionToNextLevel()
    {
        Transition.ToScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    protected void OnAudioCompleted( CaptionedAudio clip )
    {
        Debug.Log("Completed Initial Audio.");
    }

    protected virtual IEnumerator PlayInitialAudio()
    {
        yield return new WaitForSeconds(initialDelay);
        if( levelIntro )
        {
            voiceover.PlayCaptionedAudio(levelIntro);
        }
    }

    protected void ResetState( CaptionedAudio clip )
    {
        currentState = State.Waiting;
    }

    /// <summary>
    /// Function to call when a level's final set of voice lines has been run.
    /// 
    /// After voice lines are read, results in a call to OnFinishCompletionVoiceLines()
    /// </summary>
    public void CompleteLevel()
    {
        if(!isCompleted )
        {
            isCompleted = true;
            StartCoroutine(PlayCompletionVoiceLines());
        }
    }

    private IEnumerator PlayCompletionVoiceLines()
    {
        foreach( var voiceLine in completionVoiceLines )
        {
            yield return new WaitForSeconds(initialDelay);
            float elapsed = 0;
            currentState = State.Reading;
            voiceover.PlayCaptionedAudio(voiceLine);
            voiceover.OnPlaybackCompleted.AddListener(ResetState);

            while( currentState == State.Reading && elapsed < voiceLine.ClipLength)
            {
                yield return null;
                elapsed += Time.deltaTime;
            }
        }

        OnFinishCompletionVoiceLines();
    }

    protected virtual void OnFinishCompletionVoiceLines()
    {
        Debug.Log("Finished Level!");
    }
}
