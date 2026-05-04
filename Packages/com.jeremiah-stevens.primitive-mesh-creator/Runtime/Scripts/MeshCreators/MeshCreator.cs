using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


/* SetTri()
 *  - Make translation, rotation, and scale optional parameters
 */
namespace PrimitiveCreator.MeshCreators
{
    /// <summary>
    /// Base class for a procedural mesh creator.
    /// </summary>
    public abstract class MeshCreator
    {
        /// <summary>
        /// MeshType enum value to use for the given MeshCreator.
        /// </summary>
        public abstract PrimitiveCreatorUtility.MeshType MeshType { get; }

        /// <summary>
        /// Display name to use for the given MeshCreator.
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Gets the type of MeshDetails class to use 
        /// </summary>
        public virtual Type MeshDetailsType { get { return typeof(MeshDetails); } }

        /// <summary>
        /// Default version of the given mesh type.
        /// </summary>
        public Mesh DefaultMesh
        {
            get
            {
                if (_defaultMesh == null)
                    _defaultMesh = LoadDefaultMesh();
                return _defaultMesh;
            }
        }
        Mesh _defaultMesh;

        /// <summary>
        /// Default collider-optimized version of the given mesh type.
        /// </summary>
        public Mesh DefaultColliderMesh
        {
            get
            {
                if (_defaultColliderMesh == null)
                    _defaultColliderMesh = LoadDefaultColliderMesh();
                return _defaultColliderMesh;
            }
        }
        Mesh _defaultColliderMesh;




        public MeshCreator()
        {
            _defaultMesh = LoadDefaultMesh();
            _defaultColliderMesh = LoadDefaultColliderMesh();
        }

        /// <summary>
        /// Gets the default mesh from Resources (if one exists).
        /// </summary>
        private Mesh LoadDefaultMesh() { return Resources.Load<Mesh>($"{PrimitiveCreatorConfig.SaveSubpathMesh}/{DisplayName}"); }

        /// <summary>
        /// Gets the default mesh collider from Resources (if one exists).
        /// </summary>
        private Mesh LoadDefaultColliderMesh() { return Resources.Load<Mesh>($"{PrimitiveCreatorConfig.SaveSubpathColliders}/{DisplayName} (Collider)"); }




        #region Mesh Creation

        /// <summary>
        /// Gets a new MeshDetails class for the given MeshCreator.
        /// </summary>
        /// <param name="previousMeshDetails">Previous set of MeshDetails to copy to the new copy.</param>
        /// <returns>New MeshDetails class for the given MeshCreator.</returns>
        public MeshDetails GetMeshDetails(MeshDetails previousMeshDetails = null)
        {
            Type type = MeshDetailsType;
            ConstructorInfo ctor = type.GetConstructor(new[] { typeof(MeshDetails) });

            if (previousMeshDetails != null)
                return (MeshDetails)ctor.Invoke(new object[] { previousMeshDetails });
            else
                return (MeshDetails)ctor.Invoke(new object[] { null });
        }

        /// <summary>
        /// Creates a default version of the given mesh type.
        /// </summary>
        /// <returns>Mesh of the given mesh type, using standard parameters.</returns>
        public Mesh CreateMesh()
        {
            if (DefaultMesh != null) //if the default exists, use that instead
                return DefaultMesh;

            return CreateMesh(new MeshDetails());
        }

        /// <summary>
        /// Create a version of the given mesh type modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Mesh of the given type, using the given parameters.</returns>
        public abstract Mesh CreateMesh(MeshDetails meshDetails);

        /// <summary>
        /// Gets the closest vertex count to the proposed vertex count that still provides for a viable model.
        /// </summary>
        /// <param name="proposedVertexCount">Vertex count you would like the mesh to be at.</param>
        /// <returns>Closest vertex count to the proposedVertexCount, while still providing for a viable model.</returns>
        public abstract int GetClosestViableVertexCount(int proposedVertexCount);

        #endregion



        #region Collider Mesh Creation

        /// <summary>
        /// Creates a default version of the given mesh type's collider mesh.
        /// </summary>
        /// <returns>Collider-optimized version of the default mesh type.</returns>
        public Mesh CreateColliderMesh()
        {
            if (DefaultColliderMesh != null) //if the default exists, use that instead
                return DefaultColliderMesh;

            return CreateColliderMesh(new MeshDetails());
        }

