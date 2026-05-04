using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PrimitiveCreator.PrimitiveCreatorUtility;


namespace PrimitiveCreator.MeshCreators
{
    public class MeshCreatorPyramid : MeshCreator
    {
        public override MeshType MeshType => MeshType.Pyramid;
        public override string DisplayName => "Pyramid";




        /// <summary>
        /// Create a version of a pyramid mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Pyramid mesh using the given parameters.</returns>
        public override Mesh CreateMesh(MeshDetails meshDetails)
        {
            Mesh framingMesh = new Mesh();

            int vertexCount = GetClosestViableVertexCount(meshDetails.vertexCount);
            int sideWidth = GetSideWidth(vertexCount);
            int vertsPerQuadFace = (sideWidth + 1) * (sideWidth + 1);
            int vertsPerTriFace = (vertexCount - vertsPerQuadFace) / 4;
            int faceTriCount = 4;
            int faceQuadCount = 1;

            CreateFramingMesh(ref framingMesh);

            CombineInstance[] combine = new CombineInstance[faceTriCount + faceQuadCount];
            int t = 0;
            int f = 0;
            for (int i = 0; i < faceTriCount; i++)
            {
                combine[f].mesh = CreateTriangle(ref framingMesh, vertsPerTriFace, framingMesh.triangles[t], framingMesh.triangles[t + 1], framingMesh.triangles[t + 2]);
                combine[f].transform = Matrix4x4.identity;
                t += 3;
                f++;
            }
            for (int i = 0; i < faceQuadCount; i++)
            {
                combine[f].mesh = CreateQuad(ref framingMesh, vertsPerQuadFace, framingMesh.triangles[t], framingMesh.triangles[t + 1], framingMesh.triangles[t + 2], framingMesh.triangles[t + 4]);
                combine[f].transform = Matrix4x4.identity;
                t += 6;
                f++;
            }

            Mesh output = new Mesh();
            output.CombineMeshes(combine, true);

            //perform the transformation operations on the final mesh
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
        /// Creates the framing mesh to write the vertices to.
        /// </summary>
        /// <param name="mesh">Mesh to write the framing mesh to.</param>
        private void CreateFramingMesh(ref Mesh mesh)
        {
            int vertexCount = 5;
            int faceTriCount = 4;  //4 tris
            int faceQuadCount = 1; //1 quad

            Vector3[] vertices = new Vector3[faceTriCount * 3 + faceQuadCount * 4];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[faceTriCount * 3 + faceQuadCount * 6]; //1 tri per tri face, 2 tri per quad face

            /*     0 <- (0.5, 1, 0.5)
             *    /| \
             *   4 --- 1 <- (0, 0, 0)
             *  /     /
             * 3 --- 2 <- (0, 0, 1)
             */
            Vector3[] positions = new Vector3[vertexCount];
            positions[0] = new Vector3(0.5f, 1f, 0.5f);
            positions[1] = Vector3.zero;
            positions[2] = new Vector3(0, 0, 1);
            positions[3] = new Vector3(1, 0, 1);
            positions[4] = new Vector3(1, 0, 0);

            int v = 0;
            v = SetFrameFaceTri(ref positions, ref vertices, ref uvs, ref triangles, v, 2, 0, 1);
            v = SetFrameFaceTri(ref positions, ref vertices, ref uvs, ref triangles, v, 3, 0, 2);
            v = SetFrameFaceTri(ref positions, ref vertices, ref uvs, ref triangles, v, 4, 0, 3);
            v = SetFrameFaceTri(ref positions, ref vertices, ref uvs, ref triangles, v, 1, 0, 4);
            v = SetFrameFaceQuad(ref positions, ref vertices, ref uvs, ref triangles, v, 4, 3, 2, 1);

            Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] += offset;

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.RecalculateTangents();
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

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

        //TODO: currently order dependent on tris being created first. Should allow some way of carrying through v and t
        private int SetFrameFaceQuad(ref Vector3[] positions, ref Vector3[] vertices, ref Vector2[] uvs, ref int[] triangles, int v, int v00, int v01, int v11, int v10)
        {
            vertices[v] = positions[v00];
            vertices[v + 1] = positions[v01];
            vertices[v + 2] = positions[v11];
            vertices[v + 3] = positions[v10];

            uvs[v] = new Vector2(0, 0);
            uvs[v + 1] = new Vector2(0, 1);
            uvs[v + 2] = new Vector2(1, 1);
            uvs[v + 3] = new Vector2(1, 0);

            int t = v;
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

            int minimumVertexCount = 16; //4 triangles, 1 quad (4 * 3 + 1 * 4 = 16)
            if (proposedVertexCount < minimumVertexCount) return minimumVertexCount;

            int sideWidth = GetSideWidth(proposedVertexCount);

            return Mathf.RoundToInt(3 * Mathf.Pow(sideWidth, 2) + 8 * sideWidth + 5);
        }

        /// <summary>
        /// Gets the number of edges on the sides of each face.
        /// </summary>
        /// <param name="proposedVertexCount">Proposed vertex count for the entire mesh.</param>
        /// <returns>Closest number of edges to the proposed vertex count.</returns>
        private int GetSideWidth(int proposedVertexCount)
        {
            /* get the side width (i.e. the number of edges)
             * 1 * quad + 4 * triangle <= proposedVertexCount
             * 1 * (x+1)^2 + 4 * (0.5x^2 + 1.5x + 1) <= proposedVertexCount
             * (x^2 + 2x + 1) + (2x^2 + 6x + 4) <= proposedVertexCount
             * 3x^2 + 8x + 5 - proposedVertexCount <= 0
             */
            float rawSideWidth = Mathf.Max(MathExtensions.QuadraticFormula(3, 8, 5 - proposedVertexCount));
            int sideWidth = Mathf.FloorToInt(rawSideWidth);

            return sideWidth;
        }
    }
}