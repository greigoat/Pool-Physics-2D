using UnityEngine;
using System.Collections;

namespace PoolPhysics
{
    /// <summary>
    /// Base class of all pool physics colliders
    /// </summary>
    public abstract class Collider2D : MonoBehaviour
    {
        // physical material of the collider
        public PhysicsMaterial2D material;

        // is collider a trigger
        public bool isTrigger;

        // positional offset
        public Vector2 offset;

        // collsion event delegates & events
        public delegate void Collision2DHandler(ContactPoint2D contact);
        private delegate void CollisionTest2DHandler(Collider2D thisCollider);
        private event CollisionTest2DHandler OnCollisionTest2DEvent;
        public virtual event Collision2DHandler OnCollisionBegin2D;
        public virtual event Collision2DHandler OnCollisionEnd2D;
        public virtual event Collision2DHandler OnCollisionRemain2D;
        protected bool beginCollision;
        protected ContactPoint2D lastContact;
        public Bounds bounds { get; protected set; }

        // Transform scale based size
        protected virtual Vector3 scaledSize { get { return Vector3.zero; } }

        // Center of the collision volume
        protected Vector2 center
        {
            get
            {
                return ((Vector2)transform.position + offset); 
            }
        }

        // Gets the volume of the collider
        public virtual float volume
        {
            get
            {
                return 0.0F;
            }
        }

        // Collider volume mass
        public float mass { get { return volume * (material ? material.density : 1); } }
        // Zerosafe inverted mass e.g 1/mass
        public float invMass { get { return mass != 0.0F ? 1 / mass : 0.0F; } }

        // Gets the inertia of the volume
        public virtual float intertia
        {
            get
            {
                return 0.0F;
            }
        }

        // 1/inertia
        public float invInertia { get { return intertia != 0.0F ? 1 / intertia : 0.0F; } }

        // The actual intersection test
        protected virtual void OnCollisionTest2D(Collider2D other) { }

        // Init once
        protected virtual void Awake()
        {
            bounds = new Bounds(center, scaledSize);

            foreach (var cl in FindObjectsOfType<Collider2D>())
                OnCollisionTest2DEvent += cl.OnCollisionTest2D;
        }

        // Update per frame
        protected virtual void Update()
        {
            var bs = bounds;
            // TODO: Fix rotational position;

            bounds = bs;

            if (OnCollisionTest2DEvent != null)
                OnCollisionTest2DEvent(this);
        }

#if UNITY_EDITOR
        // Draw gizmos per frames
        protected virtual void OnDrawGizmos() 
        {
            var color = new Color(0.498F, 0.788F, 0.478F, 1.0F);
            Gizmos.color = color;
            UnityEditor.Handles.color = color;
        }
#endif
    }
}