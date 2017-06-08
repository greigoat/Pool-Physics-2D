using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PoolPhysics
{
    /// <summary>
    /// 2D Circle collider.
    /// Represents 2d circle collision volume.
    /// </summary>
    [AddComponentMenu("Physics2D/CircleCollider2D")]
    public class CircleCollider2D : Collider2D
    {
        // The radius of the circle
        public float radius = 1;

        // Circle collision events
        public override event Collision2DHandler OnCollisionBegin2D;
        public override event Collision2DHandler OnCollisionEnd2D;
        public override event Collision2DHandler OnCollisionRemain2D;

        // Transform scale based size
        protected override Vector3 scaledSize
        {
            get
            {
                return new Vector2(radius * 2 * transform.localScale.x, radius * 2 * transform.localScale.y);
            }
        }

        // Volume of the circle
        public override float volume
        {
            get
            {
                return Mathf.PI * radius * radius * (material ? material.density : 1);
            }
        }

        // Gets the inertia of the circle collider
        // Used by physical body
        public override float intertia
        {
            get
            {
                return mass * radius * 2;
            }
        }

        // Collision intersection test against other colliders
        protected override void OnCollisionTest2D(Collider2D other)
        {
            if (other == this)
                return;

            if (!this)
                return;

            if (!other)
                return;

            // if collider is circle
            if (other as CircleCollider2D)
            {
                var c1 = this;
                var c2 = other as CircleCollider2D;
                var Vc1 = center;
                var Vc2 = c2.center;
                var r1 = c1.radius;// *c1.transform.lossyScale.x;
                var r2 = c2.radius;// *c2.transform.lossyScale.x;

                // calculate distance
                var Vd = (Vc1 - Vc2);

                // If intersects
                if (Vd.magnitude < r1 + r2)
                {
                    var N = (Vc1 - Vc2).normalized;
                    var Vp = Vc2 + N * r2;

                    // Create new Contact point
                    lastContact = new ContactPoint2D(c1, N, c2, Vp);

                    // Fire collision events
                    //
                    if (OnCollisionBegin2D != null && !beginCollision)
                    {
                        OnCollisionBegin2D(lastContact);
                        beginCollision = true;
                    }

                    if (OnCollisionRemain2D != null)
                        OnCollisionRemain2D(lastContact);

                }
                else
                {
                    if (OnCollisionEnd2D != null && beginCollision)
                    {
                        OnCollisionEnd2D(lastContact);
                        beginCollision = false;
                    }
                }
            }
            // if collider is a 2d box collider
            // TODO: Fix collision ignoring at high speeds
            if (other as BoxCollider2D)
            {
                var B = other as BoxCollider2D;
                var A = this;
                var N = Vector2.zero;
                var r = A.radius;
                Vector2 Vc = A.transform.position;
                var Vbc = Vector2.zero;
                var max = B.bounds.max;
                var min = B.bounds.min;
                var xMax = max.x;
                var yMax = max.y;
                var xMin = min.x;
                var yMin = min.y;
                var VClx = Vc.x + r;
                var VCrx = Vc.x - r;
                var VCty = Vc.y - r;
                var VCby = Vc.y + r;

                // Top Left Corner Overlap
                if (Vc.y >= yMax && Vc.x <= xMin)
                    Vbc = new Vector2(xMin, yMax);
                // Top Right Corner Overlap
                else if (Vc.y >= yMax && Vc.x >= xMax)
                    Vbc = B.bounds.max;
                // Bottom left corner overlap
                else if (Vc.y <= yMin && Vc.x <= xMin)
                    Vbc = B.bounds.min;
                // Bottom right corner overlap
                else if (Vc.y <= yMin && Vc.x >= xMax)
                    Vbc = new Vector2(xMax, yMin);
                // Right
                else if (VClx >= xMax && Vc.y <= yMax && Vc.y >= yMin)
                    Vbc = new Vector2(xMax, Vc.y);
                // Left
                else if (VCrx <= xMin && Vc.y <= yMax && Vc.y >= yMin)
                    Vbc = new Vector2(xMin, Vc.y);
                // Top
                else if (VCty <= yMax && VCty >= yMin && Vc.x <= xMax && Vc.x >= xMin)
                    Vbc = new Vector2(Vc.x, yMax);
                // Bottom
                else if (VCby >= yMin && VCty <= yMax && Vc.x <= xMax && Vc.x >= xMin)
                    Vbc = new Vector2(Vc.x, yMin);

                // if intersects
                if (Vbc != Vector2.zero && Vector2.Distance(Vc, Vbc) <= r)
                {
                    // calculate normal
                    N = (Vc - Vbc).normalized;
                    
                    // create new contact
                    lastContact = new ContactPoint2D(A, N, B, Vbc);

                    // Fire collision events
                    //
                    if (OnCollisionBegin2D != null && !beginCollision)
                    {
                        OnCollisionBegin2D(lastContact);
                        beginCollision = true;
                    }

                    if (OnCollisionRemain2D != null)
                        OnCollisionRemain2D(lastContact);

                }
                else
                {
                    if (OnCollisionEnd2D != null && beginCollision)
                    {
                        OnCollisionEnd2D(lastContact);
                        beginCollision = false;
                    }
                }
            }
        }

#if UNITY_EDITOR
        // Draw gizmos per frame
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (Selection.activeGameObject == gameObject && enabled)
                Handles.DrawWireDisc(center, Vector3.back, radius);
        }
#endif
    }   
}
