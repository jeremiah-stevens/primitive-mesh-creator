using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PrimitiveCreator.PrimitiveCreatorUtility;


/*TODO:
 * � Option for angle of coverage (ex: hemisphere either by parallel lines or meridian)
 */
namespace PrimitiveCreator.MeshCreators
{
    public class MeshCreatorSphereUV : MeshCreator
    {
        public override MeshType MeshType => MeshType.SphereUV;
        public override string DisplayName => "Sphere (UV)";
        public override Type MeshDetailsType => typeof(MeshDetailsSphereUV);




        /// <summary>
        /// Create a version of a Sphere (UV) mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Sphere (UV) mesh using the given parameters.</returns>
        public override Mesh CreateMesh(MeshDetails meshDetails)
        {
            return CreateMesh(new MeshDetailsSphereUV(meshDetails));
        }

        /// <summary>
        /// Create a version of a Sphere (UV) mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Sphere (UV) mesh using the given parameters.</returns>
        public Mesh CreateMesh(MeshDetailsSphereUV meshDetails)
        {
            Mesh mesh = new Mesh();

            int vertexCount = GetClosestViableVertexCount(meshDetails.vertexCount);
            int meridianCount = Mathf.FloorToInt(Mathf.Sqrt(vertexCount));
            int parallelCount = meridianCount;

            CreateVertices(ref mesh, meridianCount, parallelCount, vertexCount);
            CreateFaces(ref mesh, meridianCount, parallelCount);

            TransformMesh(ref mesh, meshDetails.translation, meshDetails.rotation, meshDetails.scale);

            return mesh;
        }

        /// <summary>
        /// Creates a version of the collider-optimized mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Collider-optimized mesh of the given type, using the given parameters.</returns>
        public override Mesh CreateColliderMesh(MeshDetails meshDetails)
        {
            return CreateColliderMesh(new MeshDetailsSphereUV(meshDetails));
        }

        /// <summary>
        /// Creates a version of the collider-optimized mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Collider-optimized mesh of the given type, using the given parameters.</returns>
        public Mesh CreateColliderMesh(MeshDetailsSphereUV meshDetails)
        {
            Mesh mesh = new Mesh();

            int vertexCount = GetClosestViableVertexCount(500);
            int meridianCount = Mathf.FloorToInt(Mathf.Sqrt(vertexCount));
            int parallelCount = meridianCount;

            CreateVertices(ref mesh, meridianCount, parallelCount, vertexCount);
            CreateFaces(ref mesh, meridianCount, parallelCount);

            TransformMesh(ref mesh, meshDetails.translation, meshDetails.rotation, meshDetails.scale);

            return mesh;
        }

        /// <summary>
        /// Creates the vertices for the mesh.
        /// </summary>
        /// <param name="mesh">Mesh to write the vertices to.</param>
        /// <param name="meridianCount">Number of vertical steps for the shape ('latitude').</param>
        /// <param name="parallelCount">Number of horizontal steps for the shape ('longitude').</param>
        /// <param name="vertexCount">Number of vertices to use in the generation of the mesh.</param>
        private void CreateVertices(ref Mesh mesh, int meridianCount, int parallelCount, int vertexCount)
        {
            Vector3[] vertices = new Vector3[meridianCount * (parallelCount + 1)];
            Vector2[] uvs = new Vector2[vertices.Length];

            int v = 0;

            for(int p = 0; p <= parallelCount; p++) //bottom ring (tris)
            {
                float stepOffset = (1f / (float)parallelCount) * 0.5f;
                float t_p = (float)p / (float)parallelCount;
                float theta = 2f * Mathf.PI * t_p;
                vertices[v] = MathExtensions.SphericalToCartesian(1f, theta, 0f);
                uvs[v] = new Vector2(Mathf.Clamp(t_p + stepOffset, 0f, 1f), 0f);
                v++;
            }

            for (int m = 1; m < meridianCount - 1; m++) //middle rings (quads)
            {
                float t_m = (float)m / (float)(meridianCount - 1);
                float phi = Mathf.PI * t_m;
                for (int p = 0; p <= parallelCount; p++)
                {
                    float t_p = (float)p / (float)parallelCount;
                    float theta = 2f * Mathf.PI * t_p;
                    vertices[v] = MathExtensions.SphericalToCartesian(1f, theta, phi);
                    uvs[v] = new Vector2(t_p, t_m);
                    v++;
                }
            }

            for (int p = 0; p <= parallelCount; p++) //top ring (tris)
            {
                float stepOffset = (1f / (float)parallelCount) * 0.5f;
                float t_p = (float)p / (float)parallelCount;
                float theta = 2f * Mathf.PI * t_p;
                vertices[v] = MathExtensions.SphericalToCartesian(1f, theta, Mathf.PI);
                uvs[v] = new Vector2(Mathf.Clamp(t_p + stepOffset, 0f, 1f), 1f);
                v++;
            }


            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Quaternion.AngleAxis(90f, Vector3.right) * vertices[i];
                vertices[i] = Quaternion.AngleAxis(270f, Vector3.up) * vertices[i];
                vertices[i] *= 0.5f;
            }

