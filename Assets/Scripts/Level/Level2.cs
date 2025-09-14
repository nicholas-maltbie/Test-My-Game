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
        Frustrated,
        MovingDoor,
    }

    private State currentState = State.ReadingIntro;

    public float delayBeforeFrustratedAudio = 10.0f;

    public float initialDelay = 0.5f;

    public float waitForPlayer = 10f;

    [SerializeField]
    public CaptionedAudio level2Intro;

    [SerializeField]
    public CaptionedAudio gravityBugFoundAudio;

    [SerializeField]
    public CaptionedAudio moveDoorAudio;

    [SerializeField]
    public SceneField level2_5_jump;

    [SerializeField]
    public Rigidbody2D door;

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

    public void FixedUpdate()
    {
        if (currentState == State.MovingDoor)
        {
            // Move the door closer to the player
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            // Accelerate the door toward the player
            var delta = player.transform.position - door.transform.position;
            delta = delta.normalized * 10;
            var targetVelocity = new Vector2(delta.x, delta.y);
            var currentVelocity = door.linearVelocity;

            var push = Vector2.ClampMagnitude(targetVelocity - currentVelocity, 5 * Time.fixedDeltaTime);
            door.linearVelocity += push;
        }
    }

    public void OnPlayerFloat()
    {
        if (currentState == State.Waiting && StoryFlags.GravityBugFixed == false)
        {
            FixFloatingBug();
        }

        StoryFlags.GravityBugFixed = true;
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
        Transition.ToScene(level2_5_jump.SceneName);
    }

    private void OnAudioCompleted(CaptionedAudio clip)
    {
        if (clip == moveDoorAudio)
        {
            currentState = State.MovingDoor;
        }
        else if (clip == gravityBugFoundAudio)
        {
            GameObject.FindFirstObjectByType<BasicPlayer>().useGravity = true;
            StoryFlags.GravityBugFixed = true;
            currentState = State.Waiting;
            StartCoroutine(WaitForStuffToHappen());
        }
        else if (clip == level2Intro)
        {
            if (StoryFlags.GravityBugFixed)
            {
                FixFloatingBug();
            }
            else
            {
                currentState = State.Waiting;
                StartCoroutine(WaitForStuffToHappen());
            }
        }
    }

    private IEnumerator PlayInitialAudio()
    {
        yield return new WaitForSeconds(initialDelay);
        voiceover.PlayCaptionedAudio(level2Intro);
    }

    private IEnumerator WaitForStuffToHappen()
    {
        yield return new WaitForSeconds(waitForPlayer);
        if (currentState == State.Waiting)
        {
            voiceover.PlayCaptionedAudio(moveDoorAudio);
        }

        currentState = State.Frustrated;
    }
}
