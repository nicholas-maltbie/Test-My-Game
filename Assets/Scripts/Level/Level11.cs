
using System.Collections;
using UnityEngine;

public class Level11 : LevelBase
{
    public float firstHorseAudioDelay = 10;
    public float secondHorseAudioDelay = 5;

    [SerializeField]
    GameObject firstHorse;

    [SerializeField]
    GameObject secondHorse;

    [SerializeField]
    CaptionedAudio firstHorseHitAudio;

    [SerializeField]
    CaptionedAudio noHenriettaAudio;

    [SerializeField]
    CaptionedAudio respawnAudio;

    [SerializeField]
    GameObject door;

    private bool firstHorseHit;
    private bool secondHorseHit;

    public new void Start()
    {
        base.Start();
        Physics.gravity = Vector3.down * 10;
        StartCoroutine(SpawnHorse());
    }

    public void OnHitFirstHorse()
    {
        if (!firstHorseHit)
        {
            firstHorseHit = true;
            StartCoroutine(SpawnSecondHorse());
        }
    }

    public void OnSecondHorseHit()
    {
        if (!secondHorseHit)
        {
            secondHorseHit = true;
            base.voiceover.PlayCaptionedAudio(noHenriettaAudio);
            door.SetActive(true);
        }
    }

    public void OnPlayerRespawn()
    {
        base.voiceover.PlayCaptionedAudio(respawnAudio);
    }

    public IEnumerator SpawnSecondHorse()
    {
        base.voiceover.PlayCaptionedAudio(firstHorseHitAudio);
        yield return new WaitForSeconds(secondHorseAudioDelay);
        secondHorse.SetActive(true);
    }

    public IEnumerator SpawnHorse()
    {
        yield return new WaitForSeconds(firstHorseAudioDelay);
        firstHorse.SetActive(true);
    }
}
