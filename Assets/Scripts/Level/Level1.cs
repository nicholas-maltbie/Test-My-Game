using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Level1 : MonoBehaviour
{
    public enum State
    {
        ReadingIntro,
        Interrupted,
        Waiting,
    }

    private State currentState = State.ReadingIntro;

    public float delayBeforeFrustratedAudio = 10.0f;

    public float initialDelay = 0.5f;

    [SerializeField]
    public CaptionedAudio level1Intro;

    [SerializeField]
    public CaptionedAudio onInterrupted;

    [SerializeField]
    public CaptionedAudio onDelay;

    [SerializeField]
    public SceneField level2;

    private VoiceoverPlayback voiceover;

    public void Start()
    {
        // Initially, play the audio for the level 1 voiceover
        this.voiceover = GameObject.FindFirstObjectByType<VoiceoverPlayback>();
        StartCoroutine(PlayInitialAudio());
        voiceover.OnPlaybackCompleted.AddListener(this.OnAudioCompleted);
    }

    public void OnDestroy()
    {
        voiceover.OnPlaybackCompleted.RemoveListener(this.OnAudioCompleted);
    }

    public void OnHitDoor()
    {
        switch (currentState)
        {
            case State.ReadingIntro:
                // narrator was interrupted, transition to interrupted state.
                voiceover.PlayCaptionedAudio(onInterrupted);
                currentState = State.Interrupted;
                break;
            case State.Waiting:
                // Player waited nicely, advance to next level
                this.TransitionToNextLevel();
                break;
        }
    }

    private void TransitionToNextLevel()
    {
        Transition.ToScene(level2.SceneName);
    }

    private void OnAudioCompleted(CaptionedAudio clip)
    {
        switch (currentState)
        {
            case State.ReadingIntro:
                // Start timer to wait for 30 seconds for dev to copmlain
                StartCoroutine(PlayDelayedAudio());
                currentState = State.Waiting;
                break;

            case State.Interrupted:
                // If narrator was interrupted, once finished talking, transition to next level
                this.TransitionToNextLevel();
                break;
        }

    }

    private IEnumerator PlayInitialAudio()
    {
        yield return new WaitForSeconds(initialDelay);
        voiceover.PlayCaptionedAudio(level1Intro);
    }

    private IEnumerator PlayDelayedAudio()
    {
        yield return new WaitForSeconds(delayBeforeFrustratedAudio);
        if (this.currentState == State.Waiting)
        {
            voiceover.PlayCaptionedAudio(onDelay);
        }
    }
}
