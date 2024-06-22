using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ODMGearLeft : MonoBehaviour
{

    [Header("References")]
    public Transform rightGear;
    public Transform player, gunTip, cam;
    public LineRenderer lr;
    public LayerMask whatIsGrappleable;

    [Header("RopeAnimation")]
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve affectCurve;
    private Spring spring;
    private Vector3 currentGrapplePosition;

    private KeyCode grappleKey = KeyCode.Mouse0;
    public bool swingingLeft;
    private MovementNew pm;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereRadius = 3f;
    public Transform predictionPoint;

    [Header("Swinging")]
    public float maxSwingDistance = 100f;
    private Vector3 swingPoint;
    private SpringJoint joint;

    private ODMGearRight rightGearScript;
    private bool canDrawRope;

    void Awake()
    {
        spring = new Spring();
        spring.SetTarget(0);
    }
    private void Start()
    {
        rightGearScript = rightGear.GetComponent<ODMGearRight>();
        pm = player.GetComponent<MovementNew>();
    }

    void Update()
    {
        if (Input.GetKeyDown(grappleKey)) startSwing();

        if (Input.GetKeyUp(grappleKey) && swingingLeft)
        {
            stopSwing();
        }

        CheckForGrapplePoint();

    }
    private void LateUpdate()
    {
        drawRope();
    }

    private void startSwing()
    {
        if (Vector3.zero == predictionHit.point) return;

        canDrawRope = true;
        pm.isSwingingLeft = true;
        swingingLeft = true;
        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoit = Vector3.Distance(player.position, swingPoint);

        //The distance grapple will try to keep from grapple point
        joint.maxDistance = distanceFromPoit * 0.8f;
        joint.minDistance = distanceFromPoit * 0.25f;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        // lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;

    }

    private void stopSwing()
    {
        Destroy(joint);
        joint = null;

        canDrawRope = false;
        pm.isSwingingLeft = false;
    }

    private void CheckForGrapplePoint()
    {
        if (null != joint) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(gunTip.position, predictionSphereRadius, gameObject.transform.forward, out sphereCastHit, maxSwingDistance, whatIsGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(gunTip.position, gameObject.transform.forward, out raycastHit, maxSwingDistance, whatIsGrappleable);

        Vector3 realHitPoint;

        if (Vector3.zero != raycastHit.point)
        {
            realHitPoint = raycastHit.point;
        }
        else if (Vector3.zero != sphereCastHit.point)
        {
            realHitPoint = sphereCastHit.point;
        }
        else
        {
            realHitPoint = Vector3.zero;
        }

        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }


        predictionHit = Vector3.zero != raycastHit.point ? raycastHit : sphereCastHit;

    }

    private void drawRope()
    {
        if (!canDrawRope)
        {
            currentGrapplePosition = gunTip.position;
            spring.Reset();
            if (lr.positionCount > 0) lr.positionCount = 0;
            return;
        }

        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var grapplePoint = swingPoint;
        var gunTipPosition = gunTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        for (var i = 0; i < quality + 1; i++)
        {
            var delta = i / (float)quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                        affectCurve.Evaluate(delta);

            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }
    }


}

