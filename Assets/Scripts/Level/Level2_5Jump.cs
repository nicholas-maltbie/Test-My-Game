using System.Collections;
using UnityEngine;

public class Level2_5Jump : MonoBehaviour
{
    public enum State
    {
        ReadingIntro,
        FixingGravity,
        FixingDoubleJump,
        Waiting,
        FixingTrophyBug,
    }

    public bool initialGravityBugFixedStateDebug = false;

    private State currentState = State.ReadingIntro;

    public float initialDelay = 0.5f;

    public float waitForPlayer = 10f;

    [SerializeField]
    public CaptionedAudio level2_5_jump_into;

    [SerializeField]
    public CaptionedAudio gravityBugFoundAudio;

    [SerializeField]
    public CaptionedAudio fixingDoubleJumpAudio;

    [SerializeField]
    public CaptionedAudio fixingTrophyAudio;

    [SerializeField]
    public CaptionedAudio trophyBugFoundAudio;

    [SerializeField]
    public SceneField level3;

    [SerializeField]
    public GameObject door;

    private VoiceoverPlayback voiceover;

    private bool playerFloating = false;
    private bool trophyHit = false;
    private bool trophyBigFixed = false;
    private bool playerDoubleJump = false;

    public void Start()
    {
        if (initialGravityBugFixedStateDebug)
        {
            StoryFlags.GravityBugFixed = true;
        }

        // Initially, play the audio for the level 2.5 jump voiceover
        this.voiceover = GameObject.FindFirstObjectByType<VoiceoverPlayback>();
        StartCoroutine(PlayInitialAudio());
        voiceover.OnPlaybackCompleted.AddListener(this.OnAudioCompleted);
        BasicPlayer player = GameObject.FindFirstObjectByType<BasicPlayer>();
        player.useGravity = StoryFlags.GravityBugFixed;

        // This is out of scope for now lol.
        // player.OnDoubleJump.AddListener(this.OnDoubleJump);
    }

    public void OnDestroy()
    {
        voiceover.OnPlaybackCompleted.RemoveListener(this.OnAudioCompleted);
    }

    public void OnDoubleJump()
    {
        if (StoryFlags.GravityBugFixed)
        {
            playerDoubleJump = true;
            if (currentState == State.Waiting)
            {
                FixDoubleJump();
            }
        }
    }

    public void OnPlayerFloat()
    {
        playerFloating = true;
        if (StoryFlags.GravityBugFixed == false && currentState == State.Waiting)
        {
            FixFloatingBug();
        }
    }

    public void OnTouchTrophy()
    {
        trophyHit = true;
        if (currentState == State.Waiting && !trophyBigFixed)
        {
            FixTrophyBug();
        }
    }

    public void OnHitDoor()
    {
        TransitionToNextLevel();
    }

    private void FixTrophyBug()
    {
        trophyBigFixed = true;
        currentState = State.FixingTrophyBug;
        voiceover.PlayCaptionedAudio(fixingTrophyAudio);
    }

    private void FixDoubleJump()
    {
        StoryFlags.DoubleJumpFixed = true;
        currentState = State.FixingDoubleJump;
        voiceover.PlayCaptionedAudio(fixingDoubleJumpAudio);
    }

    private void FixFloatingBug()
    {
        StoryFlags.GravityBugFixed = true;
        currentState = State.FixingGravity;
        voiceover.PlayCaptionedAudio(gravityBugFoundAudio);
    }

    private void TransitionToNextLevel()
    {
        Transition.ToScene(level3.SceneName);
    }

    private void OnAudioCompleted(CaptionedAudio clip)
    {
        if (clip == level2_5_jump_into)
        {
            if (trophyHit && !trophyBugFoundAudio)
            {
                FixTrophyBug();
            }
            else if (playerFloating && !StoryFlags.GravityBugFixed)
            {
                FixFloatingBug();
            }
            else if (playerDoubleJump && !StoryFlags.DoubleJumpFixed)
            {
                FixDoubleJump();
            }
            else
            {
                currentState = State.Waiting;
            }
        }
        else if (clip == gravityBugFoundAudio)
        {
            GameObject.FindFirstObjectByType<BasicPlayer>().useGravity = true;
            currentState = State.Waiting;
            
            if (trophyHit && !trophyBugFoundAudio)
            {
                FixTrophyBug();
            }
        }
        else if (clip == fixingDoubleJumpAudio)
        {
            GameObject.FindFirstObjectByType<BasicPlayer>().jumpLimit = 1;
            currentState = State.Waiting;

            if (trophyHit && !trophyBugFoundAudio)
            {
                FixTrophyBug();
            }
        }
        else if (clip == fixingTrophyAudio)
        {
            door.SetActive(true);
            currentState = State.Waiting;

            if (playerFloating && !StoryFlags.GravityBugFixed)
            {
                FixFloatingBug();
            }
        }
    }

    private IEnumerator PlayInitialAudio()
    {
        yield return new WaitForSeconds(initialDelay);
        voiceover.PlayCaptionedAudio(level2_5_jump_into);
    }
}
