using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class LeverHandle : MonoBehaviour
{
    [SerializeField] float leftRotationThreshold, rightRotationThreshold;
    [SerializeField] bool isLeftTriggered = true, isRightTriggered = false;

    [SerializeField] UnityEvent triggerLeft, untriggerLeft, triggerRight, untriggerRight;

    private void OnCollisionStay2D( Collision2D collision )
    {
        if( transform.rotation.z > leftRotationThreshold )
        {
            if( !isLeftTriggered )
            {
                triggerLeft.Invoke();
                isLeftTriggered = true;
            }
        }
        else
        {
            if( isLeftTriggered )
            {
                untriggerLeft.Invoke();
                isLeftTriggered = false;
            }
        }

        if( transform.rotation.z < rightRotationThreshold )
        {
            // do right rotation threshold trigger
            if( !isRightTriggered )
            {
                triggerRight.Invoke();
                isRightTriggered = true;
            }
        }
        else
        {
            if( isRightTriggered )
            {
                untriggerRight.Invoke();
                isRightTriggered = false;
            }
        }
    }
}
