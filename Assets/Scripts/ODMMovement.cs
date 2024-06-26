using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ODMMovement : MonoBehaviour
{
    [Header("References")]
    public Transform leftGear;
    public Transform rightGear;
    public Camera cam;
    public VisualEffect speedEffect;


    public float pullForce = 100f;

    private MovementNew pm;
    private Rigidbody rb;
    private ODMGearLeft leftGearScript;
    private ODMGearRight rightgearScript;
    private Vector3 pullPoint;

    private float defaultFov = 80f;
    private float targetFov = 100f;
    public float duration = 1.0f; // Duration of the transition

    private Coroutine currentCoroutine;
    private float velocity = 0.0f;
    public float smoothTime = 0.3f; // Smoothing duration

    private void Start()
    {
        speedEffect.Stop();
        pm = transform.GetComponent<MovementNew>();
        rb = transform.GetComponent<Rigidbody>();
        leftGearScript = leftGear.GetComponent<ODMGearLeft>();
        rightgearScript = rightGear.GetComponent<ODMGearRight>();
    }


    public void pullToPoint()
    {
        speedEffect.Play();
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ChangeFOV(cam.fieldOfView, targetFov, duration));
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
        directionToPoint.y += 10f * Time.deltaTime; //adding upward force;

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
        speedEffect.Stop();
        leftGearScript.canDrawRope = false;
        pm.isSwingingLeft = false;

        rightgearScript.canDrawRope = false;
        pm.isSwingingRight = false;



        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ChangeFOV(cam.fieldOfView, defaultFov, duration));
    }


    private IEnumerator ChangeFOV(float fromFOV, float toFOV, float duration)
    {


        //  float elapsed = 0f;
        // while (elapsed < duration)
        // {
        //     cam.fieldOfView = Mathf.Lerp(fromFOV, toFOV, elapsed / duration);
        //     elapsed += Time.deltaTime;
        //     yield return null;
        // }
        // cam.fieldOfView = toFOV; // Ensure the final value is set

        float currentFOV = fromFOV;
        while (Mathf.Abs(currentFOV - toFOV) > 0.01f)
        {
            currentFOV = Mathf.SmoothDamp(currentFOV, toFOV, ref velocity, smoothTime);
            cam.fieldOfView = currentFOV;
            yield return null;
        }
        cam.fieldOfView = toFOV; // Ensure the final value is set
    }
}
