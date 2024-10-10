using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Checks Collisions
/// </summary>
public class CollisionDataRetriever : MonoBehaviour
{
    public bool onGround { get; private set; }
     public bool onWall { get; private set; }
    public float friction { get; private set; }

    public Vector2 contactNormal { get; private set; }
    public PhysicsMaterial2D material2D;

    /// <summary>
    /// On Collision Enter and Stay, we check if we are grounded and what friction to use
    /// </summary>
    /// <param name="collision2D"></param>
    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        EvaluateCollision(collision2D);
        RetrieveFriction(collision2D);
    }

    private void OnCollisionStay2D(Collision2D collision2D)
    {
        EvaluateCollision(collision2D);
        RetrieveFriction(collision2D);
    }

    /// <summary>
    /// Reset values when we leave
    /// </summary>
    /// <param name="collision2D"></param>
    private void OnCollisionExit2D(Collision2D collision2D)
    {
        onGround = false;
        friction = 0;
        onWall = false;
    }

    /// <summary>
    /// Check if we are on the ground or not
    /// </summary>
    /// <param name="collision2D"></param>
    public void EvaluateCollision(Collision2D collision2D)
    {
        // This for loop checks what we collided with.
        // It iterates through every contact point
        // And check if its contactNormal is greater than 0.9
        // 1 = Flat Surface
        for (int i = 0; i < collision2D.contactCount; i++)
        {
            contactNormal = collision2D.GetContact(i).normal;

            // This is an OR assignment operator
            // onGround = to itself or contactNormal.y >= 0.9f
            onGround |= contactNormal.y >= 0.9f;

            // To check if we are on wall, check if the contact normal is above
            // a certain treshold
            onWall = Mathf.Abs(contactNormal.x) >= 0.9f;
        }
    }
    
    /// <summary>
    /// Check the Friction of the ground we are on and set it.
    /// </summary>
    /// <param name="collision2D"></param>
    private void RetrieveFriction(Collision2D collision2D)
    {
        material2D = collision2D.rigidbody.sharedMaterial;

        // If we don't have any physics material, set the friction to 0 as a default;
        // Could probably have a default friction value declared to set this too as well..
        friction = 0;

        if (material2D != null)
        {
            friction = material2D.friction;
        }
    }

    /// <summary>
    /// Allow other scripts to access our onGround and Friction values
    /// </summary>
    /// <returns></returns>
    public bool GetOnGroundValue()
    {
        return onGround;
    }

    public float GetFrictionValue()
    {
        return friction;
    }
}
