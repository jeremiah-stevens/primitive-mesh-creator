using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PrimitiveCreator
{
    [AddComponentMenu(PrimitiveCreatorConfig.AddComponentPrefix + "Polygon Collider")]
    public class PolygonCollider : PrimitiveCollider
    {
        protected override PrimitiveCreatorUtility.MeshType meshType => PrimitiveCreatorUtility.MeshType.Polygon;
    }
}