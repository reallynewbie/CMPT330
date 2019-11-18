using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Adds forwards and backwards wobble to the boat
/// </summary>
/// 
/// Source: https://gamedev.stackexchange.com/questions/96878/how-to-animate-objects-with-bobbing-up-and-down-motion-in-unity/96880
/// Author: Chamod Welhenge
/// 
/// Field           Description
/// originalX       Original X position of the object
/// 
public class BoatMovement : MonoBehaviour
{
    float originalX;

    // Store the original X
    void Start(){
        // original X position
        originalX = transform.rotation.x;
    }

    /// <summary>
    ///  Rotates the object every frame
    /// </summary>
    void Update(){
        // Rotation value based on original X and the strength of the rotation and Sine of time
        float rotationValue = originalX + ((float)Math.Sin(Time.time) * 0.01f);
        // Set the object rotation after the new rotation and current y and z values
        transform.rotation =new Quaternion(rotationValue, transform.rotation.y, transform.rotation.z, 1f);     
    }


}
