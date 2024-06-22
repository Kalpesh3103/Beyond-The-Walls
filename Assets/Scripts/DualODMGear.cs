// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class DualODMGear : MonoBehaviour
// {

//     [Header("Referenes")]
//     public List<LineRenderer> lr;
//     public Transform player, cam;
//     public List<Transform> gunTips;
//     public LayerMask whatIsGrappleable;

//     [Header("Dual Swinging")]
//     public int totalGrapplePoints = 2;
//     public List<Transform> pointAimers;
//     private List<bool> hooksActive;

//     [Space]

//     [Header("Swinging")]
//     public float maxSwingDistance = 25f;
//     private List<Vector3> grapplePoints;
//     private List<SpringJoint> joints;

//     [Space]

//     [Header("Prediction")]
//     public List<RaycastHit> predictionHits;
//     public float predictionSphereRadius = 3f;
//     public List<Transform> predictionPoints;

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
//     private List<Vector3> currentGrapplePositions;

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

//         ListSetup();
//     }

//     private void ListSetup()
//     {
//         predictionHits = new List<RaycastHit>();
//         grapplePoints = new List<Vector3>();
//         joints = new List<SpringJoint>();
//         hooksActive = new List<bool>();
//         currentGrapplePositions = new List<Vector3>();

//         for (int i = 0; i < totalGrapplePoints; i++)
//         {
//             predictionHits.Add(new RaycastHit());
//             joints.Add(null);
//             grapplePoints.Add(Vector3.zero);
//             hooksActive.Add(false);
//             currentGrapplePositions.Add(Vector3.zero);
//         }

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (Input.GetKeyDown(grappleKey))
//         {
//             startSwing(0);
//             startSwing(1);
//         }

//         if (Input.GetKeyUp(grappleKey) && swinging)
//         {
//             stopSwing(0);
//             stopSwing(1);
//             // pullToPoint();
//         }

//         CheckForGrapplePoint();
//     }

//     private void LateUpdate()
//     {
//         drawRope();
//     }


//     private void startSwing(int swingIndex)
//     {
//         if (Vector3.zero == predictionHits[swingIndex].point) return;
//         // RaycastHit hit;
//         // if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
//         {
//             canDrawRope = true;

//             pm.isSwinging = true;
//             swinging = true;
//             grapplePoints[swingIndex] = predictionHits[swingIndex].point;
//             joints[swingIndex] = player.gameObject.AddComponent<SpringJoint>();
//             joints[swingIndex].autoConfigureConnectedAnchor = false;
//             joints[swingIndex].connectedAnchor = grapplePoints[swingIndex];

//             float distanceFromPoit = Vector3.Distance(player.position, grapplePoints[swingIndex]);

//             //The distance grapple will try to keep from grapple point
//             joints[swingIndex].maxDistance = distanceFromPoit * 0.8f;
//             joints[swingIndex].minDistance = distanceFromPoit * 0.25f;

//             joints[swingIndex].spring = 4.5f;
//             joints[swingIndex].damper = 7f;
//             joints[swingIndex].massScale = 4.5f;

//             // lr.positionCount = 2;
//             currentGrapplePositions[swingIndex] = gunTips[swingIndex].position;

//         }
//     }

//     private void stopSwing(int swingIndex)
//     {
//         pm.isSwinging = false;
//         // lr.positionCount = 0;
//         Destroy(joints[swingIndex]);
//         joints[swingIndex] = null;
//     }

//     private void drawRope()
//     {

//         // for (int point = 0; point < totalGrapplePoints; point++)
//         // {

//         if (!canDrawRope)
//         {
//             currentGrapplePositions[0] = gunTips[0].position;
//             currentGrapplePositions[1] = gunTips[1].position;

//             spring.Reset();
//             if (lr[0].positionCount > 0 && lr[1].positionCount > 0)
//             {
//                 lr[0].positionCount = 0;
//                 lr[1].positionCount = 0;
//             }
//             return;
//         }


//         if (lr[0].positionCount == 0 && lr[1].positionCount == 0)
//         {
//             spring.SetVelocity(velocity);
//             lr[0].positionCount = quality + 1;
//             lr[1].positionCount = quality + 1;

//         }

//         // if(!joint) return;
//         // if(lr.positionCount == 2)
//         {

//             spring.SetDamper(damper);
//             spring.SetStrength(strength);
//             spring.Update(Time.deltaTime);

//             var grapplePoint0 = this.grapplePoints[0];
//             var gunTipPosition0 = gunTips[0].position;
//             var up0 = Quaternion.LookRotation((grapplePoint0 - gunTipPosition0).normalized) * Vector3.up;

//             var grapplePoint1 = this.grapplePoints[1];
//             var gunTipPosition1 = gunTips[1].position;
//             var up1 = Quaternion.LookRotation((grapplePoint1 - gunTipPosition1).normalized) * Vector3.up;


