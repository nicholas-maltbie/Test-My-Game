using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Level3 : MonoBehaviour
{
    public float initialDelay = 0.5f;

    [SerializeField]
    public CaptionedAudio level3Intro;

    [SerializeField]
    public CaptionedAudio platformsAdded;

    [SerializeField]
    public CaptionedAudio doubleJumpBugFound;

    [SerializeField]
    public CaptionedAudio fixingDoubleJumpAudio;

    [SerializeField]
    public CaptionedAudio letsMoveOnAudio;

    [SerializeField]
    public SceneField level4;

    private VoiceoverPlayback voiceover;

    private bool playerBeatLevel;

    [SerializeField]
    private GameObject door;

    [SerializeField]
    private GameObject platforms;

    public void Start()
    {
        // Initially, play the audio for the level 2 voiceover
        this.voiceover = GameObject.FindFirstObjectByType<VoiceoverPlayback>();
        StartCoroutine(PlayInitialAudio());
        voiceover.OnPlaybackCompleted.AddListener(this.OnAudioCompleted);
        GameObject.FindFirstObjectByType<BasicPlayer>().jumpLimit = 2;
    }

    public void OnDestroy()
    {
        voiceover.OnPlaybackCompleted.RemoveListener(this.OnAudioCompleted);
    }

    public void OnHitTrophy()
    {
        if (platforms.gameObject.activeInHierarchy)
        {
            door.SetActive(true);
        }
        else if (!playerBeatLevel)
        {
            FixDoubleJump();
        }

        playerBeatLevel = true;
    }

    public void OnHitDoor()
    {
        TransitionToNextLevel();
    }

    private void TransitionToNextLevel()
    {
        Transition.ToScene(level4.SceneName);
    }

    private void FixDoubleJump()
    {
        StoryFlags.DoubleJumpFixed = true;
        voiceover.PlayCaptionedAudio(doubleJumpBugFound);
    }

    private void OnAudioCompleted(CaptionedAudio clip)
    {
        if (clip == level3Intro)
        {
            if (playerBeatLevel)
            {
                FixDoubleJump();
            }
            else
            {
                GameObject.FindFirstObjectByType<BasicPlayer>().jumpLimit = 999;
                StartCoroutine(FixPlatformsInABit());
            }
        }
        else if (clip == platformsAdded)
        {
            platforms.SetActive(true);
        }
        else if (clip == doubleJumpBugFound)
        {
            voiceover.PlayCaptionedAudio(fixingDoubleJumpAudio);
        }
        else if (clip == fixingDoubleJumpAudio)
        {
            GameObject.FindFirstObjectByType<BasicPlayer>().jumpLimit = 1;
            voiceover.PlayCaptionedAudio(letsMoveOnAudio);
        }
        else if (clip == letsMoveOnAudio)
        {
            door.SetActive(true);
        }
    }

    private IEnumerator PlayInitialAudio()
    {
        yield return new WaitForSeconds(initialDelay);
        voiceover.PlayCaptionedAudio(level3Intro);
    }

    private IEnumerator FixPlatformsInABit()
    {
        yield return new WaitForSeconds(10);
        if (!playerBeatLevel)
        {
            voiceover.PlayCaptionedAudio(platformsAdded);
            platforms.SetActive(true);
        }
    }
}
