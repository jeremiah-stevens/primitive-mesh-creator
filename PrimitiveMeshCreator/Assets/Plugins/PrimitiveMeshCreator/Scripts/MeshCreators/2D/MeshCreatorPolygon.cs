using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PrimitiveCreator.PrimitiveCreatorUtility;


/* polyCount   n  -> number of sides for the polygon (ex triangle is a trigon); limit to 3-gon or higher
 * innerRadius r  -> radius from center to one of the sides
 * circumRadius R -> center of polygon to one of the vertices
 * sideLength s   -> length of a polygon side
 * ...
 * sideAngle a    -> 360 / n degrees
 * https://en.wikipedia.org/wiki/Regular_polygon
 */
namespace PrimitiveCreator.MeshCreators
{
    public class MeshCreatorPolygon : MeshCreator
    {
        public override MeshType MeshType => MeshType.Polygon;
        public override string DisplayName => "Polygon";
        public override Type _meshDetails => typeof(MeshDetailsPolygon);




        /// <summary>
        /// Create a version of a polygon mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Polygon mesh using the given parameters.</returns>
        public override Mesh CreateMesh(MeshDetails meshDetails)
        {
            return CreateMesh(new MeshDetailsPolygon(meshDetails));
        }

        /// <summary>
        /// Create a version of a polygon mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Polygon mesh using the given parameters.</returns>
        public Mesh CreateMesh(MeshDetailsPolygon meshDetails)
        {
            Mesh framingMesh = new Mesh();

            int sideCount = Mathf.Max(meshDetails.n, 3);
            int vertexCount = GetClosestViableVertexCount(meshDetails.vertexCount, sideCount);
            int vCountPerSide = Mathf.FloorToInt(vertexCount / sideCount);

            CreateFramingMesh(ref framingMesh, sideCount);

            CombineInstance[] combine = new CombineInstance[sideCount];
            int t = 0;
            for (int i = 0; i < sideCount; i++)
            {
                combine[i].mesh = CreateTriangle(ref framingMesh, vCountPerSide, framingMesh.triangles[t], framingMesh.triangles[t + 1], framingMesh.triangles[t + 2]);
                combine[i].transform = Matrix4x4.identity;
                t += 3;
            }

            Mesh outputMesh = new Mesh();

            outputMesh.CombineMeshes(combine, true);

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
            return CreateColliderMesh(new MeshDetailsPolygon(meshDetails));
        }

        /// <summary>
        /// Creates a version of the collider-optimized mesh modified by the given set of MeshDetails.
        /// </summary>
        /// <param name="meshDetails">Set of details on the type of mesh to make.</param>
        /// <returns>Collider-optimized mesh of the given type, using the given parameters.</returns>
        public Mesh CreateColliderMesh(MeshDetailsPolygon meshDetails)
        {
            int vertexCount = GetClosestViableVertexCount(0);

            /*Due to 3D physics requirements, mesh must be generated as a 3D mesh. As such, this will generate a prism with the smallest viable width of the main faces.
             *         7
             *        / \
             *       0   12
             *      /|\ /|
             *     5 | 1 |
             *     |\|/| |
             *     | 6 | 11
             *     |/|\|/
             *     4 | 2
             *      \|/
             *       3
             */

            float depthOffset = 0.001f; //width of the prism sides to ensure 3D physics requirements are met

            //set up the front side
            Mesh frontSide = new Mesh();
            CreateFramingMesh(ref frontSide, meshDetails.n);
            TransformMesh(ref frontSide, Vector3.forward * depthOffset, Quaternion.identity, Vector3.one);

            //set up the back side
            Mesh backSide = new Mesh();
            CreateFramingMesh(ref backSide, meshDetails.n);
            TransformMesh(ref backSide, -1 * Vector3.forward * depthOffset, Quaternion.Euler(new Vector3(0f, 180f, 0f)), Vector3.one);

            //combine the meshes to form the two faces
            Mesh outputMesh = new Mesh();
            CombineInstance[] combine = new CombineInstance[2];
            combine[0].mesh = frontSide;
            combine[1].mesh = backSide;
            combine[0].transform = Matrix4x4.identity;
            combine[1].transform = Matrix4x4.identity;
            outputMesh.CombineMeshes(combine, true);

            int[] triangles = new int[frontSide.triangles.Length
                                        + backSide.triangles.Length
                                        + meshDetails.n * 6 //n quads
                                     ];
            int t = 0;
            for (t = 0; t < outputMesh.triangles.Length; t++)
                triangles[t] = outputMesh.triangles[t];

            for(int i = 0; i < meshDetails.n; i++)
            {
                int v00 = (i + 1) % meshDetails.n;
                int v01 = i;
                int v11 = (v01 == 0) //TODO: come up with a better way of rounding this back around to n if value is 0
                    ? (2 * meshDetails.n + 1) - meshDetails.n
                    : (2 * meshDetails.n + 1) - v01;
                int v10 = (v00 == 0)
                    ? (2 * meshDetails.n + 1) - meshDetails.n
                    : (2 * meshDetails.n + 1) - v00;

                t = SetQuad(triangles, t, v00, v01, v11, v10);
            }
            outputMesh.triangles = triangles;
            
            TransformMesh(ref outputMesh, meshDetails.translation, meshDetails.rotation, meshDetails.scale);

            return outputMesh;
        }

