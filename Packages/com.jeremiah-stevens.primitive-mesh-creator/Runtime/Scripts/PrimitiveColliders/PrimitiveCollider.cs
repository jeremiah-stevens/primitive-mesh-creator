using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PrimitiveCreator
{
    /// <summary>
    /// Base Collider component for all PrimitiveCreator colliders.
    /// </summary>
    [ExecuteAlways][AddComponentMenu("")]
    public class PrimitiveCollider : MonoBehaviour
    {
        protected virtual PrimitiveCreatorUtility.MeshType meshType => throw new System.NotImplementedException();




        private void Awake()
        {
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = PrimitiveCreatorUtility.CreateColliderMesh(meshType);

            DestroyImmediate(this);
        }
    }
}