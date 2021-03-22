using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCamera : MonoBehaviour
{
    public float movementSpeed = 10; //regular move speed
    public float movementSpeedMultiplier = 2; //left/right shift to go fast

    public float mouselookSensitivity = 3; //freelook sensitivity

    /// <summary>
    /// Update, runs and gets called every frame
    /// </summary>
    private void Update()
    {
        //if we are not pressing down right mouse button then don't continue
        if (!Input.GetButton("Fire2"))
            return;

        //our main speed
        float speed = movementSpeed * Time.deltaTime;

        //multiply our speed
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            speed *= movementSpeedMultiplier;

        //get our input axis (float values between -1, and 1 depending on what keys are pressed)
        float laterialMultiplier = Input.GetAxis("Horizontal") * speed;
        float forwardMultiplier = Input.GetAxis("Vertical") * speed;
        float verticalMultiplier = GetAxis_UpDown() * speed;

        //move the camera (based on the axis of the object so it feels natural)
        transform.position += laterialMultiplier * transform.right;
        transform.position += forwardMultiplier * transform.forward;
        transform.position += verticalMultiplier * Vector3.up;

        //freelook
        float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouselookSensitivity;
        float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * mouselookSensitivity;
        transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
    }

    /// <summary>
    /// A quick wrapper for the Q and E keys to get an Input.GetAxis style value.
    /// </summary>
    /// <returns></returns>
    private float GetAxis_UpDown()
    {
        //start at zero since no keys are pressed
        float value = 0;

        if (Input.GetKey(KeyCode.Q)) //going down, so the value is -1
        {
            value = -1;
        }
        else if (Input.GetKey(KeyCode.E)) //going up, so the value is 1
        {
            value = 1;
        }

        return value;
    }
}
