using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Level4 : MonoBehaviour
{
    private VoiceoverPlayback voiceover;

    public float initialDelay = 0.5f;

    public float fixCameraDelay = 2.5f;

    [SerializeField]
    public CaptionedAudio level4Intro;

    [SerializeField]
    public CaptionedAudio handOffWork;

    [SerializeField]
    private GameObject door;

    [SerializeField]
    private SceneField level5;

    [SerializeField]
    private HorizontalCameraFollow cameraFollower;

    private bool playerExitedCameraField = false;
    private bool introAudioCompleted = false;
    private bool joshAudioCompleted = false;
    private bool levelTransferQueued = false;

    public void Start()
    {
        // Initially, play the audio for the level 2 voiceover
        this.voiceover = GameObject.FindFirstObjectByType<VoiceoverPlayback>();
        StartCoroutine(PlayInitialAudio());
        voiceover.OnPlaybackCompleted.AddListener(this.OnAudioCompleted);

        var player = GameObject.FindFirstObjectByType<BasicPlayer>();
        if (StoryFlags.DoubleJumpFixed)
        {
            player.jumpLimit = 2;
        }
        else
        {
            player.jumpLimit = 999;
        }
    }

    public void OnDestroy()
    {
        voiceover.OnPlaybackCompleted.RemoveListener(this.OnAudioCompleted);
    }

    public void OnExitCameraView()
    {
        if (introAudioCompleted && !playerExitedCameraField)
        {
            playerExitedCameraField = true;
            StartCoroutine(FixCameraFollower());
            voiceover.PlayCaptionedAudio(handOffWork);
        }
    }

    public void TransitionToNextLevel()
    {
        if (joshAudioCompleted)
        {
            Transition.ToScene(level5.SceneName);
        }
        else
        {
            levelTransferQueued = true;
        }
    }

    private void OnAudioCompleted(CaptionedAudio clip)
    {
        if (clip == level4Intro)
        {
            introAudioCompleted = true;
            if (playerExitedCameraField)
            {
                voiceover.PlayCaptionedAudio(handOffWork);
            }
        }
        if (clip == handOffWork)
        {
            joshAudioCompleted = true;
            if (levelTransferQueued)
            {
                TransitionToNextLevel();
            }
        }
    }

    private IEnumerator FixCameraFollower()
    {
        yield return new WaitForSeconds(fixCameraDelay);
        cameraFollower.enabled = true;
    }

    private IEnumerator PlayInitialAudio()
    {
        yield return new WaitForSeconds(initialDelay);
        voiceover.PlayCaptionedAudio(level4Intro);
    }
}
