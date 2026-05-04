using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PrimitiveCreator.PrimitiveCreatorUtility;


namespace PrimitiveCreator.MeshCreators
{
    public class MeshCreatorTetrahedron : MeshCreator
    {
        public override MeshType MeshType => MeshType.Tetrahedron;
        public override string DisplayName => "Tetrahedron (d4)";




        /// <summary>
        /// Create a version of a Tetrahedron (d4) mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Tetrahedron (d4) mesh using the given parameters.</returns>
        public override Mesh CreateMesh(MeshDetails meshDetails)
        {
            Mesh framingMesh = new Mesh();

            int vertexCount = GetClosestViableVertexCount(meshDetails.vertexCount);
            int vertexCountPerSide = Mathf.FloorToInt(vertexCount / 4);
            int faceCount = 4;

            CreateFramingMesh(ref framingMesh);

            CombineInstance[] combine = new CombineInstance[faceCount];
            int t = 0;
            for (int i = 0; i < faceCount; i++)
            {
                combine[i].mesh = CreateTriangle(ref framingMesh, vertexCountPerSide, framingMesh.triangles[t], framingMesh.triangles[t + 1], framingMesh.triangles[t + 2]);
                combine[i].transform = Matrix4x4.identity;
                t += 3;
            }
            Mesh output = new Mesh();
            output.CombineMeshes(combine, true);

            TransformMesh(ref output, meshDetails.translation, meshDetails.rotation, meshDetails.scale);

            return output;
        }

        /// <summary>
        /// Creates a version of the collider-optimized mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Collider-optimized mesh of the given type, using the given parameters.</returns>
        public override Mesh CreateColliderMesh(MeshDetails meshDetails)
        {
            Mesh outputMesh = new Mesh();
            CreateFramingMesh(ref outputMesh);

            TransformMesh(ref outputMesh, meshDetails.translation, meshDetails.rotation, meshDetails.scale);

            return outputMesh;
        }

        /// <summary>
        /// Creates the framing mesh to generate the tris onto.
        /// </summary>
        /// <param name="mesh">Mesh to write the framing faces to.</param>
        private void CreateFramingMesh(ref Mesh mesh)
        {
            int vertexCount = 4;
            int faceCount = 4;

            Vector3[] vertices = new Vector3[faceCount * 3];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[faceCount * 3]; //4 faces, 1 tri per face (4 * 1)


            /* For a tetrahedron, you can embed it inside of a cube in two ways. This algorithm takes
             * advantage of this, then rotates the model so that vertex 0 is in the forward direction:
             * https://en.wikipedia.org/wiki/Tetrahedron (see Geometric relations)
             *
             * (top-down view)
             *     3
             *    /| \
             *   / 0  \
             *  / /  \ \
             * 1 ------ 2
             */

            Vector3[] positions = new Vector3[vertexCount]; //4 framing vertices, faces are made out of these

            float r = 0.5f; //radius of the circle (0.5f for now)

            float a = 1f / 3f;
            float b = Mathf.Sqrt(8f / 9f);
            float c = Mathf.Sqrt(2f / 9f);
            float d = Mathf.Sqrt(2f / 3f);

            positions[0] = new Vector3(b, 0f, -a);
            positions[1] = new Vector3(-c, d, -a);
            positions[2] = new Vector3(-c, -d, -a);
            positions[3] = new Vector3(0f, 0f, 1f);


            int v = 0;
            v = SetFrameFaceTri(ref positions, ref vertices, ref uvs, ref triangles, v, 1, 0, 2);
            v = SetFrameFaceTri(ref positions, ref vertices, ref uvs, ref triangles, v, 2, 0, 3);
            v = SetFrameFaceTri(ref positions, ref vertices, ref uvs, ref triangles, v, 3, 0, 1);
            v = SetFrameFaceTri(ref positions, ref vertices, ref uvs, ref triangles, v, 2, 3, 1);

            //rotates all points so that vertex 0 is in the forward direction
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = Quaternion.Euler(new Vector3(20f, 0f, 90f)) * vertices[i];

            for (int i = 0; i < vertices.Length; i++)
                vertices[i] *= r;

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }

        /// <summary>
        /// Sets a framing triangle face on the mesh.
        /// </summary>
        /// <param name="positions">Set of vertices to use in generating the faces (i.e. the 'positions' used to create the faces).</param>
        /// <param name="vertices">Array of vertices in the mesh.</param>
        /// <param name="uvs">Array of uv coordinates in the mesh.</param>
        /// <param name="triangles">Array of tri indices in the mesh.</param>
        /// <param name="v">Starting vertex index.</param>
        /// <param name="v00">Bottom-left index of the triangle.</param>
        /// <param name="v01">Top index of the triangle.</param>
        /// <param name="v10">Buttom-right index of the triangle.</param>
        /// <returns>Next vertex index to write to in the mesh.</returns>
        private int SetFrameFaceTri(ref Vector3[] positions, ref Vector3[] vertices, ref Vector2[] uvs, ref int[] triangles, int v, int v00, int v01, int v10)
        {
            vertices[v] = positions[v00];
            vertices[v + 1] = positions[v01];
            vertices[v + 2] = positions[v10];

            uvs[v] = new Vector2(0, 0);
            uvs[v + 1] = new Vector2(0.5f, 1f);
            uvs[v + 2] = new Vector2(1, 0);

            int t = v;
            SetTri(triangles, t, v, v + 1, v + 2);

            return v + 3;
        }

        /// <summary>
        /// Gets the closest vertex count to the proposed vertex count that still provides for a viable model.
        /// </summary>
        /// <param name="proposedVertexCount">Vertex count you would like the mesh to be at.</param>
        /// <returns>Closest vertex count to the proposedVertexCount, while still providing for a viable model.</returns>
        public override int GetClosestViableVertexCount(int proposedVertexCount)
        {
            proposedVertexCount = Mathf.Clamp(proposedVertexCount, PrimitiveCreatorConfig.MinimumVertexCount, PrimitiveCreatorConfig.MaximumVertexCount);

            int minimumVertexCount = 12;
            if (proposedVertexCount <= minimumVertexCount) return minimumVertexCount;

            int proposedVerticesPerFace = Mathf.FloorToInt(proposedVertexCount / 4f);
            int vertsPerFace = PrimitiveCreatorUtility.GetClosestViableVertexCount(MeshType.Triangle, proposedVerticesPerFace);
            return vertsPerFace * 4;
        }
    }
}