        /// <summary>
        /// Creates a version of the collider-optimized mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Collider-optimized mesh of the given type, using the given parameters.</returns>
        public abstract Mesh CreateColliderMesh(MeshDetails meshDetails);

        #endregion



        #region Helper Methods

        /// <summary>
        /// Sets the indices in the triangle array to define the triangle set by the vertices v00, v01, v10 (in clockwise order).
        /// </summary>
        /// <param name="triangles">Array of triangles to write the indices to.</param>
        /// <param name="i">First index in the triangle array to define the triangle from.</param>
        /// <param name="v00">Bottom-left vertex index of the triangle.</param>
        /// <param name="v01">Top vertex index of the triangle.</param>
        /// <param name="v10">Bottom-right vertex index of the triangle.</param>
        /// <returns>Next empty triangle index to write to</returns>
        protected static int SetTri(int[] triangles, int i, int v00, int v01, int v10)
        {
            /*   __v01
             *    /|  \
             *   /    _\|
             * v00 <-- v10
             */
            triangles[i] = v00;
            triangles[i + 1] = v01;
            triangles[i + 2] = v10;
            return i + 3;
        }

        /// <summary>
        /// Sets the indices in the triangle array to define the quad set by the vertices v00, v01, v11, v10 (in clockwise order).
        /// </summary>
        /// <param name="triangles">Array of triangles to write the indices to.</param>
        /// <param name="i">First index in the triangle array to define the quad from.</param>
        /// <param name="v00">Bottom-left vertex index of the quad.</param>
        /// <param name="v01">Top-left vertex index of the quad.</param>
        /// <param name="v11">Top-right vertex index of the quad.</param>
        /// <param name="v10">Bottom-right vertex index of the quad.</param>
        /// <returns></returns>
        protected static int SetQuad(int[] triangles, int i, int v00, int v01, int v11, int v10)
        {
            /* Source: https://catlikecoding.com/unity/tutorials/rounded-cube/
             * v01 -- v11
             *  |   /  |
             *  |  /   |
             * v00 -- v10
             */
            i = SetTri(triangles, i, v00, v01, v11);
            i = SetTri(triangles, i, v11, v10, v00);
            return i;
        }

        /// <summary>
        /// Applies the provided set of transformation operations to the mesh in the following order (scale -> rotation -> translation).
        /// </summary>
        /// <param name="mesh">Mesh to transform.</param>
        /// <param name="translation">Translation operation to apply to the mesh.</param>
        /// <param name="rotation">Rotation operation to apply to the mesh.</param>
        /// <param name="scale">Scaling operation to apply to the mesh.</param>
        protected static void TransformMesh(ref Mesh mesh, Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            Vector3[] verts = new Vector3[mesh.vertexCount];
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 currVert = mesh.vertices[i];
                currVert = Vector3.Scale(currVert, scale);
                currVert = rotation * currVert;
                currVert += translation;

                verts[i] = currVert;
            }

            mesh.vertices = verts;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }

        #endregion



        #region Triangle Mesh Creation

        /// <summary>
        /// Create a triangle mesh bound by points (v00, v01, v10 in clockwise pattern) with up to vertexCount vertices.
        /// </summary>
        /// <param name="framingMesh">Sample mesh with the set of vertices that frame the requested triangle face.</param>
        /// <param name="vertexCount">Requested amount of vertices in the mesh (final count is bound between [minimumVertexCount, proposedVertexCount]).</param>
        /// <param name="v00">Bottom-left vertex index of the triangle.</param>
        /// <param name="v01">Top vertex index of the triangle.</param>
        /// <param name="v10">Bottom-right vertex index of the triangle.</param>
        /// <returns>New triangle mesh defined by the provided parameters.</returns>
        protected static Mesh CreateTriangle(ref Mesh framingMesh, int vertexCount, int v00, int v01, int v10)
        {
            Mesh outputMesh = new Mesh();

            int boundVertexCount = GetTriangleVertexCount(vertexCount);

            CreateTriangleVertices(ref outputMesh, ref framingMesh, boundVertexCount, v00, v01, v10);
            CreateTriangleTris(ref outputMesh);
            outputMesh.RecalculateNormals();
            outputMesh.RecalculateTangents();

            //TODO: re-enable for applying lightmap baking
            //#if UNITY_EDITOR
            //            Unwrapping.GenerateSecondaryUVSet(output);
            //#endif

            return outputMesh;
        }

