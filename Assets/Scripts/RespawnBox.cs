using UnityEngine;

public class RespawnBox : MonoBehaviour
{
    [SerializeField]
    Transform respawnPosition;

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Basic3DPlayer>() != null)
        {
            var cc = other.GetComponent<CharacterController>();
            cc.enabled = false;
            other.gameObject.transform.position = respawnPosition.position;
            cc.enabled = true;
        }
    }
}