using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PrimitiveCreator
{
    [AddComponentMenu(PrimitiveCreatorConfig.AddComponentPrefix + "Sphere (UV) Collider")]
    public class SphereUVCollider : PrimitiveCollider
    {
        protected override PrimitiveCreatorUtility.MeshType meshType => PrimitiveCreatorUtility.MeshType.SphereUV;
    }
}