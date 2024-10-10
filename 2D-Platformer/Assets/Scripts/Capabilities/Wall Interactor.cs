using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInteractor : MonoBehaviour
{
    public bool WallJumping {get; private set;}

   
    [Header("Wall Slide")]
    [SerializeField]
    [Range(0.1f, 5f)] private float _wallSlideMaxSpeed = 2f;


    [Header("Wall Jump")]
    // Typical Mega Man X Wall Jump
    [SerializeField] private Vector2 wallJumpClimb = new Vector2(4f,12f);
    // Typical Mario Wall Jump
    [SerializeField] private Vector2 wallJumpBounce = new Vector2(10.7f, 10f);
    // Allow for traversal with Distance Walls
    // Unlike Mario, you hold away from the wall
    [SerializeField] private Vector2 wallJumpLeap = new Vector2(14f, 12f);

    [SerializeField] private InputController inputController = null;
    private CollisionDataRetriever collisionDataRetriever;
    private Rigidbody2D _rb2d;
    private Vector2 velocity;
    private bool onWallThis;
    private bool onGroundThis;
    private bool wantsJump;

    private float _wallDirectionX;
    

    
    // Start is called before the first frame update
    void Start()
    {
        collisionDataRetriever = GetComponent<CollisionDataRetriever>();
        _rb2d = GetComponent<Rigidbody2D>();
        // Need to clean up the code so it works like shinjis
        //_controller = GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        // If we are on a wall and not grounded (so as to avoid corner stuff)
        if (onWallThis && !onGroundThis)
        {
            // We can recieve jump inputs
            wantsJump |= inputController.RetrieveJumpHoldInput();
        }
    }

    private void FixedUpdate()
    {
        velocity = _rb2d.velocity;
        // Retrieve from our Collision Detector data relating to if we are on a wall or 
        // on the ground
        onWallThis = collisionDataRetriever.onWall;
        onGroundThis = collisionDataRetriever.onGround;
        // Gets a value of 1 if player is on left of wall, -1 on the right side
        _wallDirectionX = collisionDataRetriever.contactNormal.x;

        #region Wall Slide

        if (onWallThis)
        {
            // If we are on a wall and if we are going down
            if (velocity.y <= - _wallSlideMaxSpeed)
            {
                // Set our velocity to go down
                velocity.y = -_wallSlideMaxSpeed;
            }

        }

        // assign back the cached velocity to our rigidbody2d
        _rb2d.velocity = velocity;

        #endregion

        #region Wall Jump

        // Reset Wall Jumping variable if we're not on the wall and arent moving
        // Or just otherwise on the ground

        if ((onWallThis && velocity.x == 0) || onGroundThis)
        {
            WallJumping = false;
        }

        if(wantsJump)
        {
            // Compare our wall contact normal with our player input
            // If they are equal, it means we are holding against the wall
            if (-_wallDirectionX == inputController.RetrieveMoveInput() )
            {
                velocity = new Vector2(wallJumpClimb.x * _wallDirectionX, wallJumpClimb.y);
                WallJumping = true;
                wantsJump = false;
                Debug.Log("Mega Man X");
            }
            //if we aren't holding up against a wall...we do our alt wall jump.
            // the wall bounce
            else if (inputController.RetrieveMoveInput() == 0)
            {
                velocity = new Vector2(wallJumpBounce.x * _wallDirectionX, wallJumpBounce.y);
                WallJumping = true;
                wantsJump = false;
                Debug.Log("Mario");
            }
            // otherwise, do a wall leap
            else
            {
                velocity = new Vector2( wallJumpLeap.x * _wallDirectionX, wallJumpLeap.y);
                WallJumping = true;
                wantsJump = false;
                Debug.Log("SUPER Mario");
            }
        }
        
        #endregion

        // Set the RB velocity to the new one
        _rb2d.velocity = velocity;
    }
}
