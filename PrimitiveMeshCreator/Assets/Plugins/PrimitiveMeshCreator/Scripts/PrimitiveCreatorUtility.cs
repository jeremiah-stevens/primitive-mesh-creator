using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using PrimitiveCreator.MeshCreators;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace PrimitiveCreator
{
    /// <summary>
    /// Standard interface for creating primitive meshes.
    /// </summary>
    public static class PrimitiveCreatorUtility
    {
        //TODO: reorganize these better
        public enum MeshType
        {
            //2D shapes
            Quad = 0,        //simple 2D quad; draws a single face (as opposed to 4-gon)
            Triangle = 2,    //simple 2D triangle; draws a single face (as opposed to 3-gon)
            Polygon = 3,     //generic 2D polygon (ex: 4-gon is a quad, 5-gon is a pentagon, ...); each n is given its own triangle

            //Polyhedrons
            Tetrahedron = 4, //Platonic solid, 4-sided (i.e. a 'd4')
            Hexahedron = 8,  //Platonic solid, 6-sided (i.e. a 'd6')
            Octahedron = 6,  //Platonic solid, 8-sided (i.e. a 'd8')

            //General Shapes
            Cube = 1,        //simple 3D cube
            Pyramid = 7,     //Pyramid (4 triangles, 1 quad at the bottom)

            //Spheres
            SphereUV = 5,    //UV sphere (i.e. a sphere consisting of quads divided between the poles)
        }

        //bounds on vertex count to prevent excessive model size
        public const int MinVertexCount = 0;
        public const int MaxVertexCount = 10000;

        private static Dictionary<MeshType, MeshCreator> _meshCreators = new Dictionary<MeshType, MeshCreator>();




        #region MeshCreator Dictionary

        /// <summary>
        /// Initializes the MeshCreator dictionary for access.
        /// </summary>
        private static void Initialize()
        {
            _meshCreators.Clear();

            Assembly assembly = Assembly.GetAssembly(typeof(MeshCreator));

            var meshCreatorTypes = assembly.GetTypes()
                .Where(t => typeof(MeshCreator).IsAssignableFrom(t) && !t.IsAbstract);

            foreach(var meshCreatorType in meshCreatorTypes)
            {
                MeshCreator creator = Activator.CreateInstance(meshCreatorType) as MeshCreator;
                _meshCreators.Add(creator.MeshType, creator);
            }
        }

        /// <summary>
        /// Gets a MeshCreator for the requested MeshType.
        /// </summary>
        /// <param name="meshType">MeshType to get the associated MeshCreator.</param>
        /// <returns>MeshCreator for the requested MeshType.</returns>
        private static MeshCreator GetMeshCreator(MeshType meshType)
        {
            if (_meshCreators == null || _meshCreators.Count == 0)
                Initialize();

            return _meshCreators[meshType];
        }

        #endregion



        #region Primitive Mesh Creation

        /// <summary>
        /// Gets the display name for the provided MeshType.
        /// </summary>
        /// <param name="meshType">MeshType to get the name for.</param>
        public static string GetDisplayName(MeshType meshType)
        {
            return GetMeshCreator(meshType).DisplayName;
        }

        /// <summary>
        /// Creates a default Mesh of the given MeshType.
        /// </summary>
        /// <param name="meshType">MeshType to create.</param>
        /// <returns>Default Mesh of the given MeshType.</returns>
        public static Mesh CreateMesh(MeshType meshType)
        {
            return GetMeshCreator(meshType).CreateMesh();
        }

        /// <summary>
        /// Creates a mesh of the given MeshType customized by the given MeshDetails.
        /// </summary>
        /// <param name="meshType">MeshType to create.</param>
        /// <param name="meshDetails">Set of details to modify the mesh with.</param>
        /// <returns>Mesh of the given MeshType customized with the MeshDetails.</returns>
        public static Mesh CreateMesh(MeshType meshType, MeshCreator.MeshDetails meshDetails)
        {
            return GetMeshCreator(meshType).CreateMesh(meshDetails);
        }

        /// <summary>
        /// Returns a new MeshDetails of the appropriate class. If previousMeshDetails is provided, it will convert properties of the previousMeshDetails into the new class.
        /// </summary>
        /// <param name="meshType">Type of MeshDetail to return</param>
        /// <param name="previousMeshDetails">(Optional) Set of previous mesh details to copy into new class</param>
        /// <returns>New MeshDetails class</returns>
        public static MeshCreator.MeshDetails GetMeshDetails(MeshType meshType, MeshCreator.MeshDetails previousMeshDetails = null)
        {
            return GetMeshCreator(meshType).GetMeshDetails(previousMeshDetails);
        }

        /// <summary>
        /// Gets the closest vertex count to the proposed vertex count that still provides for a viable model.
        /// </summary>
        /// <param name="meshType">Type of mesh to check for vertex count.</param>
        /// <param name="vertexCount">Vertex count you would like the mesh to be at.</param>
        /// <returns>Closest vertex count to the proposedVertexCount, while still providing for a viable model of meshType type.</returns>
        public static int GetClosestViableVertexCount(MeshType meshType, int vertexCount)
        {
            return GetMeshCreator(meshType).GetClosestViableVertexCount(vertexCount);
        }

        #endregion



        #region Collider Mesh Creation

        /// <summary>
        /// Creates a default collider-optimized mesh of the given MeshType.
        /// </summary>
        /// <param name="meshType">MeshType to create.</param>
        /// <returns>Default collider-optimized Mesh of the given MeshType.</returns>
        public static Mesh CreateColliderMesh(MeshType meshType)
        {
            return GetMeshCreator(meshType).CreateColliderMesh();
        }

        /// <summary>
        /// Creates a collider-optimized mesh of the given MeshType customized by the given MeshDetails.
        /// </summary>
        /// <param name="meshType">MeshType to create.</param>
        /// <param name="meshDetails">Set of details to modify the mesh with.</param>
        /// <returns>Collider-optimized Mesh of the given MeshType customized with the MeshDetails.</returns>
        public static Mesh CreateColliderMesh(MeshType meshType, MeshCreator.MeshDetails meshDetails)
        {
            return GetMeshCreator(meshType).CreateColliderMesh(meshDetails);
        }

        #endregion



        #region Primitive Object Creation

        /// <summary>
        /// Creates a game object with a MeshType-specific mesh renderer and appropriate collider.
        /// </summary>
        /// <param name="meshType">Type of mesh to create.</param>
        public static GameObject CreatePrimitive(MeshType meshType)
        {
            //Create the object using ObjectFactory in-editor (allows for Undo operation) or GameObject in-build
#if UNITY_EDITOR
            GameObject obj = ObjectFactory.CreateGameObject(GetDisplayName(meshType));
#else
            GameObject obj = new GameObject(GetDisplayName(meshType));
#endif

            MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
            renderer.material = GetDefaultMaterial();

            MeshFilter filter = obj.AddComponent<MeshFilter>();
            filter.mesh = CreateMesh(meshType);

            obj = AddPrimitiveCollider(meshType, obj);

            return obj;
        }
        
        /// <summary>
        /// Gets the default Material to use, given the active render pipeline.
        /// </summary>
        /// <returns>Material to use </returns>
        public static Material GetDefaultMaterial()
        {
            if (GraphicsSettings.currentRenderPipeline == null) //no active scriptable render pipeline, use the default render pipeline
            {
                return new Material(Shader.Find("Standard"));
            }
            else //use the active render pipeline's default shader
            {
                return new Material(GraphicsSettings.currentRenderPipeline.defaultShader);
            }
        }

        #endregion



        #region Primitive Collider

        /// <summary>
        /// Adds the appropriate primitive collider for the given MeshType to the given GameObject
        /// </summary>
        public static GameObject AddPrimitiveCollider(MeshType meshType, GameObject obj)
        {
            switch (meshType)
            {
                default: //default is to just use a MeshCollider with the appropriate mesh
                    MeshCollider col = obj.AddComponent<MeshCollider>();
                    col.sharedMesh = CreateColliderMesh(meshType);
                    col.convex = true;
                    break;
            }

            return obj;
        }

        #endregion
    }
}