using UnityEngine;

/// <summary>
/// GrapplingGun
/// This script handles the logic for firing the Grappling Rope, how the player will be affected by the rope, and where we can launch the point to.
/// Basically, the script activates our GrapplingRope game object, gives it a point to grapple too, and determines how our player will launch
/// </summary>
public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public GrapplingRope grappleRope;

    [Header("Layers Settings:")]
    [SerializeField] private bool grappleToAll = false;
    [SerializeField] private int grappableLayerNumber = 9;

    [Header("Main Camera:")]
    [Tooltip("This reference is used for allowing us to click on where we weant the Grappling Gun to fire at.")]
    public Camera m_camera;

    [Header("Transform Ref:")]
    [Tooltip("The Character holding the gun - most likely the Player")]
    public Transform gunHolder;
    [Tooltip("Where the gun will pivot around when aiming")]
    public Transform gunPivot;
    [Tooltip("Where the Rope will fire from")]
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")]
    [Tooltip("If true, this makes it so the gun rotates slowly towards where you are aiming.")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;
    [SerializeField] private InputController inputController = null;
    private Vector2 playerDirection;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [Tooltip("Max Distance is shown via a Gizmo")]
    [SerializeField] private float maxDistance = 20;

    
    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;

    [Tooltip("Transform Launch - Overrides all Physics Physics Launch - Uses Physics to move Via Spring Joint")]
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float launchSpeed = 1;

    [Header("No Launch To Point")]
    [Tooltip("Only use if there is no Launch to Point. You can have the Spring Joint 2D automatically configure the distance between the Player and the Point, keeping the player in its place.")]
    [SerializeField] private bool autoConfigureDistance = false;
    [Tooltip("If you rather configure it yourself, you can do so here. This does its own sort of launch as it pulls the player in.")]
    [SerializeField] private float targetDistance = 3;
    [Tooltip("Adjusts the 'bouciness' of the grapple. Higher numbers basically gets rid of any animation of the player being pulled up.")]
    [SerializeField] private float targetFrequncy = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    /// <summary>
    /// First, we disable our rope and spring joint.
    /// </summary>
    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;

    }

    /// <summary>
    /// There are 3 Main States in Grappling
    /// </summary>
    private void Update()
    {
        playerDirection.x = inputController.RetrieveMoveInput();
        playerDirection.y = inputController.RetrieveUpDownInput();
        
        // When we first Click, we check if we should grapple to the grapple point
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetGrapplePoint();
        }
        // When we hold the mouse down...
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            // We rotate our gun towards where we pointed to
            // And disable slow rotation, to rotate our gun immediately
            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
            }
            // If grappleRope isn't enabled, we're probably not pointing at anything we can grapple to.
            // This is the perfect place to add in some juice that says "hey, you cant grapple"
            // Maybe have the rope shoot out based on its max distance, then retract back...
            else
            {        
                Debug.Log("cant grapple");
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                // Usually set to true
                RotateGun(mousePos, true);
            }

            // If we wanted to launch to point and we are grappling...
            // Is grappling is set to true when we start to grapple/the rope reaches the grapple point
            if (launchToPoint && grappleRope.isGrappling)
            {
                // We check if we wanted to Transform Launch
                if (launchType == LaunchType.Transform_Launch)
                {
                    // Calculate the first point distance
                    Vector2 firePointDistnace = firePoint.position - gunHolder.localPosition;
                    // Launch the player twards the grapple point, 
                    // But we also make sure the player doesnt go exactly where fire point is.
                    // So we subtract that.
                    Vector2 targetPos = grapplePoint - firePointDistnace;
                    gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, Time.deltaTime * launchSpeed);
                }
            }
        }
        // If we let go...
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            m_rigidbody.gravityScale = 1;
        }
        
        // When Input for the Grappling Gun isn't being recieved, the gun Rotates towards
        // The mouse position 
        else
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            // The Method that rotates the mouse in question
            RotateGun(mousePos, false);
        }
    }

    /// <summary>
    /// Rotates the Gun, and checks if we rotate over time or not.
    /// </summary>
    /// <param name="lookPoint"></param>
    /// <param name="allowRotationOverTime"></param>
    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        // Calculate the Distance Vector to do some Trig
        Vector3 distanceVector = lookPoint - gunPivot.position;
        // Which lets us find the angle we want
        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;

        // If we want to rotate over time we do so here.
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        // Otherwise just rotate towards the aim.
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    /// <summary>
    /// Set's the point we want to Grapple towards
    /// </summary>
    void SetGrapplePoint()
    {
        // Make a raycast using the distance vector of our cursor as the raycast's direction
        // (Thats why we normalize it too )
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        

        if (Physics2D.Raycast(firePoint.position, distanceVector.normalized))
        {
            RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
            // If the ray casts hit something, we check to see if it hit the layer we want
            // Or if we dont care what it hits
            if (_hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll)
            {
                // Then, we check if its within range
                // Or ignore it if we don't have max distance
                if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistance || !hasMaxDistance)
                {
                    // Once we are here, set the grapple point to the raycast's _hit.point (it's a vector 3)
                    grapplePoint = _hit.point;
                    // Set the distance vector for the grapple rope script
                    grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                    // Enable the rope!
                    grappleRope.enabled = true;
                    Debug.Log("Enable Rope");

                    // Grappling is called in Grappling Rope, whenever the rope
                    // reaches its destination.
                }
            }
        }
    }

    /// <summary>
    /// This function is most important. It determines what kind of Launch we want.
    /// </summary>
    public void Grapple()
    {
        // There is only one occassion we want to use
        // Spring Joint 2D's Autoconfigure Distance
        // So we just disable it. 
        m_springJoint2D.autoConfigureDistance = false;

        // If we aren't Launching to point AND we arent Auto Configuring...
        if (!launchToPoint && !autoConfigureDistance)
        {
            // We set the distance and frequency according to 
            // what we put in the inspector.
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequncy;
        }
        // IF we aren't launching to point...
        if (!launchToPoint)
        {
            // If we want to autoconfigure distance...
            if (autoConfigureDistance)
            {
                // Enable Autoconfigure Distance
                m_springJoint2D.autoConfigureDistance = true;
                // Set the Frequency to 0
                m_springJoint2D.frequency = 0;
            }

            // In the end, we set the connected anchor to the grapple point vector 3
            m_springJoint2D.connectedAnchor = grapplePoint;
            // And enable our spring joint
            m_springJoint2D.enabled = true;
        }
        // If we do want Launch to point...
        else
        {
            // Here we check our launch types.
            switch (launchType)
            {
                // Physics
                case LaunchType.Physics_Launch:
                    // Set our connected anchor
                    m_springJoint2D.connectedAnchor = grapplePoint;

                    // Calculate the Vector 
                    Vector2 distanceVector = firePoint.position - gunHolder.position;

                    //Set the other stuff
                    m_springJoint2D.distance = distanceVector.magnitude;
                    m_springJoint2D.frequency = launchSpeed;
                    m_springJoint2D.enabled = true;
                    break;
                case LaunchType.Transform_Launch:
                    //Set our gravity and velocity to 0
                    // The actual logic is handled in fixed update
                    m_rigidbody.gravityScale = 0;
                    m_rigidbody.velocity = Vector2.zero;
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }
    }

}