        /// <summary>
        /// Creates the set of vertices for the requested triangle mesh.
        /// </summary>
        /// <param name="outputMesh">Mesh to write the vertices to.</param>
        /// <param name="framingMesh">Sample mesh with the set of vertices that frame the requested triangle face.</param>
        /// <param name="vertexCount">Number of vertices to produce in the triangle.</param>
        /// <param name="v00">Bottom-left vertex index of the triangle.</param>
        /// <param name="v01">Top vertex index of the triangle.</param>
        /// <param name="v10">Bottom-right vertex index of the triangle.</param>
        private static void CreateTriangleVertices(ref Mesh outputMesh, ref Mesh framingMesh, int vertexCount, int v00, int v01, int v10)
        {
            int sideEdges = GetTriangleSideSteps(vertexCount);

            Vector3[] vertices = new Vector3[vertexCount];
            Vector2[] uv = new Vector2[vertices.Length];

            /*       v01
             *      /   \
             *     /     \
             *    /       \
             *  v00 ------ v10
             */
            int i = 0;
            for (int y = 0; y <= sideEdges; y++)
            {
                float t_y = (float)y / (float)sideEdges;
                float margin = t_y / 2f;
                int xSteps = sideEdges - y;
                float p_y = Mathf.Lerp(0f, 1f, t_y);
                for (int x = 0; x <= xSteps; x++)
                {
                    float t_x = (xSteps > 0) ? (float)x / (float)xSteps : 1f;
                    float p_x = Mathf.Lerp(0f, 1 - p_y, t_x);
                    vertices[i] = MathExtensions.LerpBarycentric(p_x, p_y, framingMesh.vertices[v00], framingMesh.vertices[v10], framingMesh.vertices[v01]);
                    uv[i] = MathExtensions.LerpBarycentric(p_x, p_y, framingMesh.uv[v00], framingMesh.uv[v10], framingMesh.uv[v01]);
                    i++;
                }
            }

            outputMesh.vertices = vertices;
            outputMesh.uv = uv;

            return;
        }

        /// <summary>
        /// Creates the set of tris for the requested triangle mesh.
        /// </summary>
        /// <param name="outputMesh">Mesh to write the tris to.</param>
        private static void CreateTriangleTris(ref Mesh outputMesh)
        {
            int sideSteps = GetTriangleSideSteps(outputMesh.vertexCount);
            int triCount = GetTriangleTriCount(outputMesh.vertexCount);

            /*       v02
             *      /   \
             *    v01 - v11
             *   /   \ /   \
             * v00 - v10 - v20
             */
            int[] triangles = new int[triCount * 3];
            int t = 0, v = 0;

            for (int y = 0; y < sideSteps; y++, v++)
            {
                int xSteps = sideSteps - y;
                int row = xSteps + 1;
                for (int x = 0; x < xSteps; x++, v++)
                {
                    if (x < xSteps - 1)
                        t = SetQuad(triangles, t, v + 1, v, v + row, v + row + 1);
                    else
                    {
                        t = SetTri(triangles, t, v, v + row, v + 1);
                    }
                }
            }

            outputMesh.triangles = triangles;
        }

        /// <summary>
        /// Gets the closest vertex count to the proposed vertex count that still provides for a viable mesh (final count is bound between [minimumVertexCount, proposedVertexCount]).
        /// </summary>
        /// <param name="proposedVertexCount">Proposed vertex count for the given mesh.</param>
        /// <returns>Closest vertex count that still provides for a viable mesh.</returns>
        protected static int GetTriangleVertexCount(int proposedVertexCount)
        {
            proposedVertexCount = Mathf.Clamp(proposedVertexCount, PrimitiveCreatorConfig.MinimumVertexCount, PrimitiveCreatorConfig.MaximumVertexCount);

            int minimumVertexCount = 3;
            if (proposedVertexCount < minimumVertexCount) return minimumVertexCount;

            int sideWidth = GetTriangleSideSteps(proposedVertexCount);

            return Mathf.FloorToInt(0.5f * Mathf.Pow(sideWidth, 2) + 1.5f * sideWidth + 1);
        }

