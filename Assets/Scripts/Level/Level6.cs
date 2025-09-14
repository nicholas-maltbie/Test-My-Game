using UnityEngine;

public class Level6 : LevelBase
{
    private GameObject hat;
    [SerializeField] GameObject door;

    public override void Start()
    {
        base.Start();
        // Hat is first child of gameobject
        hat = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
    }

    public void OnSaloonVisit()
    {
        hat.SetActive(false);
    }

    protected override void OnFinishCompletionVoiceLines()
    {
        base.OnFinishCompletionVoiceLines();
        door.SetActive(true);
    }
}
