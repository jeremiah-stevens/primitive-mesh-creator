using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PrimitiveCreator.PrimitiveCreatorUtility;


namespace PrimitiveCreator.MeshCreators
{
    public class MeshCreatorOctahedron : MeshCreator
    {
        public override MeshType MeshType => MeshType.Octahedron;
        public override string DisplayName => "Octahedron (d8)";




        /// <summary>
        /// Create a version of an octahedron (d8) mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Octahedron (d8) mesh using the given parameters.</returns>
        public override Mesh CreateMesh(MeshDetails meshDetails)
        {
            Mesh framingMesh = new Mesh();

            int boundedVertexCount = GetClosestViableVertexCount(meshDetails.vertexCount);
            int vertexCountPerSide = Mathf.FloorToInt(boundedVertexCount / 6);
            int faceCount = 8;

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
            int vertexCount = GetClosestViableVertexCount(0);

            Mesh outputMesh = new Mesh();
            CreateFramingMesh(ref outputMesh);

            TransformMesh(ref outputMesh, meshDetails.translation, meshDetails.rotation, meshDetails.scale);

            return outputMesh;
        }

        /// <summary>
        /// Creates the framing mesh to generate the octahedron onto.
        /// </summary>
        /// <param name="framingMesh">Mesh to write the vertices to.</param>
        private void CreateFramingMesh(ref Mesh framingMesh)
        {
            int vertexCount = 6;
            int faceCount = 8;

            Vector3[] vertices = new Vector3[faceCount * 3];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[faceCount * 3]; //8 faces, 1 tri per face (8 * 1)

            /*     0     <- (0, 0.5, 0)
             *    /| \
             *   4 --- 1 <- (-0.5, 0, 0)
             *  /     /
             * 3 --- 2 <- (0, 0, 0.5)
             *  \ | /
             *    5     <- (0, -0.5, 0)
             */
            float r = 0.5f;
            Vector3[] positions = new Vector3[vertexCount]; //6 framing vertices, faces are made out of these
            positions[0] = new Vector3(0, r, 0);
            positions[1] = new Vector3(-r, 0, 0);
            positions[2] = new Vector3(0, 0, -r);
            positions[3] = new Vector3(r, 0, 0);
            positions[4] = new Vector3(0, 0, r);
            positions[5] = new Vector3(0, -r, 0);

            int v = 0;
            v = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, v, 1, 0, 2);
            v = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, v, 2, 0, 3);
            v = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, v, 3, 0, 4);
            v = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, v, 4, 0, 1);
            v = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, v, 2, 5, 1);
            v = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, v, 3, 5, 2);
            v = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, v, 4, 5, 3);
            v = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, v, 1, 5, 4);

            framingMesh.vertices = vertices;
            framingMesh.uv = uvs;
            framingMesh.triangles = triangles;
            framingMesh.RecalculateNormals();
            framingMesh.RecalculateTangents();
        }

        /// <summary>
        /// Sets the vertices, uv coordinates, and triangles for a given face on the framing mesh.
        /// </summary>
        /// <returns>Next vertex index to write to</returns>
        private int SetFrameFace(ref Vector3[] positions, ref Vector3[] vertices, ref Vector2[] uvs, ref int[] triangles, int v, int v00, int v01, int v10)
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

            int minimumVertexCount = 24;
            if (proposedVertexCount <= minimumVertexCount) return minimumVertexCount;

            int proposedVerticesPerFace = Mathf.FloorToInt(proposedVertexCount / 8f);
            int vertsPerFace = PrimitiveCreatorUtility.GetClosestViableVertexCount(MeshType.Triangle, proposedVerticesPerFace);
            return vertsPerFace * 8;
        }
    }
}