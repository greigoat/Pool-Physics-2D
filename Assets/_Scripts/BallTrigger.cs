using UnityEngine;
using System.Collections;


/// <summary>
///  Ball Trigger Class.
///  The tigger event will be invoked when any of balls existing are within collision area.
///  Should be placed on goal game objects
/// </summary>
[RequireComponent(typeof(PoolPhysics.Collider2D))]
public class BallTrigger : MonoBehaviour 
{
    private PoolPhysics.CircleCollider2D colliderComponent;

	// Use this for initialization
	void Start ()
    {
        // Find related component
        colliderComponent = GetComponent<PoolPhysics.CircleCollider2D>();
        
        // Subscribe to the collision event
        colliderComponent.OnCollisionRemain2D += OnCollisionRemain2D;
	}

    // Will do things if balls are within goal area
    void OnCollisionRemain2D(PoolPhysics.ContactPoint2D contact)
    {
        // If this contact transform is within this collider area
        var dst = Vector2.Distance(transform.position, contact.otherCollider.transform.position);
        if (dst < colliderComponent.radius)
        {
            // Check if collider is the cue ball.
            if (contact.otherCollider.tag == "CueBall")
            {
                // Reset cue ball position
                contact.otherCollider.transform.position = new Vector2(9.06F, 0.07F);

                // Reset physical body velocity
                var pb = contact.otherCollider.GetComponent<PoolPhysics.PhysicalBody2D>();
                pb.Stop();

                return;
            }

            // Or else just destroy the ball
            Destroy(contact.otherCollider.gameObject);
        }
    }
}
