using UnityEngine;
using UnityEngine.Events;

public class SecondHorse : MonoBehaviour
{
    public UnityEvent OnTouchEvent;

    public void OnPlayerHit(GameObject player, Vector3 point)
    {
        // Check which direction to throw the horse
        Vector3 dir = transform.position - player.transform.position;
        GetComponent<Rigidbody>().AddForceAtPosition(dir.normalized * 10, point, ForceMode.VelocityChange);

        OnTouchEvent.Invoke();
    }
}
