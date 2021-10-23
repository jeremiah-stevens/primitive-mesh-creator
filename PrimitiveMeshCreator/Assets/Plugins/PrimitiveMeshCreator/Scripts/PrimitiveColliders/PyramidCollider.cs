using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PrimitiveCreator
{
    [AddComponentMenu(PrimitiveCreatorConfig.AddComponentPrefix + "Pyramid Collider")]
    public class PyramidCollider : PrimitiveCollider
    {
        protected override PrimitiveCreatorUtility.MeshType meshType => PrimitiveCreatorUtility.MeshType.Pyramid;
    }
}