        /// <summary>
        /// Gets the amount of edges along the side of a triangle with vertexCount vertices.
        /// </summary>
        /// <param name="vertexCount">Amount of vertices in the triangle.</param>
        /// <returns>Amount of edges along the side of a triangle with vertexCount vertices.</returns>
        private static int GetTriangleSideSteps(int vertexCount)
        {
            /* Triangle can be viewed as a half of a full quad, provided we double the inside edge (|sideWidth|)
             * ([quad vertex count] + [sideWidth]) / 2 <= proposedVertexCount
             * ([(x + 1) * (x + 1)] + (x + 1)) / 2 <= proposedVertexCount
             * (x^2 + 2x + 1 + x + 1) / 2 <= proposedVertexCount
             * (x^2 + 3x + 2) / 2 <= proposedVertexCount
             * 0.5x^2 + 1.5x + 1 - proposedVertexCount <= 0
             */
            float rawSideWidth = Mathf.Max(MathExtensions.QuadraticFormula(0.5f, 1.5f, 1 - vertexCount));
            int sideWidth = Mathf.FloorToInt(rawSideWidth);

            return sideWidth;
        }

        /// <summary>
        /// Gets the amount of tris of a triangle with vertexCount vertices.
        /// </summary>
        /// <param name="vertexCount">Amount of vertices in the triangle.</param>
        /// <returns>Amount of tris in a triangle with vertexCount vertices.</returns>
        private static int GetTriangleTriCount(int vertexCount)
        {
            int sideSteps = GetTriangleSideSteps(vertexCount);

            return sideSteps * sideSteps;
        }

        #endregion



        #region Quad Mesh Creation

        /// <summary>
        /// Create a quad mesh bound by points (v00, v01, v11, v10 in clockwise pattern) with up to vertexCount vertices
        /// </summary>
        /// <param name="framingMesh">Sample mesh of points to write</param>
        /// <param name="vertexCount">Requested amount of vertices in the mesh (final count is bound between [minimumVertexCount, proposedVertexCount]).</param>
        /// <param name="v00">Bottom-left bounding index of the requested quad.</param>
        /// <param name="v01">Top-left index of the requested quad.</param>
        /// <param name="v11">Top-right index of the requested quad.</param>
        /// <param name="v10">Bottom-right index of the requested quad.</param>
        /// <returns>New quad mesh defined by the provided parameters.</returns>
        protected static Mesh CreateQuad(ref Mesh framingMesh, int vertexCount, int v00, int v01, int v11, int v10)
        {
            Mesh output = new Mesh();

            int boundVertexCount = GetQuadVertexCount(vertexCount);

            CreateQuadVertices(ref output, ref framingMesh, boundVertexCount, v00, v01, v11, v10);
            CreateQuadTris(ref output);
            output.RecalculateNormals();
            output.RecalculateTangents();

            //TODO: re-enable for applying lightmap baking
            //#if UNITY_EDITOR
            //            Unwrapping.GenerateSecondaryUVSet(output);
            //#endif

            return output;
        }

        /// <summary>
        /// Creates the set of vertices for the requested quad mesh.
        /// </summary>
        /// <param name="outputMesh">Mesh to write the vertices to.</param>
        /// <param name="framingMesh">Sample mesh with the set of vertices that frame the requested quad face.</param>
        /// <param name="vertexCount">Number of vertices to produce in the quad.</param>
        /// <param name="v00">Bottom-left vertex index of the quad.</param>
        /// <param name="v01">Top-left vertex index of the quad.</param>
        /// <param name="v11">Top-right vertex index of the quad.</param>
        /// <param name="v10">Bottom-right vertex index of the quad.</param>
        private static void CreateQuadVertices(ref Mesh outputMesh, ref Mesh framingMesh, int vertexCount, int v00, int v01, int v11, int v10)
        {
            int sideSteps = Mathf.FloorToInt(Mathf.Sqrt(vertexCount)) - 1;

            Vector3[] vertices = new Vector3[(sideSteps + 1) * (sideSteps + 1)];
            Vector2[] uv = new Vector2[vertices.Length];

            int i = 0;
            for (int y = 0; y <= sideSteps; y++)
            {
                float p_y = (float)y / (float)sideSteps;
                for (int x = 0; x <= sideSteps; x++)
                {
                    float p_x = (float)x / (float)sideSteps;
                    vertices[i] = MathExtensions.LerpBilinear(p_x, p_y, framingMesh.vertices[v00], framingMesh.vertices[v10], framingMesh.vertices[v11], framingMesh.vertices[v01]);
                    uv[i] = MathExtensions.LerpBilinear(p_x, p_y, framingMesh.uv[v00], framingMesh.uv[v10], framingMesh.uv[v11], framingMesh.uv[v01]);
                    i++;
                }
            }

            outputMesh.vertices = vertices;
            outputMesh.uv = uv;
        }

