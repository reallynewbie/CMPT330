using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]

public class TPC_Control_Mage : TPC_Control_Root
{
    public enum TPC_State { ON_GROUND, IN_AIR, ATTACK1, ATTACK2 };

    //Critical Constants
    public float TURN_SPEED = 180f; // Turning speed per second
    public float RUN_SPEED = 5f; // Move speed per second
    public float HYSTERESIS = 0.25f;

    private TPC_State state;
    private Animator animator;

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
    }

    // Use FixedUpdate, not Update, because we're using the physics engine
    void FixedUpdate()
    {
        bool isWalking = Input.GetButton("Walk");
        bool isAttacking = Input.GetButton("Fire1");
        bool isAttack2 = Input.GetButton("Fire2");
        animator = gameObject.GetComponent<Animator>();

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
                SetSpeed(RUN_SPEED);
                //SetSpeed(isWalking ? WALK_SPEED : RUN_SPEED);

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

    public override void SetSpeed(float v)
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
}
