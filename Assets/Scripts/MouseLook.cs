using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // Start is called before the first frame update



    public Transform playerBody;

    public float sensitivity = 100f;

    public float xRotation = 0f;

    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime ;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime ;

        xRotation -= mouseY;
        xRotation = Math.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); //look up&down

        playerBody.Rotate(Vector3.up*mouseX); //look left&right

        
    }
}
