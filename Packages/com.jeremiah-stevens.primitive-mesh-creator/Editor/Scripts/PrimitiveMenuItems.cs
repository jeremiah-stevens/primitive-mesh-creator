#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using static PrimitiveCreator.PrimitiveCreatorUtility;


/*TODO:
 * • See about dynamically generating these create buttons based on MeshType enums
 * • Translate the position of the primitive based on the camera position (Unity does this in the Editor, need to see how we can use this); see: https://answers.unity.com/questions/240999/instantiate-object-in-middle-of-editor-view-not-ma.html
 */
namespace PrimitiveCreator
{
    /// <summary>
    /// Provides a set of MenuItem functions for creating primitives within a scene. These can be found under 'GameObject/3D Object/Primitive Creator' in the Unity Editor.
    /// </summary>
    public static class PrimitiveMenuItems
    {
        //constants used to divide the different primitives based on category
        const int Priority2DShapes = 0;
        const int PriorityGeneric = 20;
        const int PrioritySpheres = 40;
        const int PriorityPolyhedrons = 60;




        #region MenuItem Functions

        //2D shapes

        [MenuItem(PrimitiveCreatorConfig.MenuItemPrefix + "Quad", priority = Priority2DShapes)]
        static void CreateQuad() { CreatePrimitiveInScene(MeshType.Quad); }

        [MenuItem(PrimitiveCreatorConfig.MenuItemPrefix + "Triangle", priority = Priority2DShapes)]
        static void CreateTriangle() { CreatePrimitiveInScene(MeshType.Triangle); }

        [MenuItem(PrimitiveCreatorConfig.MenuItemPrefix + "Polygon", priority = Priority2DShapes)]
        static void CreatePolygon() { CreatePrimitiveInScene(MeshType.Polygon); }


        //Generic 3D

        [MenuItem(PrimitiveCreatorConfig.MenuItemPrefix + "Cube", priority = PriorityGeneric)]
        static void CreateCube() { CreatePrimitiveInScene(MeshType.Cube); }

        [MenuItem(PrimitiveCreatorConfig.MenuItemPrefix + "Pyramid", priority = PriorityGeneric)]
        static void CreatePyramid() { CreatePrimitiveInScene(MeshType.Pyramid); }


        //Spheres

        [MenuItem(PrimitiveCreatorConfig.MenuItemPrefix + "Sphere (UV)", priority = PrioritySpheres)]
        static void CreateSphereUV() { CreatePrimitiveInScene(MeshType.SphereUV); }


        //Polyhedrons

        [MenuItem(PrimitiveCreatorConfig.MenuItemPrefix + "Tetrahedron (d4)", priority = PriorityPolyhedrons)]
        static void CreateTetrahedron() { CreatePrimitiveInScene(MeshType.Tetrahedron); }

        [MenuItem(PrimitiveCreatorConfig.MenuItemPrefix + "Hexahedron (d6)", priority = PriorityPolyhedrons)]
        static void CreateHexahedron() { CreatePrimitiveInScene(MeshType.Hexahedron); }

        [MenuItem(PrimitiveCreatorConfig.MenuItemPrefix + "Octahedron(d8)", priority = PriorityPolyhedrons)]
        static void CreateOctahedron() { CreatePrimitiveInScene(MeshType.Octahedron); }

        //PrimitiveCreatorComponent
        [MenuItem(PrimitiveCreatorConfig.MenuItemPrefix + "Primitive Creator Component", priority = -1)]
        static void CreatePrimitiveCreatorComponent()
        {
#if UNITY_EDITOR
            GameObject obj = ObjectFactory.CreateGameObject("PrimitiveCreatorComponent");
#else
            GameObject obj = new GameObject("PrimitiveCreatorComponent");
#endif
            obj.transform.parent = Selection.activeTransform;
            obj.transform.localPosition = Vector3.zero;

            obj.AddComponent(typeof(PrimitiveCreatorComponent));
        }

        #endregion




        static void CreatePrimitiveInScene(MeshType meshType)
        {
            GameObject spawnedPrimitive = PrimitiveCreatorUtility.CreatePrimitive(meshType);
            spawnedPrimitive.transform.parent = Selection.activeTransform;
            spawnedPrimitive.transform.localPosition = Vector3.zero;
        }
    }
}

#endif