using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PoolPhysics
{
    /// <summary>
    /// 2D box collider.
    /// Represents 2D AABB box collision
    /// </summary>
    public class BoxCollider2D : Collider2D
    {
        // Initial size
        public Vector2 size = Vector2.one;

        // Transform scale based size
        protected override Vector3 scaledSize
        {
            get
            {
                var s = new Vector2(size.x * transform.localScale.x, size.y * transform.localScale.y);
                //s = transform.InverseTransformDirection(s);
                return s;
            }
        }

        // 2D volume of the box
        public override float volume
        {
            get { return Mathf.Abs(bounds.max.x) * Mathf.Abs(bounds.max.y); }
        }

        // Gets the inertia of the box volume
        // Used by physical body
        public override float intertia
        {
            get
            {
                var bc = this;
                const float kInv3 = 1 / 3.0F;
                float I = 0.0F;

                // Create vertices from bounds
                Vector2[] vertices = new Vector2[]
                    {
                        new Vector2(bc.bounds.min.x, bc.bounds.max.y),
                        new Vector2(bc.bounds.max.x, bc.bounds.max.y),
                        new Vector2(bc.bounds.max.x, bc.bounds.min.y),
                        new Vector2(bc.bounds.min.x, bc.bounds.min.y)
                    };

                // Calculate intertia for each vertex
                for (int i1 = 0; i1 < vertices.Length; ++i1)
                {
                    Vector2 p1 = vertices[i1];
                    int i2 = i1 + 1 < vertices.Length ? i1 + 1 : 0;
                    var p2 = vertices[i2];
                    //a.x* b.y - a.y * b.x;
                    float D = p1.x * p2.y - p1.y * p2.x;
                    float intx2 = p1.x * p1.x + p2.x * p1.x + p2.x * p2.x;
                    float inty2 = p1.y * p1.y + p2.y * p1.y + p2.y * p2.y;

                    // Add to the total intertia sum
                    I += (0.25f * kInv3 * D) * (intx2 + inty2);
                }

                // If there's no material density of 1 will be applied
                return I * (material ? material.density : 1);
            }
        } 

        // UNDONE: handle box collision events?
        //public override event Collision2DHandler OnCollisionBegin2D;
        //public override event Collision2DHandler OnCollisionEnd2D;
        //public override event Collision2DHandler OnCollisionRemain2D;

        // Component awake
        protected override void Awake()
        {
            base.Awake();
        }

        // Component update per frame
        protected override void Update()
        {
            base.Update();
        }

#if UNITY_EDITOR
        // Draws gizmos per frame
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            // Draw if selected object is equal this game object and game object is enabled
            if (Selection.activeGameObject == gameObject && enabled)
                Handles.DrawWireCube(center, scaledSize);
        }
#endif
    }
}
