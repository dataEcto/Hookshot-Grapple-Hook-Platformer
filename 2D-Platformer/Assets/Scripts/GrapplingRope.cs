using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    [Header("General Refernces:")]
    public GrapplingGun grapplingGun;
    public LineRenderer m_lineRenderer;

    [Header("General Settings:")]
    [Tooltip("The amount of points the line renderer will make when creating the rope. 40 and above is fine.")]
    [SerializeField] private int percision = 40;
    [Tooltip("How long it takes for the rope to straighten up.")]
    [Range(0, 20)] [SerializeField] private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")]
    [Tooltip("This effects how the rope looks after its been shot. Starts at 0,0, ends in 1,0")]
    public AnimationCurve ropeAnimationCurve;
    [Tooltip("The size of the wave.")]
    [Range(0.01f, 4)] [SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;

    [Header("Rope Progression:")]
    [Tooltip("Represents the lerp from point a (the fire point) to point b (the point that was aimed at). The values are 0,0, end in 1,1")]
    public AnimationCurve ropeProgressionCurve;
    [Tooltip("Speed of the Lerp")]
    [SerializeField] [Range(1, 50)] private float ropeProgressionSpeed = 1;

    float moveTime = 0;

    [HideInInspector] public bool isGrappling = true;

    bool strightLine = true;

    /// <summary>
    /// OnEnable, we set some parameters
    /// </summary>
    private void OnEnable()
    {
        // We start counting the time since this Rope has been enabled.
        moveTime = 0;
        // Set the amount of points that make up our Rope Line
        m_lineRenderer.positionCount = percision;
        // Start off our wave size with its default size
        waveSize = StartWaveSize;
        // Presumably, we just fired the rope, so we don't want it straight yet.
        strightLine = false;

        // See method for explanation
        LinePointsToFirePoint();

        // Enable our line renderer
        m_lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        m_lineRenderer.enabled = false;
        isGrappling = false;
    }

    /// <summary>
    /// Sets all of our points determined via percision to the firepoint.
    /// </summary>
    private void LinePointsToFirePoint()
    {
        for (int i = 0; i < percision; i++)
        {
            m_lineRenderer.SetPosition(i, grapplingGun.firePoint.position);
        }
    }

    private void Update()
    {
        // Increase our move time
        moveTime += Time.deltaTime;
        // Draw the Rope
        DrawRope();
    }

    /// <summary>
    /// Draws the Rope properly, using the points we have that percision gave us
    /// </summary>
    void DrawRope()
    {
        if (!strightLine)
        {
            // We want to check if our last point reached the Grapple Point yet.
            if (m_lineRenderer.GetPosition(percision - 1).x == grapplingGun.grapplePoint.x)
            {
                // If it did, we want to straight up the line, which you will see in the Else statement
                strightLine = true;
            }
            else
            {   // Otherwise, draw the rope waves
                DrawRopeWaves();
            }
        }
        else
        {
            // This is where we start grappling
            if (!isGrappling)
            {
                grapplingGun.Grapple();
                isGrappling = true;
            }
            if (waveSize > 0)
            {
                // Gradually decrease the size of the waves as its above 0
                waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopeWaves();
            }
            else
            {
                // We make sure WaveSize is 0 and no longer negative
                waveSize = 0;

                // Now our line is straight. But we still have all our points, so we need
                // To get rid of them
                if (m_lineRenderer.positionCount != 2) 
                { 
                    m_lineRenderer.positionCount = 2;   
                }

                //Set our 2 Points
                DrawRopeNoWaves();
            }
        }
    }

    /// <summary>
    /// Draw the rope waves
    /// A bit of complicated code that has to do with animation curves.
    /// I'll probably look at this explanation later once i study Animation Curves...
    /// </summary>
    void DrawRopeWaves()
    {
        for (int i = 0; i < percision; i++)
        {
            float delta = (float)i / ((float)percision - 1f);
            Vector2 offset = Vector2.Perpendicular(grapplingGun.grappleDistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * waveSize;
            Vector2 targetPosition = Vector2.Lerp(grapplingGun.firePoint.position, grapplingGun.grapplePoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(grapplingGun.firePoint.position, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            m_lineRenderer.SetPosition(i, currentPosition);
        }
    }

    
    /// <summary>
    /// Draw the Rope without any waves.
    /// </summary>
    void DrawRopeNoWaves()
    {
        m_lineRenderer.SetPosition(0, grapplingGun.firePoint.position);
        m_lineRenderer.SetPosition(1, grapplingGun.grapplePoint);
    }
}
