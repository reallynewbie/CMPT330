/*
 * Using code from the following:
 *  https://forum.unity.com/threads/attack-combo-system-help-with-c.241166/
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This part protects the program by making it require the following
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]

/*
 * Capsule collider?
 * A grounded collision involves surrounding the object in some kind of
 * collider and letting the physics collision handler deal with it.
 * Since we have a capsule, we may as well use it
 */

public class TPC_Control_Test : MonoBehaviour
{
    // Test constants
    private const float OLD_SPEED = 0.85f;
    private const float NEW_SPEED = 1f - OLD_SPEED;

    // States
    public enum TPC_State {ON_GROUND, IN_AIR, ATTACK1, ATTACK2, ATTACK3};
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
    public float TURN_SPEED = 180f; // Turning speed per second
    public float RUN_SPEED = 5f; // Move speed per second
    public float JUMP_SPEED = 4f; // Jump initial vertical speed
    public float HYSTERESIS = 0.25f;

    // Public Variables
    public TPC_State state;
    private bool isGrounded = true;
    private Rigidbody physics;
    private Vector3 groundNormal = Vector3.up;
    private Transform body;
    private Vector3 bodyCenter;
    private float capsuleHeight;
    private Vector3 capsuleCenter;
    private CapsuleCollider capsule;
    private Animator animator;
    private Vector3 speed;

    // Combo ideas
    private bool ActivateTimerToReset = false;
    public float currentComboTimer;
    public int currentComboState = 0;
    private float origTimer;

    // Start is called before the first frame update
    void Start()
    {
        // Here we grab CapsuleCollider + initial data
        capsule = GetComponent<CapsuleCollider>();
        capsuleHeight = capsule.height;
        capsuleCenter = capsule.center;

        physics = transform.GetComponent<Rigidbody>(); // Gets the Rigidbody
        physics.constraints = RigidbodyConstraints.FreezeRotationX | 
            RigidbodyConstraints.FreezeRotationY | 
            RigidbodyConstraints.FreezeRotationZ;
        // In the above, we make it so that Rigidbody can't be 
        // rotated by the physics engine; we want to do that ourselves

        state = TPC_State.ON_GROUND;

        bodyCenter = transform.localPosition;

        animator = gameObject.GetComponent<Animator>();

        animator.SetFloat("Speed", 0f);
        animator.SetInteger("State", (int)TPC_State.ON_GROUND);
        animator.SetBool("Dead", false);
        animator.SetBool("Dying", false);

        // Combo Stuff
        // Store original timer reset duration
        origTimer = currentComboTimer;
    }

    // Use FixedUpdate, not Update, because we're using the physics engine
    void FixedUpdate()
    {
        bool isAttacking = Input.GetButton("Fire1");
        bool isAttack2 = Input.GetButton("Fire2");
        animator = gameObject.GetComponent<Animator>();

        // Combo stuff
        NewComboSystem();
        //Initially set to false, so the method won't start
        ResetComboState(ActivateTimerToReset);

        /*
         * A switch is good because it keeps actions separate
         * Don't have to worry about a lot of boolean variables
         * And it makes coding and testing easier
         * Since we're working with the buttons a lot (transition conditions)
         * we get them once and store them in temp variables
         */
        switch (state)
        {
            case TPC_State.ON_GROUND: // Handle actions possible on the ground
                // If falling, just set isGrounded and change states
                // Return prevents any further processing
                // Motion keeps cur velocity and physics engine will cause player to drop to ground
                if (!isGrounded)
                {
                    state = TPC_State.IN_AIR;
                    return;
                }
                // Need to make sure state transition is valid
                // + need to add a Y velocity component to get jump going
                if (isGrounded && isAttacking)
                {
                    state = TPC_State.ATTACK1;
                    animator.SetInteger("State", (int)state);
                    return;
                }

                // When moving we need to rotate out object
                // transform.rotate is easier to deal with than rigidBody.SetTorque
                // Only rotating about the Y axis so X and Z = 0
                transform.Rotate(0, Input.GetAxis("Turn") * TURN_SPEED * Time.deltaTime, 0);
                SetSpeed (RUN_SPEED);

                physics.AddForce(Vector3.down * physics.mass * 10f); // Force char to the ground

                break;

            case TPC_State.IN_AIR: // In the air, can't do much
                if (isGrounded)
                    state = TPC_State.ON_GROUND;
                break;

            case TPC_State.ATTACK1: // The mage does the first of his attacks
                animator.SetInteger("State", (int)state);
                /*
                if (isAttack2)
                {
                    StartCoroutine(Attack2());
                }
                */
                state = TPC_State.ON_GROUND;
                animator.SetInteger("State", (int)state);
                break;

        }
    }

    /*
    private IEnumerator Attack2()
    {
        Debug.Log("Here we are!");
        state = TPC_State.ATTACK1;
        animator.SetInteger("State", (int)state);

        yield return new WaitForSeconds(1f);

        state = TPC_State.ON_GROUND;
        animator.SetInteger("State", (int)state);
    }
    */

    private void SetSpeed (float v)
    {
        float vSpeed = physics.velocity.y;

        animator = gameObject.GetComponent<Animator>();

        Vector3 targetSpeed = Input.GetAxis("Forward") * v * transform.forward;
        Vector3 newSpeed = speed * OLD_SPEED + targetSpeed * NEW_SPEED;

        speed = ((newSpeed - targetSpeed).sqrMagnitude > 0.0001f) ?
            Vector3.ProjectOnPlane(newSpeed, groundNormal) :
            Vector3.ProjectOnPlane(targetSpeed, groundNormal);

        // Vertical speed
        speed.y = vSpeed;

        physics.velocity = speed;
        animator.SetFloat("Speed", speed.magnitude / v);
    }

    void ResetComboState(bool resetTimer)
    {
        if (resetTimer)
        //if the bool that you pass to the method is true
        // (aka if ActivateTimerToReset is true, then the timer start
        {
            currentComboTimer -= Time.deltaTime;
            //If the parameter bool is set to true, a timer start, when the timer
            //runs out (because you don't press fast enought Z the second time)
            //currentComboState is set again to zero, and you need to press it twice again
            if (currentComboTimer <= 0)
            {
                currentComboState = 0;
                ActivateTimerToReset = false;
                currentComboTimer = origTimer;
            }
        }
    }

    void NewComboSystem()
    {
        if (Input.GetButton("Fire1"))
        {

            //No need to create a comboStateUpdate()
            //function while you can directly
            //increment a variable using ++ operator
            currentComboState++;

            //Okay, you pressed Z once, so now the resetcombostate Function is
            //set to true, and the timer starts to reset the currcombostate
            ActivateTimerToReset = true;

            //Note that I'm to lazy to setup a switch statement
            //that would be WAY more readable than 3 if's in a row
            if (currentComboState == 1)
            {

                Debug.Log("1 hit");
            }
            if (currentComboState == 2)
            {
                Debug.Log("2 hit, The combo Should Start");
            }
            if (currentComboState >= 3)
            {
                Debug.Log("Whooaaa 3 hits in half a second!");
            }
        }
    }

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
                foreach(ContactPoint pt in collision.contacts)
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
