using UnityEngine;

public class Killbox : MonoBehaviour
{
    Vector3 startLocation;
    void Start()
    {
        startLocation = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    private void OnCollisionEnter2D( Collision2D collision )
    {
        if( collision.gameObject.tag == "Player")
        {
            collision.transform.position = startLocation;
            collision.rigidbody.linearVelocity = Vector3.zero;
        }
    }
}
