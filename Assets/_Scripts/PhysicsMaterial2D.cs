using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;


namespace PoolPhysics
{
    // Create material for an physical body
    public class PhysicsMaterial2D : ScriptableObject
    {
        // the density of the body
        public float density = 1;

        // the elasticity factor the body
        public float elasticity = 0.8F;

#if UNITY_EDITOR
        // Create actual asset
        [MenuItem("Assets/Create/PoolPhysics/Material")]
        public static void CreateAsset()
        {
            var asset = ScriptableObject.CreateInstance<PhysicsMaterial2D>();

            AssetDatabase.CreateAsset(asset, "Assets/NewPoolPhysicsMaterial.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
#endif

    }
}