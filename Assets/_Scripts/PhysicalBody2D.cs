using UnityEngine;
using System.Collections.Generic;


namespace PoolPhysics
{
    // Contains information about collision contact
    public struct ContactPoint2D
    {
        public ContactPoint2D(
            Collider2D collider,
            Vector2 normal,
            Collider2D otherCollider,
            Vector2 point)
        {
            this.collider = collider;
            this.normal = normal;
            this.otherCollider = otherCollider;
            this.point = point;
        }

        public Collider2D collider;
        public Vector2 normal;
        public Collider2D otherCollider;
        public Vector2 point;
    }
}


namespace PoolPhysics
{
    // Physical body (rigid body) component 
    [RequireComponent(typeof(CircleCollider2D))]
    public sealed class PhysicalBody2D : MonoBehaviour
    {
        // the drag of the body
        public float drag = 1;

        // angular and linear velocities
        public Vector2 velocity { get; set; }
        public Vector3 angularVelocity{ get; private set; }

        private CircleCollider2D circleCollider2D;

        void OnValidate()
        {
            // UNDONE: This line is nonsense.
            drag = Mathf.Clamp(drag, 0.0F, float.MaxValue);
        }

        /// <summary>
        /// Apply impulse to the body.
        /// Do not use per frame!
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyImpulse(Vector2 impulse)
        {
            velocity += circleCollider2D.invMass * impulse;
        }

        // Reset velocity back to zero
        public void Stop()
        {
            velocity = Vector2.zero;
        }

        // Init once
        void Start()
        {
            // Get related component
            circleCollider2D = GetComponent<CircleCollider2D>();

            // Subscribe to events
            circleCollider2D.OnCollisionBegin2D += OnCollisionBegin2D;
            circleCollider2D.OnCollisionEnd2D += OnCollisionEnd2D;
            circleCollider2D.OnCollisionRemain2D += OnCollisionRemain2D;
        }

        // Resolve collision per frame when colliding
        void OnCollisionRemain2D(ContactPoint2D contact)
        {
            var otherRigidbody = contact.otherCollider.GetComponent<PhysicalBody2D>();
            var otherMaterial = contact.otherCollider.material;
            var thisCollider = circleCollider2D;
            var normal = contact.normal;

            // Do not resolve collision if one of colliders is trigger
            if (thisCollider.isTrigger || contact.otherCollider.isTrigger)
                return;

            // Place object on the contact point.
            transform.position = contact.point + contact.normal * (thisCollider.radius + 0.001F);
            
            // Calculate radius from center of mass (COM) to a contact point for both objects
            var aRadius = contact.point - (Vector2)thisCollider.transform.position;
            var bRadius = contact.point - (Vector2)contact.otherCollider.transform.position;

            // Calculate relative velocity
            var relativeVelocity = otherRigidbody && otherRigidbody.enabled ? otherRigidbody.velocity - velocity : -velocity;

            // Calculate velocity along normal
            float velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);

            // Calculate cross product for between radius and normal for both objects
            float aRadiusAcrossNormal = MathExt.CrossProduct(aRadius, normal);
            float bRadiusAcrissNormal = MathExt.CrossProduct(bRadius, normal);

            // Calculate inversed mass sum
            float invMassSum = circleCollider2D.invMass + contact.otherCollider.invMass
                + Mathf.Pow(aRadiusAcrossNormal, 2) * circleCollider2D.invInertia + Mathf.Pow(bRadiusAcrissNormal, 2) * contact.otherCollider.invInertia;

            // Calculate elasticity
            var elisticity = Mathf.Min(circleCollider2D.material.elasticity, otherMaterial ? otherMaterial.elasticity : circleCollider2D.material.elasticity);

            // Calculate impulse
            float impulseFactor = -(1.0F + elisticity) * velocityAlongNormal;
            impulseFactor /= invMassSum;
            var impulse = impulseFactor * normal;

            // Apply impulse for this object
            ApplyImpulse(-impulse);

            // Apply impulse for collision object
            if (otherRigidbody && otherRigidbody.enabled)
                otherRigidbody.ApplyImpulse(impulse);
        }

        // Empty for now
        void OnCollisionEnd2D(ContactPoint2D contact)
        {
        }

        // Empty for now
        void OnCollisionBegin2D(ContactPoint2D contact)
        {
            
        }

        // Update per frame
        void FixedUpdate()
        {
            /* 
             * Simulate angular velocity as if sphere was on the ground
             * ----------------
             * Cross product!!
             * let V = [3x,3y,0z]
             * let Pd = [0x, 0y, 1z] 
             * V x Pd = [a3y * a1z - a0z*b0y, a0z*b0x - a3x * b1z, a3x*b0y - a3y * b0x]
             * V x Pd = [3, -3, 0]
             * -------------------
             */

            // Calculate rotation of the body
            var I = circleCollider2D.intertia;
            var V = velocity;
            var p = transform.position;
            p.z -= circleCollider2D.radius;

            var T = Vector3.Cross(V, transform.position - p);
            T = new Vector3(T.x, T.y, 0.0F);
            angularVelocity = T / I;

            // Apply drag to teh velocity
            velocity *= Mathf.Clamp01(1f - drag * Time.fixedDeltaTime);
            transform.position += (Vector3)velocity * Time.fixedDeltaTime;

            // Apply body roation
            // x axis -> z axis
            // y axis -> x axis
            // z axis -> y axis
            transform.Rotate(angularVelocity, Space.World);
        }
    }
}