        /// <summary>
        /// Creates the framing mesh to generate the pentagon onto.
        /// </summary>
        /// <param name="framingMesh">Mesh to write the vertices to.</param>
        /// <param name="sideCount">Number of sides to the polygon.</param>
        private void CreateFramingMesh(ref Mesh framingMesh, int sideCount)
        {
            int vertexCount = sideCount + 1; //n-gon vertices + 1 for center

            Vector3[] vertices = new Vector3[vertexCount];
            Vector2[] uv = new Vector2[vertices.Length];

            //generate vertices
            for (int p = 0; p < sideCount; p++) //for each poly/edge of the polygon
            {
                vertices[p] = GetRotatedVertexSpot(p, sideCount, 0.5f);
            }
            vertices[vertexCount - 1] = Vector3.zero;

            for (int v = 0; v < vertices.Length; v++)
            {
                uv[v] = vertices[v] + new Vector3(0.5f, 0.5f);
            }

            framingMesh.vertices = vertices;
            framingMesh.uv = uv;
            framingMesh.RecalculateTangents();


            //generate triangles
            int[] triangles = new int[sideCount * 3];
            int t = 0;
            for (int p = 0; p < sideCount; p++)
            {
                int r = (p + 1) % sideCount;
                t = SetTri(triangles, t, r, vertices.Length - 1, p);
            }
            framingMesh.triangles = triangles;

            framingMesh.RecalculateNormals();
        }

        /// <summary>
        /// Gets a position rotated around the unit circle.
        /// </summary>
        /// <param name="polyVertex">Number of </param>
        /// <param name="sideCount"></param>
        /// <param name="ringMagnitude"></param>
        /// <returns></returns>
        private Vector3 GetRotatedVertexSpot(int polyVertex, int sideCount, float ringMagnitude)
        {
            Vector3 v0 = Vector3.up * ringMagnitude;
            float sideAngleAmount = 360f / (float)sideCount;
            float angleAmount = sideAngleAmount * polyVertex;
            return Quaternion.AngleAxis(angleAmount, Vector3.forward * -1f) * v0;
        }



        /// <summary>
        /// Gets the closest vertex count to the proposed vertex count that still provides for a viable model (defaults to 5 sides, a pentagon).
        /// </summary>
        /// <param name="proposedVertexCount">Vertex count you would like the mesh to be at.</param>
        /// <returns>Closest vertex count to the proposedVertexCount, while still providing for a viable model.</returns>
        public override int GetClosestViableVertexCount(int proposedVertexCount)
        {
            return GetClosestViableVertexCount(proposedVertexCount, 5);
        }

        /// <summary>
        ///Gets the closest vertex count to the proposed vertex count that still provides for a viable model.
        /// </summary>
        /// <param name="proposedVertexCount">Vertex count you would like the mesh to be at.</param>
        /// <param name="sideCount">Number of sides to the polygon (ex: 3 is a trigon, 5 is a pentagon).</param>
        /// <returns>Closest vertex count to the proposedVertexCount, while still providing for a viable model.</returns>
        public int GetClosestViableVertexCount(int proposedVertexCount, int sideCount)
        {
            proposedVertexCount = Mathf.Clamp(proposedVertexCount, PrimitiveCreatorConfig.MinimumVertexCount, PrimitiveCreatorConfig.MaximumVertexCount);

            //at minimum, 3 for each side (3 * sideCount)
            int minimumVertexCount = 3 * sideCount;
            if (proposedVertexCount < minimumVertexCount) return minimumVertexCount;

            int proposedVCountPerSide = proposedVertexCount / sideCount;
            return PrimitiveCreatorUtility.GetClosestViableVertexCount(PrimitiveCreatorUtility.MeshType.Triangle, proposedVCountPerSide) * sideCount;
        }



        /// <summary>
        /// Set of directions for creating a procedural polygon mesh.
        /// </summary>
        public class MeshDetailsPolygon : MeshDetails
        {
            [Range(3, 50)] public int n = 5; //number of sides for the polygon

            public MeshDetailsPolygon() : base() { }

            public MeshDetailsPolygon(MeshDetails prevMeshDetails) : base(prevMeshDetails)
            {
                if (prevMeshDetails == null)
                    return;
                if(prevMeshDetails.GetType().Equals(typeof(MeshDetailsPolygon)))
                    this.n = ((MeshDetailsPolygon)prevMeshDetails).n;
            }
        }
    }
}