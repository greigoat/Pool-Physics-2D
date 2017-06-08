using UnityEngine;

namespace PoolPhysics
{
    // Extened math tools for vectors
    static class MathExt
    {
        // Two crossed vectors return a scalar
        public static float CrossProduct(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        // Cross product
        public static Vector2 CrossProduct(Vector2 a, float s)
        {
            return new Vector2(s * a.y, -s * a.x);
        }

        // Cross product
        public static Vector2 CrossProduct(float s, Vector2 a)
        {
            return new Vector2(-s * a.y, s * a.x);
        }
    }
}
