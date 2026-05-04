using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PrimitiveCreator
{
    [AddComponentMenu(PrimitiveCreatorConfig.AddComponentPrefix + "Tetrahedron Collider")]
    public class TetrahedronCollider : PrimitiveCollider
    {
        protected override PrimitiveCreatorUtility.MeshType meshType => PrimitiveCreatorUtility.MeshType.Tetrahedron;
    }
}