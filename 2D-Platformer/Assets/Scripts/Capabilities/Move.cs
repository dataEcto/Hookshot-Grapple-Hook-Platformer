using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private InputController inputController = null;
    [SerializeField, Range(0f,100f)] private float maxSpeed = 15f;
    [SerializeField, Range(0f,100f)] private float maxAcceleration = 35f;
    [SerializeField, Range(0f,100f)] private float maxAirAcceleration = 20f;
    // Wall Stick Time determines how long we stay on a wall before we fall off
    // This is so we can wait for the Wall Leap off input.
    // Since Wall Leap is a hold away from wall input, we want to let our play have some leeway
    // I bet i can use another version of this variable to imitate Celeste 2 wall stick after grappling...
    // Maybe by having it check the grappling script to see if its grappled, the collison data script to see if were at wall
    // And then setting wall stick to true?
    [SerializeField, Range (0.05F,0.5F)] private float wallStickTime = 0.25f;

    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Vector2 currentVelocity;
    private Rigidbody2D _rb2d;
    private CollisionDataRetriever collisionDataRetriever;
    private WallInteractor wallInteractorScript;

    // How fast our speed will increase
    private float maxSpeedChange;
    private float acceleration;
    private float wallStickCounter;
    private bool onGroundThis;


    // Start is called before the first frame update
    void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        collisionDataRetriever = GetComponent<CollisionDataRetriever>();
        wallInteractorScript = GetComponent<WallInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        // We retrieve direction from our Generic Input Class
        direction.x = inputController.RetrieveMoveInput();
        // Our velocity is made up of the direction we want to go mulitplied by max speed - any potential friction
        desiredVelocity = new Vector2(direction.x,0f) * Mathf.Max(maxSpeed - collisionDataRetriever.GetFrictionValue(),0f);
    }

    private void FixedUpdate()
    {
        // Set our OnGroundThis bool to the one in the GroundCheck Script;
        onGroundThis = collisionDataRetriever.GetOnGroundValue();
        currentVelocity = _rb2d.velocity;

        // Set acceleration based on the onGround variable
        // If we are grounded we use normal acceleration
        // If we aren't, we use air
        acceleration = onGroundThis ? maxAcceleration: maxAirAcceleration;
        // Where acceleration is calculated
        maxSpeedChange = acceleration * Time.deltaTime;
        // Actual movement
        currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, desiredVelocity.x, maxSpeedChange);



        # region Wall Stick
        // If we are on a wall, not touching the ground, and not Mario Wall Jumping
        if (collisionDataRetriever.onWall && !collisionDataRetriever.onGround && !wallInteractorScript.WallJumping)
        {
            if (wallStickCounter > 0)
            {
                //Prevent us from moving away from the wall
                currentVelocity.x = 0;

                // If we point away from the wall...
                if (inputController.RetrieveMoveInput() == collisionDataRetriever.contactNormal.x)
                {
                    wallStickCounter -= Time.deltaTime;
                }
                // if there is no movement, reset the counter. this way. movement will be blocked until the coutner reaches 0
                else
                {
                    wallStickCounter = wallStickTime;
                }
            }
            // Reset wall stick counter in other solutions, just in case
            else
            {
                wallStickCounter = wallStickTime;
            }
        }
        # endregion

                // Set the rigidbodys velocity to this new velocity;
        _rb2d.velocity = currentVelocity;
    }
}
