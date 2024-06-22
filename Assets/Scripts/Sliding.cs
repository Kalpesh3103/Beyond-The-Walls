using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    public Transform player;
    private Rigidbody rb;
    private MovementNew pm;
    public float slideForce;

    private float updatedSlidingForce;

    private float slideTimer;
    public float slideDuration;

    private float startYScale;
    private float slideYScale;

    float verticalInput;
    float horizontalInput;

    bool isSliding;

    // Start is called before the first frame update
    void Start()
    {
        pm = player.GetComponent<MovementNew>();
        rb = GetComponent<Rigidbody>();
        startYScale = player.transform.localScale.y;
        slideYScale = startYScale * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftControl) && (verticalInput != 0 || horizontalInput != 0))
        {
            startSlide();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) && isSliding)
        {
            stopSlide();
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                Debug.Log("Stopping slide in time : " + slideDuration);
                stopSlide();
            }
        }
    }

    void FixedUpdate()
    {
        if (isSliding)
        {
            slidingMovement();
        }
    }

    void startSlide()
    {
        isSliding = true;
        pm.isSliding = true;
        slideTimer = slideDuration;

        player.localScale = new Vector3(player.localScale.x, slideYScale, player.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    void slidingMovement()
    {
        Vector3 slidingDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(slidingDirection.normalized * slideForce, ForceMode.Force);
    }

    void stopSlide()
    {
        pm.isSliding = false;
        player.localScale = new Vector3(player.localScale.x, startYScale, player.localScale.z);
        // rb.velocity = new Vector3(0, 0, 0);

        Invoke(nameof(enableSlide), 0.1f);
    }

    void enableSlide()
    {
        isSliding = false;
    }
}
