using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    // Update is called once per frame
    public float skyMoveSpeed;
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.deltaTime * skyMoveSpeed);
    }
}