            mesh.vertices = vertices;
            mesh.uv = uvs;
        }

        /// <summary>
        /// Creates the faces of the mesh (triangles, normals, tangents).
        /// </summary>
        /// <param name="mesh">Mesh to write the faces to.</param>
        /// <param name="meridianCount">Number of vertical steps for the shape ('latitude').</param>
        /// <param name="parallelCount">Number of horizontal steps for the shape ('longitude').</param>
        private void CreateFaces(ref Mesh mesh, int meridianCount, int parallelCount)
        {
            int quadRings = meridianCount - 2;
            int triRings = 2;
            int[] triangles = new int[((quadRings - 1) * parallelCount) * 6 + (triRings * parallelCount) * 3];
            int t = 0;
            int parallelVertexCount = parallelCount + 1; //number of vertices in a parallel line (parallelCount + 1 for the overlap)

            for(int p = 0; p < parallelCount; p++) //bottom ring (tris)
            {
                int v00 = parallelVertexCount + (p + 1) % (parallelVertexCount);
                int v01 = p;
                int v10 = parallelVertexCount + p;
                t = SetTri(triangles, t, v00, v01, v10);
            }

            for (int m = 1; m < meridianCount - 2; m++) //middle rings (quads)
            {
                for (int p = 0; p < parallelCount; p++) //for each parallel vert in the current meridian shell
                {
                    int v00 = m * parallelVertexCount + p;
                    int v01 = (m + 1) * (parallelVertexCount) + p;
                    int v11 = (m + 1) * (parallelVertexCount) + (p + 1) % parallelVertexCount;
                    int v10 = m * parallelVertexCount + (p + 1) % parallelVertexCount;
                    t = SetQuad(triangles, t, v00, v01, v11, v10);
                }
            }

            for (int p = 0; p < parallelCount; p++) //top ring (tris)
            {
                int v00 = (mesh.vertexCount - parallelVertexCount * 2) + p;
                int v01 = (mesh.vertexCount - parallelVertexCount) + p;
                int v10 = (mesh.vertexCount - parallelVertexCount * 2) + (p + 1) % parallelVertexCount;
                t = SetTri(triangles, t, v00, v01, v10);
            }

            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }



        /// <summary>
        /// Gets the closest vertex count to the proposed vertex count that still provides for a viable model.
        /// </summary>
        /// <param name="proposedVertexCount">Vertex count you would like the mesh to be at.</param>
        /// <returns>Closest vertex count to the proposedVertexCount, while still providing for a viable model.</returns>
        public override int GetClosestViableVertexCount(int proposedVertexCount)
        {
            proposedVertexCount = Mathf.Clamp(proposedVertexCount, PrimitiveCreatorConfig.MinimumVertexCount, PrimitiveCreatorConfig.MaximumVertexCount);

            int minimumVertexCount = 30;
            if (proposedVertexCount < minimumVertexCount) return minimumVertexCount;

            int t = Mathf.FloorToInt(Mathf.Sqrt(proposedVertexCount));
            
            return (t + 1) * t; //one extra parallel to wrap uv seams
        }




        /// <summary>
        /// Set of directions for creating a procedural sphere (UV) mesh.
        /// </summary>
        public class MeshDetailsSphereUV : MeshDetails
        {
            public MeshDetailsSphereUV() : base()
            {
                this.vertexCount = 100;
            }

            public MeshDetailsSphereUV(MeshDetails prevMeshDetails) : base(prevMeshDetails) { }
        }
    }
}