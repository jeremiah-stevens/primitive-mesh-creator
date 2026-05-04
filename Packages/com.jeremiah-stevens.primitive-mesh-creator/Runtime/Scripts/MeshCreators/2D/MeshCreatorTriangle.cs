using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PrimitiveCreator.PrimitiveCreatorUtility;

/*TODO:
 * • Options to define points custom for a tri (i.e. allow for very distorted tri)
 */
namespace PrimitiveCreator.MeshCreators
{
    public class MeshCreatorTriangle : MeshCreator
    {
        public override MeshType MeshType => MeshType.Triangle;
        public override string DisplayName => "Triangle";




        /// <summary>
        /// Create a version of a triangle mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Triangle mesh using the given parameters.</returns>
        public override Mesh CreateMesh(MeshDetails meshDetails)
        {
            Mesh framingMesh = new Mesh();
            CreateFramingVertices(ref framingMesh);

            int vertexCount = GetClosestViableVertexCount(meshDetails.vertexCount);

            Mesh outputMesh = CreateTriangle(ref framingMesh, vertexCount, 0, 1, 2);

            TransformMesh(ref outputMesh, meshDetails.translation, meshDetails.rotation, meshDetails.scale);

            return outputMesh;
        }

        /// <summary>
        /// Creates a version of the collider-optimized mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Collider-optimized mesh of the given type, using the given parameters.</returns>
        public override Mesh CreateColliderMesh(MeshDetails meshDetails)
        {
            int vertexCount = GetClosestViableVertexCount(0);

            /*Due to 3D physics requirements, mesh must be generated as a 3D mesh. As such, this will generate a prism with the smallest viable width of the main faces.
             *     1 - 4
             *    / \   \
             *   /   \   \
             *  0 --  2 - 3
             */

            float depthOffset = 0.001f; //width of the prism sides to ensure 3D physics requirements are met

            //set up the front side
            Mesh frontSide = new Mesh();
            CreateFramingVertices(ref frontSide);
            int[] triangles = new int[3];
            SetTri(triangles, 0, 0, 1, 2);
            frontSide.triangles = triangles;
            TransformMesh(ref frontSide, Vector3.forward * depthOffset, Quaternion.identity, Vector3.one);

            //set up the back side
            Mesh backSide = new Mesh();
            CreateFramingVertices(ref backSide);
            triangles = new int[3];
            SetTri(triangles, 0, 0, 1, 2);
            backSide.triangles = triangles;
            TransformMesh(ref backSide, -1 * Vector3.forward * depthOffset, Quaternion.Euler(new Vector3(0f, 180f, 0f)), Vector3.one);

            //combine the meshes to form the two faces
            Mesh outputMesh = new Mesh();
            CombineInstance[] combine = new CombineInstance[2];
            combine[0].mesh = frontSide;
            combine[1].mesh = backSide;
            combine[0].transform = Matrix4x4.identity;
            combine[1].transform = Matrix4x4.identity;
            outputMesh.CombineMeshes(combine, true);

            triangles = new int[(2 * 3) + (3 * 6)]; // 2 triangles for main faces, 3 quads for prism sides
            int t = 0;
            for (t = 0; t < outputMesh.triangles.Length; t++)
                triangles[t] = outputMesh.triangles[t];
            t = SetQuad(triangles, t, 2, 1, 4, 3);
            t = SetQuad(triangles, t, 1, 0, 5, 4);
            t = SetQuad(triangles, t, 0, 2, 3, 5);
            outputMesh.triangles = triangles;

            TransformMesh(ref outputMesh, meshDetails.translation, meshDetails.rotation, meshDetails.scale);

            return outputMesh;
        }

        /// <summary>
        /// Creates the framing mesh to generate the triangle onto.
        /// </summary>
        /// <param name="framingMesh">Mesh to write the vertices to.</param>
        private void CreateFramingVertices(ref Mesh framingMesh)
        {
            Vector3[] vertices = new Vector3[3];
            Vector2[] uv = new Vector2[vertices.Length];

            vertices[0] = new Vector3(0f, 0f);
            vertices[1] = new Vector3(0.5f, 1f);
            vertices[2] = new Vector3(1f, 0f);

            uv[0] = new Vector2(0f, 0f);
            uv[1] = new Vector2(0.5f, 1f);
            uv[2] = new Vector2(1f, 0f);

            Vector3 offset = new Vector3(-0.5f, -0.5f);
            for (int v = 0; v < vertices.Length; v++)
            {
                vertices[v] += offset;
            }

            framingMesh.vertices = vertices;
            framingMesh.uv = uv;
            framingMesh.RecalculateNormals();
            framingMesh.RecalculateTangents();
        }



        /// <summary>
        /// Gets the closest vertex count to the proposed vertex count that still provides for a viable model.
        /// </summary>
        /// <param name="proposedVertexCount">Vertex count you would like the mesh to be at.</param>
        /// <returns>Closest vertex count to the proposedVertexCount, while still providing for a viable model.</returns>
        public override int GetClosestViableVertexCount(int proposedVertexCount)
        {
            return GetTriangleVertexCount(proposedVertexCount);
        }
    }
}