using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This part protects the program by making it require the following
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

/*
 * Capsule collider?
 * A grounded collision involves surrounding the object in some kind of
 * collider and letting the physics collision handler deal with it.
 * Since we have a capsule, we may as well use it
 */

public abstract class TPC_Control_Root : MonoBehaviour
{

    /* 
     * We create an enum for our states:
     *       * Easier to understand
     *       * Better for documentation
     *       * Can use enum in a switch statement
     * The above enum and TPC_State state below are public right now
     * for testing. Once the code is working and accepted, they'll be
     * turned into private variables
     */
    //Critical Constants
    // public float TURN_SPEED; // Turning speed per second
    // public float RUN_SPEED; // Move speed per second
    // public float HYSTERESIS;
    protected const float OLD_SPEED = 0.85f;
    protected const float NEW_SPEED = 1f - OLD_SPEED;

    // Public Variables
    protected bool isGrounded = true;
    protected Rigidbody physics;
    protected Vector3 groundNormal = Vector3.up;
    protected Transform body;
    protected Vector3 bodyCenter;
    protected float capsuleHeight;
    protected Vector3 capsuleCenter;
    protected CapsuleCollider capsule;
    protected Vector3 speed;

    // Start is called before the first frame update

    // SetSpeed is HERE!
    // Abtsract to be used in controller scripts using this abstract
    public abstract void SetSpeed(float v);

    /*
     * These methods are to handle collision events
     * 
     * OnCollisionEnter is executed when you first enter a collider.
     * We'll want to make sure we only handle terrain objects though. Side
     * collisions with other objects will need to be handled in other ways.
     * The easiest way is to use tags for groups of objects.
     * 
     * OnCollisionExit is called when you leave a collision in a given frame
     * All we need to do is set isGround to false
     */
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Terrain":
                // Calculate avg of ground contact point surface normals
                groundNormal = Vector3.zero;
                foreach (ContactPoint pt in collision.contacts)
                {
                    groundNormal += pt.normal;
                }
                groundNormal.Normalize();
                // We averaged normals to create the surface normal

                isGrounded = true;
                break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Terrain":
                isGrounded = false;
                break;
        }
    }
    private void OnCollisionStay(Collision collision) { }
}
