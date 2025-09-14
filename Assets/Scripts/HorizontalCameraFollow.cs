using UnityEngine;

public class HorizontalCameraFollow : MonoBehaviour
{
    Vector3 initialPosition;
    Transform followTarget;

    private void Start()
    {
        initialPosition = transform.position;
        followTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        var targetPosition = new Vector3( Mathf.Max(followTarget.position.x, initialPosition.x),
                                          Mathf.Max(followTarget.position.y, initialPosition.y),
                                          transform.position.z);

        transform.position = Vector3.Slerp(transform.position, targetPosition, .1f);
    }
}
