using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys any object comes in contact
/// </summary>
/// 
/// Author: Chamod Welhenge
/// 
public class Death : MonoBehaviour
{
    /// <summary>
    /// If a collision trigger has detected
    /// </summary>
    /// <param name="other">Object collided</param>
    private void OnTriggerEnter(Collider other)
    {
        // destroy that game object
        Destroy(other.gameObject);
    }


}

