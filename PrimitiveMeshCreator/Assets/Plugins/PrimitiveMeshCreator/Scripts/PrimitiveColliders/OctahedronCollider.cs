using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PrimitiveCreator
{
    [AddComponentMenu(PrimitiveCreatorConfig.AddComponentPrefix + "Octahedron Collider")]
    public class OctahedronCollider : PrimitiveCollider
    {
        protected override PrimitiveCreatorUtility.MeshType meshType => PrimitiveCreatorUtility.MeshType.Octahedron;
    }
}