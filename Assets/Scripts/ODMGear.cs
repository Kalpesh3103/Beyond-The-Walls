// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ODMGear : MonoBehaviour
// {

//     [Header("Referenes")]
//     public LineRenderer lr;
//     public Transform gunTip, player, cam;
//     public LayerMask whatIsGrappleable;

//     [Header("Swinging")]
//     public float maxSwingDistance = 25f;
//     private Vector3 swingPoint;
//     private SpringJoint joint;

//     [Header("Prediction")]
//     public RaycastHit predictionHit;
//     public float predictionSphereRadius = 3f;
//     public Transform predictionPoint;

//     [Header("Input")]
//     public KeyCode grappleKey = KeyCode.Mouse0;
//     public float moveSpeed;

//     [Header("GrapplingRope")]
//     public int quality;
//     public float damper;
//     public float strength;
//     public float velocity;
//     public float waveCount;
//     public float waveHeight;
//     public AnimationCurve affectCurve;
//     private Spring spring;
//     private Vector3 currentGrapplePosition;

//     private MovementNew pm;
//     private Rigidbody rb;
//     private bool swinging;
//     private bool canDrawRope;

//     void Awake()
//     {
//         spring = new Spring();
//         spring.SetTarget(0);
//     }
//     // Start is called before the first frame update
//     void Start()
//     {
//         // lr.positionCount = 0;
//         pm = player.GetComponent<MovementNew>();
//         rb = pm.GetComponent<Rigidbody>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (Input.GetKeyDown(grappleKey)) startSwing();

//         if (Input.GetKeyUp(grappleKey) && swinging)
//         {
//             stopSwing();
//             pullToPoint();
//         }

//         CheckForGrapplePoint();
//     }

//     private void LateUpdate()
//     {
//         drawRope();
//     }


//     private void startSwing()
//     {
//         if (Vector3.zero == predictionHit.point) return;
//         // RaycastHit hit;
//         // if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
//         {
//             canDrawRope = true;

//             pm.isSwinging = true;
//             swinging = true;
//             swingPoint = predictionHit.point;
//             joint = player.gameObject.AddComponent<SpringJoint>();
//             joint.autoConfigureConnectedAnchor = false;
//             joint.connectedAnchor = swingPoint;

//             float distanceFromPoit = Vector3.Distance(player.position, swingPoint);

//             //The distance grapple will try to keep from grapple point
//             joint.maxDistance = distanceFromPoit * 0.8f;
//             joint.minDistance = distanceFromPoit * 0.25f;

//             joint.spring = 4.5f;
//             joint.damper = 7f;
//             joint.massScale = 4.5f;

//             // lr.positionCount = 2;
//             currentGrapplePosition = gunTip.position;

//         }
//     }

//     private void stopSwing()
//     {
//         pm.isSwinging = false;
//         // lr.positionCount = 0;
//         Destroy(joint);
//         joint = null;
//     }

//     private void drawRope()
//     {

//         if (!canDrawRope)
//         {
//             currentGrapplePosition = gunTip.position;
//             spring.Reset();
//             if (lr.positionCount > 0)
//                 lr.positionCount = 0;
//             return;
//         }


//         if (lr.positionCount == 0)
//         {
//             spring.SetVelocity(velocity);
//             lr.positionCount = quality + 1;
//         }

//         // if(!joint) return;
//         // if(lr.positionCount == 2)
//         {



//             spring.SetDamper(damper);
//             spring.SetStrength(strength);
//             spring.Update(Time.deltaTime);

//             var grapplePoint = swingPoint;
//             var gunTipPosition = gunTip.position;
//             var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

//             currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

//             for (var i = 0; i < quality + 1; i++)
//             {
//                 var delta = i / (float)quality;
//                 var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
//                             affectCurve.Evaluate(delta);

//                 lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
//             }


//             // lr.SetPosition(0, gunTip.position);
//             // lr.SetPosition(1, swingPoint);
//         }
//     }


//     private void destroyRope()
//     {
//         currentGrapplePosition = gunTip.position;
//         spring.Reset();
//         if (lr.positionCount > 0)
//             lr.positionCount = 0;
//         return;

//     }

//     private void pullToPoint()
//     {
//         swinging = false;
//         float distanceFromPoint = Vector3.Distance(player.position, swingPoint);
//         Vector3 directionToPoint = (swingPoint - transform.position).normalized;

//         rb.velocity = directionToPoint * moveSpeed;

//         if (rb.velocity.magnitude > 0)
//         {
//             float timeToMove = distanceFromPoint / rb.velocity.magnitude;

//             Invoke(nameof(stopPulling), timeToMove);
//         }
//         else
//         {
//             Invoke(nameof(stopPulling), 0.1f);
//         }

//     }

//     private void stopPulling()
//     {
//         canDrawRope = false;
//         pm.isSwinging = false;
//         // destroyRope();
//     }


//     private void CheckForGrapplePoint()
//     {

//         if (null != joint) return;

//         RaycastHit sphereCastHit;
//         Physics.SphereCast(cam.position, predictionSphereRadius, cam.forward, out sphereCastHit, maxSwingDistance, whatIsGrappleable);

//         RaycastHit raycastHit;
//         Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, whatIsGrappleable);


//         Vector3 realHitPoint;

//         if (Vector3.zero != raycastHit.point)
//         {
//             realHitPoint = raycastHit.point;
//         }
//         else if (Vector3.zero != sphereCastHit.point)
//         {
//             realHitPoint = sphereCastHit.point;
//         }
//         else
//         {
//             realHitPoint = Vector3.zero;
//         }

//         if (realHitPoint != Vector3.zero)
//         {
//             predictionPoint.gameObject.SetActive(true);
//             predictionPoint.position = realHitPoint;
//         }
//         else
//         {
//             predictionPoint.gameObject.SetActive(false);
//         }


//         predictionHit = Vector3.zero != raycastHit.point ? raycastHit : sphereCastHit;
//     }

// }
