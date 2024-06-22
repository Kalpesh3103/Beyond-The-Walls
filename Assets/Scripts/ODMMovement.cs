using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ODMMovement : MonoBehaviour
{
    [Header("References")]
    public Transform leftGear;
    public Transform rightGear;

    public float pullForce = 100f;

    private MovementNew pm;
    private Rigidbody rb;
    private ODMGearLeft leftGearScript;
    private ODMGearRight rightgearScript;
    private Vector3 pullPoint;

    private void Start()
    {
        pm = transform.GetComponent<MovementNew>();
        rb = transform.GetComponent<Rigidbody>();
        leftGearScript = leftGear.GetComponent<ODMGearLeft>();
        rightgearScript = rightGear.GetComponent<ODMGearRight>();
    }


    public void pullToPoint()
    {
        Debug.Log("Pulling to point");

        if (leftGearScript.swingingLeft && rightgearScript.swingingRight)
        {
            // Pull to middle;
            pullPoint = Vector3.Lerp(leftGearScript.swingPoint, rightgearScript.swingPoint, 0.5f);
        }

        else if (leftGearScript.swingingLeft && !rightgearScript.swingingRight)
        {
            // Pull to left
            pullPoint = leftGearScript.swingPoint;
        }
        else if (!leftGearScript.swingingLeft && rightgearScript.swingingRight)
        {
            // Pull to right
            pullPoint = rightgearScript.swingPoint;
        }

        leftGearScript.swingingLeft = false;
        rightgearScript.swingingRight = false;

        Vector3 directionToPoint = (pullPoint - transform.position).normalized * pullForce;
        rb.AddForce(directionToPoint, ForceMode.Impulse);

        Invoke(nameof(stopPulling), 0.1f);
    }


    private void stopPulling()
    {
        leftGearScript.canDrawRope = false;
        pm.isSwingingLeft = false;

        rightgearScript.canDrawRope = false;
        pm.isSwingingRight = false;
    }


}
