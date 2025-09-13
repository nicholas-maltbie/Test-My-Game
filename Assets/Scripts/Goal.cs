using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    public UnityEvent OnTouchEvent;

    private void OnCollisionEnter2D( Collision2D collision )
    {
        if( collision.gameObject.tag == "Player" )
        {
            OnTouchEvent.Invoke();
            Debug.Log("Touched Trophy!");
        }
    }
}
