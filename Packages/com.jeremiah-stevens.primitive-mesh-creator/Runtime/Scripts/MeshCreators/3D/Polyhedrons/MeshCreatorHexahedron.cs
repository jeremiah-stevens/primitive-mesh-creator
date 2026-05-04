using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PrimitiveCreator.PrimitiveCreatorUtility;


namespace PrimitiveCreator.MeshCreators
{
    /// <summary>
    /// MeshCreator for a six-sided polyhedron. Varies from a Cube in that it is bound to a Unit Sphere, not a Unit Cube.
    /// </summary>
    public class MeshCreatorHexahedron : MeshCreator
    {
        public override MeshType MeshType => MeshType.Hexahedron;
        public override string DisplayName => "Hexahedron (d6)";




        /// <summary>
        /// Create a version of a Hexahedron (d6) mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Hexahedron (d6) mesh using the given parameters.</returns>
        public override Mesh CreateMesh(MeshDetails meshDetails)
        {
            Mesh framingMesh = new Mesh();

            int vertexCount = GetClosestViableVertexCount(meshDetails.vertexCount);
            int faceCount = 6;
            int vertexCountPerSide = Mathf.FloorToInt(vertexCount / faceCount);

            CreateFramingMesh(ref framingMesh);

            CombineInstance[] combine = new CombineInstance[faceCount];
            int t = 0;
            for (int i = 0; i < faceCount; i++)
            {
                combine[i].mesh = CreateQuad(ref framingMesh, vertexCountPerSide, framingMesh.triangles[t], framingMesh.triangles[t + 1], framingMesh.triangles[t + 2], framingMesh.triangles[t + 4]);
                combine[i].transform = Matrix4x4.identity;
                t += 6;
            }

            Mesh output = new Mesh();
            output.CombineMeshes(combine, true);

            //perform the transformation operations on the final mesh
            TransformMesh(ref output, meshDetails.translation, meshDetails.rotation, meshDetails.scale);

            return output;
        }
        
        public override Mesh CreateColliderMesh(MeshDetails meshDetails)
        {
            int vertexCount = GetClosestViableVertexCount(0);

            Mesh outputMesh = new Mesh();
            CreateFramingMesh(ref outputMesh);

            TransformMesh(ref outputMesh, meshDetails.translation, meshDetails.rotation, meshDetails.scale);

            return outputMesh;
        }

        /// <summary>
        /// Creates the framing mesh to generate the shape onto.
        /// </summary>
        /// <param name="framingMesh">Mesh to write the vertices to.</param>
        private void CreateFramingMesh(ref Mesh mesh)
        {
            int vertexCount = 8;
            int faceCount = 6;

            Vector3[] vertices = new Vector3[faceCount * 4];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[faceCount * 6]; //6 faces, 2 tris per face (6 * 2 * 3)

            /*   3 --- 0 <- (-r, r, -r)
             *  /     /|
             * 2 --- 1 |
             * | 7---|-4 <- (-r, -r, -r)
             * |/    |/
             * 6 --- 5 <- (-r, -r, r)
             */

            /* the radius of the circumscribed sphere is sqrt(3) / 2 * the edge-length
             * 1/2 = sqrt(3) / 2 * e
             * e = 1 / sqrt(3)
             */
            float r = 1 / Mathf.Sqrt(3);
            Vector3[] positions = new Vector3[vertexCount]; //8 framing vertices, faces are made out of these
            positions[0] = new Vector3(0f, r, 0f);
            positions[1] = new Vector3(0f, r, r);
            positions[2] = new Vector3(r, r, r);
            positions[3] = new Vector3(r, r, 0f);
            positions[4] = new Vector3(0f, 0f, 0f);
            positions[5] = new Vector3(0f, 0f, r);
            positions[6] = new Vector3(r, 0f, r);
            positions[7] = new Vector3(r, 0f, 0f);

            int i = 0;
            i = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, i, 0, 1, 2, 3);
            i = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, i, 4, 0, 3, 7);
            i = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, i, 7, 3, 2, 6);
            i = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, i, 6, 2, 1, 5);
            i = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, i, 5, 1, 0, 4);
            i = SetFrameFace(ref positions, ref vertices, ref uvs, ref triangles, i, 7, 6, 5, 4);

            Vector3 offset = new Vector3(-r / 2f, -r / 2f, -r / 2f);
            for (int v = 0; v < vertices.Length; v++)
                vertices[v] += offset;

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.RecalculateTangents();
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        /// <summary>
        /// Sets the vertices, uv coordinates, and triangles for a given face on the framing mesh.
        /// </summary>
        /// <returns>Next vertex index to write to</returns>
        private int SetFrameFace(ref Vector3[] positions, ref Vector3[] vertices, ref Vector2[] uvs, ref int[] triangles, int v, int v00, int v01, int v11, int v10)
        {
            vertices[v] = positions[v00];
            vertices[v + 1] = positions[v01];
            vertices[v + 2] = positions[v11];
            vertices[v + 3] = positions[v10];

            uvs[v] = new Vector2(0, 0);
            uvs[v + 1] = new Vector2(0, 1);
            uvs[v + 2] = new Vector2(1, 1);
            uvs[v + 3] = new Vector2(1, 0);

            int t = (v / 4) * 6;
            SetQuad(triangles, t, v, v + 1, v + 2, v + 3);

            return v + 4;
        }



        /// <summary>
        /// Gets the closest vertex count to the proposed vertex count that still provides for a viable model.
        /// </summary>
        /// <param name="proposedVertexCount">Vertex count you would like the mesh to be at.</param>
        /// <returns>Closest vertex count to the proposedVertexCount, while still providing for a viable model.</returns>
        public override int GetClosestViableVertexCount(int proposedVertexCount)
        {
            proposedVertexCount = Mathf.Clamp(proposedVertexCount, PrimitiveCreatorConfig.MinimumVertexCount, PrimitiveCreatorConfig.MaximumVertexCount);

            int minimumVertexCount = 24; //4 vertices * 6 faces -> 24
            if (proposedVertexCount < minimumVertexCount) return minimumVertexCount;
            else
            {
                int verticesOnSide = Mathf.FloorToInt((float)proposedVertexCount / 6f);
                return PrimitiveCreatorUtility.GetClosestViableVertexCount(PrimitiveCreatorUtility.MeshType.Quad, verticesOnSide) * 6;
            }
        }
    }
}