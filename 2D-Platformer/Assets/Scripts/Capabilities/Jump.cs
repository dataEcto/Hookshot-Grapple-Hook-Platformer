using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Jump : MonoBehaviour
{
    [SerializeField] private InputController playerInput = null;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 3f;
    [SerializeField, Range(0,5)] private int maxAirJumps = 0;
    // The value of the gravity scale our character will change to when going down
    // aka how fast the character will fall
    [SerializeField, Range(0F,5F)] private float downwardMovementMultiplier = 3f;
    // Same principle as above, just upwards
    // aka how fast the character will reach the peak
    [SerializeField, Range (0f,5f)] private float upwardMovementMultiplier = 3f;
    [SerializeField, Range (0f,0.3f)] private float coyoteTime = 0.2f;
    [SerializeField, Range (0f, 0.3f)] private float jumpBufferTime = 0.2f;

    private Rigidbody2D _rb2d;
    private CollisionDataRetriever groundCheck;
    private Vector2 velocity;

    private int currentJumpAmount;

    // We save this scale just in case
    private float defaultGravityScale;
    private float coyoteCounter;
    private float jumpBufferCounter;

    private bool wantsJump;
    private bool onGroundThis;
    private bool isJumping;

    void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<CollisionDataRetriever>();

        // This value is applied when the character is on the ground
        defaultGravityScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        // This Or Operator lets this remain set, even in a new update cycle
        // Until we manually set it to false 
        wantsJump |= playerInput.RetrieveJumpInput();
    }

    private void FixedUpdate()
    {
        onGroundThis = groundCheck.GetOnGroundValue();
        velocity = _rb2d.velocity;

        // If we are grounded, reset our air jumps
        // We also want to make sure we are truly grounded, so we check that our y velocity is 0
        if (onGroundThis && _rb2d.velocity.y == 0)
        {
            currentJumpAmount = 0;
            // Set the Coyote Time back to its max
            coyoteCounter = coyoteTime;
            isJumping = false;
        }
        else
        {
            // If we aren't grounded, we must be in the air
            // Start up Coyote Time then
            // in JumpAction(), we check if coyotetime is greater than 0
            // And if it is, then we can still jump
            coyoteCounter -= Time.deltaTime;
        }

        // Then check if we want a jump

        if (wantsJump)
        {
            wantsJump = false;
            //JumpAction();
            jumpBufferCounter = jumpBufferTime;
        }
        else if ( !wantsJump && jumpBufferTime > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0)
        {
            JumpAction();
        }

        // If we are going up/holding the jump button...
        if (playerInput.RetrieveJumpHoldInput() && _rb2d.velocity.y > 0)
        {
            // Apply our upwardmovement multplyer
            _rb2d.gravityScale = upwardMovementMultiplier;
        }
        // Going down/not holding the jump button...
        else if (!playerInput.RetrieveJumpHoldInput() || _rb2d.velocity.y < 0)
        {
            _rb2d.gravityScale = downwardMovementMultiplier;
        }
        // Otherwise, set to default
        else if (_rb2d.velocity.y == 0)
        {
            _rb2d.gravityScale = defaultGravityScale;
        }

        // And at the very end we apply our velocity to the rigidBody Velocity

        _rb2d.velocity = velocity;
    }

    /// <summary>
    /// This is where Jumping is calculated and then happens
    /// </summary>
    private void JumpAction()
    {
        if (coyoteCounter > 0F || currentJumpAmount < maxAirJumps && isJumping)
        {
            Debug.Log("Jump");

            if (isJumping)
            {
                currentJumpAmount += 1;
            }
            
            jumpBufferCounter = 0;
            coyoteCounter = 0;
            // Adjusted this based on a YT Comment
            // float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight);
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight * upwardMovementMultiplier);
            isJumping = true;

            // Make sure our jump speed never goes negative
            if (velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed = velocity.y, 0f);
            }
            
            // Add Jump Speed to velocity
            // velocity.y += jumpSpeed;
            velocity.y = jumpSpeed;
            
        }
    }
}

