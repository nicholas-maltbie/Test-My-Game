using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Voiceover", menuName = "ScriptableObjects/CaptionedAudio", order = 1)]
public class CaptionedAudio : ScriptableObject
{
    [SerializeField]
    private AudioClip audioClip;

    [SerializeField]
    private TextAsset captionsSrt;

    private Subtitle[] cachedSubtitles;

    /// <summary>
    /// Gets the audio clip for this segment.
    /// </summary>
    public AudioClip AudioClip => audioClip;

    /// <summary>
    /// Gets the set of subtitles for a given captioend audio.
    /// </summary>
    public Subtitle[] Subtitles => cachedSubtitles ??= ParseSubtitles();

    /// <summary>
    /// Gets the length of the captioned audio in seconds.
    /// It's the maximum of the audio clip or the subtitles.
    /// </summary>
    public float ClipLength => Mathf.Max(audioClip.length, Subtitles.Max(sub => (float)sub.EndTime.TotalSeconds));

    /// <summary>
    /// Get the current caption for a given playback time.
    /// </summary>
    /// <returns>Caption for the given time.</returns>
    public string CurrentCaption(TimeSpan time)
    {
        if (this.Subtitles.Length == 0)
        {
            return string.Empty;
        }

        int left = 0;
        int right = this.Subtitles.Length - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            Subtitle selected = this.Subtitles[mid];

            if (selected.StartTime > time)
            {
                right = mid - 1;
            }
            else if (selected.EndTime < time)
            {
                left = mid + 1;
            }
            else
            {
                return selected.Text;
            }
        }

        return string.Empty;
    }

    public class Subtitle
    {
        public int Index { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Text { get; set; }
    }

    public Subtitle[] ParseSubtitles()
    {
        List<Subtitle> subtitles = new List<Subtitle>();
        var lines = captionsSrt.text.Split("\n");
        int i = 0;
        while (i < lines.Length)
        {
            if (int.TryParse(lines[i], out int index))
            {
                var times = lines[++i].Split(" --> ");
                var text = "";
                i++;

                while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
                {
                    text += lines[i++].Trim() + "\n";
                }

                subtitles.Add(new Subtitle
                {
                    Index = index,
                    StartTime = TimeSpan.Parse(times[0].Replace(",", ".")),
                    EndTime = TimeSpan.Parse(times[1].Replace(",", ".")),
                    Text = text,
                });
            }

            i++;
        }

        return subtitles.ToArray();
    }
}