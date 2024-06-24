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
        float newForce = 0f;
        Debug.Log("Pulling to point");

        if (leftGearScript.swingingLeft && rightgearScript.swingingRight)
        {
            // Pull to middle;
            newForce = pullForce;
            pullPoint = Vector3.Lerp(leftGearScript.swingPoint, rightgearScript.swingPoint, 0.5f);
        }

        else if (leftGearScript.swingingLeft && !rightgearScript.swingingRight)
        {
            // Pull to left
            newForce = pullForce / 2;
            pullPoint = leftGearScript.swingPoint;
        }
        else if (!leftGearScript.swingingLeft && rightgearScript.swingingRight)
        {
            // Pull to right
            newForce = pullForce / 2;
            pullPoint = rightgearScript.swingPoint;
        }

        leftGearScript.swingingLeft = false;
        rightgearScript.swingingRight = false;

        Vector3 direction = pullPoint - transform.position;

        Vector3 directionToPoint = (pullPoint - transform.position).normalized * newForce;
        rb.AddForce(directionToPoint, ForceMode.Impulse);
        // rb.velocity = directionToPoint;
        // float velocity = rb.velocity.magnitude;
        Vector3 newVelocity = directionToPoint / rb.mass;
        float velocity = newVelocity.magnitude;


        Debug.Log("Velocity : " + velocity);
        if (velocity > 0) // if Player is moving
        {
            float timeToMove = Vector3.Distance(transform.position, pullPoint) / velocity;
            Debug.Log(timeToMove);
            Invoke(nameof(stopPulling), timeToMove);
        }
        else // if something goees wrong
        {
            Invoke(nameof(stopPulling), 0.1f);
        }
    }


    private void stopPulling()
    {
        leftGearScript.canDrawRope = false;
        pm.isSwingingLeft = false;

        rightgearScript.canDrawRope = false;
        pm.isSwingingRight = false;
    }

}
