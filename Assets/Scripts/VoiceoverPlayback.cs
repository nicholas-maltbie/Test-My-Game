using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoiceoverPlayback : MonoBehaviour
{
    [SerializeField]
    public CaptionedAudio playbackOnStart;

    private TextBoundingBox captionBox;
    private AudioSource audioSource;
    private CaptionedAudio currentlyPlaying;

    public void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
        this.audioSource.loop = false;
        this.captionBox = GameObject.FindGameObjectWithTag("SubtitleOverlay")?.GetComponent<TextBoundingBox>();
    }

    public void Start()
    {
        if (playbackOnStart != null)
        {
            this.PlayCaptionedAudio(playbackOnStart);
        }
    }

    public void PlayCaptionedAudio(CaptionedAudio audio)
    {
        this.currentlyPlaying = audio;
        audioSource.clip = audio.AudioClip;
        audioSource.Play();
    }

    public void Update()
    {
        if (currentlyPlaying != null)
        {
            var playbackTime = audioSource.time;
            if (playbackTime > currentlyPlaying.ClipLength)
            {
                captionBox.UpdateText(string.Empty);
                return;
            }

            string targetText = currentlyPlaying.CurrentCaption(TimeSpan.FromSeconds(playbackTime));
            if (targetText != captionBox.CurrentText)
            {
                captionBox.UpdateText(targetText);
            }
        }
        else if (captionBox.CurrentText != string.Empty)
        {
            captionBox.UpdateText(string.Empty);
        }
    }
}
