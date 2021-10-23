using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using static PrimitiveCreator.PrimitiveCreatorUtility;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace PrimitiveCreator
{
    [CreateAssetMenu(fileName = "New PrimitiveCreatorConfig", menuName = "Primitive Creator/PrimitiveCreatorConfig")]
    public class PrimitiveCreatorConfig : ScriptableObject
    {
        private static PrimitiveCreatorConfig _instance;
        public static PrimitiveCreatorConfig instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PrimitiveCreatorConfig();

                return _instance;
            }
        }


        //Model limitations
        public const int MinimumVertexCount = 0;
        public const int MaximumVertexCount = 10000;

        //MenuItem attributes
        public const string MenuItemPrefix = "GameObject/3D Object/Primitive Creator/";

        //default mesh saving operations
        public const string SaveSubpathMesh = "Meshes/Defaults";
        public const string SaveSubpathColliders = "Meshes/Colliders";

        //AddCollider attributes
        public const string AddComponentPrefix = "Primitive Creator/";

#if UNITY_EDITOR
        [SerializeField, HideInInspector] DefaultAsset pluginRootFolder;
#endif

        /*TODO:
         * settings to save Collider meshes:
         * • Save all DefaultColliderMeshes
         * • Save the missing DefaultColliderMeshes
         */
    }
}