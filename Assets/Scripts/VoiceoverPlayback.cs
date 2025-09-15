using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class VoiceoverPlayback : MonoBehaviour
{
    public const float bufferTime = 3f;

    private TextBoundingBox captionBox;
    private AudioSource audioSource;
    private CaptionedAudio currentlyPlaying;
    private float autioStartTime;

    /// <summary>
    /// Unity event whenever playback of current audio is completed.
    /// Value indicates which audio clip was completed.
    /// </summary>
    public UnityEvent<CaptionedAudio> OnPlaybackCompleted;

    public void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
        this.audioSource.loop = false;
        this.captionBox = GameObject.FindGameObjectWithTag("SubtitleOverlay")?.GetComponent<TextBoundingBox>();
    }

    public void PlayCaptionedAudio(CaptionedAudio audio)
    {
        this.currentlyPlaying = audio;
        this.audioSource.clip = audio.AudioClip;
        this.audioSource.Play();
        autioStartTime = Time.time;
    }

    public void Stop()
    {
        if (this.currentlyPlaying != null)
        {
            captionBox.UpdateText(string.Empty);
            var completed = this.currentlyPlaying;
            currentlyPlaying = null;
            OnPlaybackCompleted?.Invoke(completed);
        }
    }

    public void Update()
    {
        if (currentlyPlaying != null)
        {
            var audioCompleted = audioSource.time >= audioSource.clip.length || (Time.time - bufferTime > currentlyPlaying.ClipLength + bufferTime);
            if (!audioSource.isPlaying && audioCompleted)
            {
                this.Stop();
                return;
            }

            string targetText = currentlyPlaying.CurrentCaption(TimeSpan.FromSeconds(audioSource.time));
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
