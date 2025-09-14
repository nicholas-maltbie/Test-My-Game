using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class VoiceoverPlayback : MonoBehaviour
{
    private TextBoundingBox captionBox;
    private AudioSource audioSource;
    private CaptionedAudio currentlyPlaying;

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
            if (!audioSource.isPlaying && audioSource.time >= audioSource.clip.length)
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