//             currentGrapplePositions[0] = Vector3.Lerp(currentGrapplePositions[0], grapplePoint0, Time.deltaTime * 12f);
//             currentGrapplePositions[1] = Vector3.Lerp(currentGrapplePositions[1], grapplePoint1, Time.deltaTime * 12f);


//             for (var i = 0; i < quality + 1; i++)
//             {
//                 var delta0 = i / (float)quality;
//                 var delta1 = i / (float)quality;


//                 var offset0 = up0 * waveHeight * Mathf.Sin(delta0 * waveCount * Mathf.PI) * spring.Value *
//                             affectCurve.Evaluate(delta0);
//                 var offset1 = up1 * waveHeight * Mathf.Sin(delta1 * waveCount * Mathf.PI) * spring.Value *
//                                         affectCurve.Evaluate(delta1);

//                 lr[0].SetPosition(i, Vector3.Lerp(gunTipPosition0, currentGrapplePositions[0], delta0) + offset0);
//                 lr[1].SetPosition(i, Vector3.Lerp(gunTipPosition1, currentGrapplePositions[1], delta1) + offset1);


//             }


//             // lr.SetPosition(0, gunTip.position);
//             // lr.SetPosition(1, swingPoint);
//             // }
//         }
//     }


//     private void destroyRope()
//     {
//         for (int i = 0; i < totalGrapplePoints; i++)
//         {
//             currentGrapplePositions[i] = gunTips[i].position;
//             spring.Reset();
//             if (lr[i].positionCount > 0)
//                 lr[i].positionCount = 0;
//             continue;                 //Need to check here : Before it was "return;"
//         }
//     }

//     // private void pullToPoint()
//     // {
//     //     swinging = false;
//     //     float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
//     //     Vector3 directionToPoint = (grapplePoint - transform.position).normalized;

//     //     rb.velocity = directionToPoint * moveSpeed;

//     //     if (rb.velocity.magnitude > 0)
//     //     {
//     //         float timeToMove = distanceFromPoint / rb.velocity.magnitude;

//     //         Invoke(nameof(stopPulling), timeToMove);
//     //     }
//     //     else
//     //     {
//     //         Invoke(nameof(stopPulling), 0.1f);
//     //     }

//     // }

//     // private void stopPulling()
//     // {
//     //     canDrawRope = false;
//     //     pm.isSwinging = false;
//     //     // destroyRope();
//     // }


//     private void CheckForGrapplePoint()
//     {

//         // for (int i = 0; i < totalGrapplePoints; i++)
//         // {
//         if (hooksActive[0] && hooksActive[1]) { /* Do Nothing */ }
//         else
//         {
//             RaycastHit sphereCastHit0;
//             Physics.SphereCast(pointAimers[0].position, predictionSphereRadius, pointAimers[0].forward, out sphereCastHit0, maxSwingDistance, whatIsGrappleable);
//             RaycastHit sphereCastHit1;
//             Physics.SphereCast(pointAimers[1].position, predictionSphereRadius, pointAimers[1].forward, out sphereCastHit1, maxSwingDistance, whatIsGrappleable);

//             RaycastHit raycastHit0;
//             Physics.Raycast(cam.position, cam.forward, out raycastHit0, maxSwingDistance, whatIsGrappleable);

//             RaycastHit raycastHit1;
//             Physics.Raycast(cam.position, cam.forward, out raycastHit1, maxSwingDistance, whatIsGrappleable);


//             Vector3 realHitPoint0;
//             Vector3 realHitPoint1;


//             if (Vector3.zero != raycastHit0.point && Vector3.zero != raycastHit1.point)
//             {
//                 realHitPoint0 = raycastHit0.point;
//                 realHitPoint1 = raycastHit1.point;

//             }
//             else if (Vector3.zero != sphereCastHit0.point && Vector3.zero != sphereCastHit1.point)
//             {
//                 realHitPoint0 = sphereCastHit0.point;
//                 realHitPoint1 = sphereCastHit1.point;

//             }
//             else
//             {
//                 realHitPoint0 = Vector3.zero;
//                 realHitPoint1 = Vector3.zero;

//             }

//             if (realHitPoint0 != Vector3.zero && realHitPoint1 != Vector3.zero)
//             {
//                 predictionPoints[0].gameObject.SetActive(true);
//                 predictionPoints[0].position = realHitPoint0;

//                 predictionPoints[1].gameObject.SetActive(true);
//                 predictionPoints[1].position = realHitPoint1;
//             }
//             else
//             {
//                 predictionPoints[0].gameObject.SetActive(false);
//                 predictionPoints[1].gameObject.SetActive(false);

//             }

//             predictionHits[0] = Vector3.zero != raycastHit0.point ? raycastHit0 : sphereCastHit0;

//             predictionHits[1] = Vector3.zero != raycastHit1.point ? raycastHit1 : sphereCastHit1;
//         }
//         // }


//     }

// }

