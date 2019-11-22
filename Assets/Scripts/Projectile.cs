using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves projectile and deals damage to player
/// </summary>
/// 
/// Source: https://www.youtube.com/watch?v=wkKsl1Mfp5M
/// 
/// Field           Description
/// speed           Speed of the projectile
/// 
/// Author: Chamod Welhenge
/// 

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the rigidbody of bullet
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }



    /// <summary>
    /// Deals damage to object it hits
    /// </summary>
    /// <param name="collision">Object being collided with</param>
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
    }

}