        /// <summary>
        /// Creates the set of tris for the requested quad mesh.
        /// </summary>
        /// <param name="mesh">Mesh to write the tris to.</param>
        private static void CreateQuadTris(ref Mesh outputMesh)
        {
            int vertexCount = outputMesh.vertexCount;
            int sideSteps = Mathf.FloorToInt(Mathf.Sqrt(vertexCount)) - 1;

            int quadCount = sideSteps * sideSteps;
            int[] triangles = new int[quadCount * 6];
            int row = sideSteps + 1;
            int t = 0, v = 0;

            for (int y = 0; y < sideSteps; y++, v++)
            {
                for (int x = 0; x < sideSteps; x++, v++)
                {
                    t = SetQuad(triangles, t, v, v + row, v + row + 1, v + 1);
                }
            }

            outputMesh.triangles = triangles;
        }

        /// <summary>
        /// Gets the closest vertex count to the proposed vertex count that still provides for a viable mesh (final count is bound between [minimumVertexCount, proposedVertexCount]).
        /// </summary>
        /// <param name="proposedVertexCount">Proposed vertex count for the given mesh.</param>
        /// <returns>Closest vertex count that still provides for a viable mesh.</returns>
        protected static int GetQuadVertexCount(int proposedVertexCount)
        {
            proposedVertexCount = Mathf.Clamp(proposedVertexCount, PrimitiveCreatorConfig.MinimumVertexCount, PrimitiveCreatorConfig.MaximumVertexCount);

            int minimumVertexCount = 4;
            if (proposedVertexCount < minimumVertexCount) return minimumVertexCount;
            else
            {
                //(x + 1) * (y + 1) <= proposedVertexCount where x = y (square shape) -> x^2 + 2x + (1 - proposedVertexCount) <= 0
                float rawSideWidth = Mathf.Max(MathExtensions.QuadraticFormula(1f, 2f, 1f - proposedVertexCount));
                int sideWidth = Mathf.FloorToInt(rawSideWidth);

                return (sideWidth + 1) * (sideWidth + 1);
            }
        }

        #endregion




        /// <summary>
        /// Base set of directions for creating a procedural mesh. Can be extended for each particular mesh (ex: radius of a cylinder).
        /// </summary>
        [System.Serializable]
        public class MeshDetails
        {
            public Vector3 translation;
            public Quaternion rotation;
            public Vector3 scale;
            [Range(PrimitiveCreatorConfig.MinimumVertexCount, PrimitiveCreatorConfig.MaximumVertexCount)] public int vertexCount;




            public MeshDetails() { Initialize(null); }

            public MeshDetails(MeshDetails prevMeshDetails = null) { Initialize(prevMeshDetails); }

            /// <summary>
            /// Create a new set of mesh details based on the provided parameters.
            /// </summary>
            /// <param name="translation">Translation to apply to the entire mesh upon completion.</param>
            /// <param name="rotation">Rotation to apply to the entire mesh upon completion.</param>
            /// <param name="scale">Scaling to apply to the entire mesh upon completion.</param>
            /// <param name="vertexCount">Requested number of vertices in the model (final count is bound between [minimumVertexCount, proposedVertexCount]).</param>
            public MeshDetails(Vector3 translation, Quaternion rotation, Vector3 scale, int vertexCount)
            {
                Initialize(translation, rotation, scale, vertexCount);
            }




            protected virtual void Initialize(MeshDetails prevMeshDetails = null)
            {
                if (prevMeshDetails != null)
                    Initialize(prevMeshDetails.translation, prevMeshDetails.rotation, prevMeshDetails.scale, prevMeshDetails.vertexCount);
                else
                    Initialize(Vector3.zero, Quaternion.identity, Vector3.one, 0);
            }

            protected virtual void Initialize(Vector3 translation, Quaternion rotation, Vector3 scale, int vertexCount)
            {
                this.translation = translation;
                this.rotation = rotation;
                this.scale = scale;
                this.vertexCount = vertexCount;
            }
        }
    }
}