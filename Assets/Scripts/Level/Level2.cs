using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Level2 : MonoBehaviour
{
    public enum State
    {
        ReadingIntro,
        Waiting,
        IdentifyBug,
    }

    private bool gravityBugFound = false;

    private State currentState = State.ReadingIntro;

    public float delayBeforeFrustratedAudio = 10.0f;

    public float initialDelay = 0.5f;

    [SerializeField]
    public CaptionedAudio level2Intro;

    [SerializeField]
    public CaptionedAudio gravityBugFoundAudio;

    [SerializeField]
    public SceneField level3;

    private VoiceoverPlayback voiceover;

    public void Start()
    {
        // Initially, play the audio for the level 2 voiceover
        this.voiceover = GameObject.FindFirstObjectByType<VoiceoverPlayback>();
        StartCoroutine(PlayInitialAudio());
        voiceover.OnPlaybackCompleted.AddListener(this.OnAudioCompleted);
    }

    public void OnDestroy()
    {
        voiceover.OnPlaybackCompleted.RemoveListener(this.OnAudioCompleted);
    }

    public void OnPlayerFloat()
    {
        if (currentState == State.Waiting && gravityBugFound == false)
        {
            FixFloatingBug();
        }

        gravityBugFound = true;
    }

    public void OnHitDoor()
    {
        TransitionToNextLevel();
    }

    private void FixFloatingBug()
    {
        StoryFlags.GravityBugFixed = true;
        currentState = State.IdentifyBug;
        voiceover.PlayCaptionedAudio(gravityBugFoundAudio);
    }

    private void TransitionToNextLevel()
    {
        SceneManager.LoadScene(level3.SceneName);
    }

    private void OnAudioCompleted(CaptionedAudio clip)
    {
        switch (currentState)
        {
            case State.IdentifyBug:
                GameObject.FindFirstObjectByType<BasicPlayer>().useGravity = true;
                currentState = State.Waiting;
                break;
            case State.ReadingIntro:
                if (gravityBugFound)
                {
                    FixFloatingBug();
                }
                else
                {
                    currentState = State.Waiting;
                }

                break;
        }
    }

    private IEnumerator PlayInitialAudio()
    {
        yield return new WaitForSeconds(initialDelay);
        voiceover.PlayCaptionedAudio(level2Intro);
    }
}
