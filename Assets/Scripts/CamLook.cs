using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamLook : MonoBehaviour
{
    // Start is called before the first frame update

    public float sensitivity = 110f;


    float xRotation;
    float yRotation;

    public Transform orientation;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X")  * Time.deltaTime * sensitivity ;
        float mouseY = Input.GetAxis("Mouse Y")  * Time.deltaTime * sensitivity ;

        xRotation -=mouseY;
        yRotation +=mouseX;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

    }
}
