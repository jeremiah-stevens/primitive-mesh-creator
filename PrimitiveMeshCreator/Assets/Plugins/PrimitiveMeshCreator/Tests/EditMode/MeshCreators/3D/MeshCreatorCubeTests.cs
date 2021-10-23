using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PrimitiveCreator.Tests;
using static PrimitiveCreator.MeshCreators.MeshCreator;

/*TODO:
 * • CreateMesh(MeshDetails)
 *    > Custom details
 */
namespace PrimitiveCreator.MeshCreators
{
    /// <summary>
    /// Test suite for MeshCreatorCube.
    /// </summary>
    public class MeshCreatorCubeTests : MonoBehaviour
    {
        #region CreateMesh()

        [Test]
        public void CreateMeshReturnsDefaultMesh()
        {
            MeshCreatorCube creator = new MeshCreatorCube();

            Mesh outputMesh = creator.CreateMesh();


            /*   3 --- 0 <- (-0.5, 0.5, -0.5)
             *  /     /|
             * 2 --- 1 |
             * | 7---|-4 <- (-0.5, -0.5, -0.5)
             * |/    |/
             * 6 --- 5 <- (-0.5, -0.5, 0.5)
             */
            float a = 0.5f;
            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(-a, a, -a);
            points[1] = new Vector3(-a, a, a);
            points[2] = new Vector3(a, a, a);
            points[3] = new Vector3(a, a, -a);
            points[4] = new Vector3(-a, -a, -a);
            points[5] = new Vector3(-a, -a, a);
            points[6] = new Vector3(a, -a, a);
            points[7] = new Vector3(a, -a, -a);

            //confirm the vertices (each face is set left-to-right, bottom-to-top)
            Vector3[] vertices = new Vector3[24] //6 faces, 4 vertices per face (6 * 4)
            {
            points[0], points[3], points[1], points[2], //top-side
            points[4], points[7], points[0], points[3], //back-side
            points[7], points[6], points[3], points[2], //left-side
            points[6], points[5], points[2], points[1], //front-side
            points[5], points[4], points[1], points[0], //right-side
            points[7], points[4], points[6], points[5], //bottom-side
            };

            TestUtilities.AssertAreApproximatelyEqual(vertices, outputMesh.vertices);

            /* (0,1) - (1,1)
             *   |       |
             *   |       |
             * (0,0) - (1,0)
             */
            Vector2[] pointUvs = new Vector2[4];
            pointUvs[0] = new Vector2(0f, 0f);
            pointUvs[1] = new Vector2(0f, 1f);
            pointUvs[2] = new Vector2(1f, 1f);
            pointUvs[3] = new Vector2(1f, 0f);

            //confirm the uv coordinates (each face is set left-to-right, bottom-to-top)
            Vector2[] uvs = new Vector2[24] //24 vertices
            {
            pointUvs[0], pointUvs[3], pointUvs[1], pointUvs[2],
            pointUvs[0], pointUvs[3], pointUvs[1], pointUvs[2],
            pointUvs[0], pointUvs[3], pointUvs[1], pointUvs[2],
            pointUvs[0], pointUvs[3], pointUvs[1], pointUvs[2],
            pointUvs[0], pointUvs[3], pointUvs[1], pointUvs[2],
            pointUvs[0], pointUvs[3], pointUvs[1], pointUvs[2]
            };
            TestUtilities.AssertAreApproximatelyEqual(uvs, outputMesh.uv);

            /* 2 --- 3      6 --- 7
             * |   / |      |   / |
             * | /   |      | /   |
             * 0 --- 1      4 --- 5     ...
             */
            //confirm the tris (each face is set left-to-right, bottom-to-top)
            int[] tris = new int[36] //6 faces, 1 quad per face, 2 tris per quad, 3 verts per tri (6 * 1 * 2 * 3)
            {
            0, 2, 3, 3, 1, 0,
            4, 6, 7, 7, 5, 4,
            8, 10, 11, 11, 9, 8,
            12, 14, 15, 15, 13, 12,
            16, 18, 19, 19, 17, 16,
            20, 22, 23, 23, 21, 20
            };
            Assert.AreEqual(tris, outputMesh.triangles);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region CreateMesh(MeshDetails)

        [Test]
        public void CreateMeshSetsVerticesProperly()
        {
            MeshCreatorCube creator = new MeshCreatorCube();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 54; //9 per face

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /*
             *26   2 - 1 - 0 <- (-0.5, -0.5, -0.5)
             * |  /   /   /
             *17 5 - 4 - 3 <- (-0.5, -0.5, 0)
             * |/   /   /
             * 8 - 7 - 6 <- (-0.5, -0.5, 0.5)
             */

            /*   3 --- 0 <- (-0.5, 0.5, -0.5)
             *  /     /|
             * 2 --- 1 |
             * | 7---|-4 <- (-0.5, -0.5, -0.5)
             * |/    |/
             * 6 --- 5 <- (-0.5, -0.5, 0.5)
             */
            float a = 0.5f;
            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(-a, a, -a);
            points[1] = new Vector3(-a, a, a);
            points[2] = new Vector3(a, a, a);
            points[3] = new Vector3(a, a, -a);
            points[4] = new Vector3(-a, -a, -a);
            points[5] = new Vector3(-a, -a, a);
            points[6] = new Vector3(a, -a, a);
            points[7] = new Vector3(a, -a, -a);

            Vector3[] framingVertices = new Vector3[24] //6 faces, 4 vertices per face (6 * 4)
            {
            points[0], points[3], points[1], points[2], //top-side
            points[4], points[7], points[0], points[3], //back-side
            points[7], points[6], points[3], points[2], //left-side
            points[6], points[5], points[2], points[1], //front-side
            points[5], points[4], points[1], points[0], //right-side
            points[7], points[4], points[6], points[5], //bottom-side
            };
            int faceCount = 6;
            int sideLength = 3; //number of vertices along a side

            int outVert = 0; //output mesh vertex
            for (int f = 0; f < faceCount; f++)
            {
                Vector3 v0 = framingVertices[4 * f];
                Vector3 v1 = framingVertices[4 * f + 1];
                Vector3 v2 = framingVertices[4 * f + 2];
                Vector3 v3 = framingVertices[4 * f + 3];

                for (int y = 0; y < sideLength; y++)
                {
                    float v = (float)y / (float)(sideLength - 1);
                    for (int x = 0; x < sideLength; x++)
                    {
                        float u = (float)x / (float)(sideLength - 1);
                        Vector3 expectedVert = MathExtensions.LerpBilinear(u, v, v0, v1, v3, v2);
                        Assert.Less(Vector3.Distance(expectedVert, outputMesh.vertices[outVert++]), Mathf.Epsilon);
                    }
                }
            }
        }

        [Test]
        public void CreateMeshSetsUVsProperly()
        {
            MeshCreatorCube creator = new MeshCreatorCube();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 54; //9 per face

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /*   3 --- 0 <- (-0.5, 0.5, -0.5)
             *  /     /|
             * 2 --- 1 |
             * | 7---|-4 <- (-0.5, -0.5, -0.5)
             * |/    |/
             * 6 --- 5 <- (-0.5, -0.5, 0.5)
             */

            int faceCount = 6;
            int sideLength = 3; //number of vertices along a side

            int outVert = 0; //output mesh vertex
            for (int f = 0; f < faceCount; f++)
            {
                Vector3 v0 = new Vector2(0f, 0f);
                Vector3 v1 = new Vector2(1f, 0f);
                Vector3 v2 = new Vector2(0f, 1f);
                Vector3 v3 = new Vector3(1f, 1f);

                for (int y = 0; y < sideLength; y++)
                {
                    float v = (float)y / (float)(sideLength - 1);
                    for (int x = 0; x < sideLength; x++)
                    {
                        float u = (float)x / (float)(sideLength - 1);
                        Vector3 expectedVert = MathExtensions.LerpBilinear(u, v, v0, v1, v3, v2);
                        Assert.Less(Vector3.Distance(expectedVert, outputMesh.uv[outVert++]), Mathf.Epsilon);
                    }
                }
            }
        }

        [Test]
        public void CreateMeshSetsTrisProperly()
        {
            MeshCreatorCube creator = new MeshCreatorCube();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 54; //9 per face

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /*
             *26   2 - 1 - 0 <- (-0.5, -0.5, -0.5)
             * |  /   /   /
             *17 5 - 4 - 3 <- (-0.5, -0.5, 0)
             * |/   /   /
             * 8 - 7 - 6 <- (-0.5, -0.5, 0.5)
             */
            int faceCount = 6;
            int quadsPerSide = 2; //number of quads per side of a face
            int vertsPerSide = 3; //number of verts per side of a face

            int outTri = 0; //index in outputMesh.triangle we're currently checking
            int t = 0; //index of the expected
            for (int f = 0; f < faceCount; f++)
            {
                for (int y = 0; y < quadsPerSide; y++)
                {
                    for (int x = 0; x < quadsPerSide; x++)
                    {
                        Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerSide, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerSide + 1, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerSide + 1, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + 1, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t, outputMesh.triangles[outTri++]);

                        t += (x == quadsPerSide - 1) ? 2 : 1; //if we're on the last quad of the face, skip to the first of the next row
                    }

                    if (y == quadsPerSide - 1) //on the last row of the face, move to the next face
                    {
                        t += vertsPerSide;
                    }
                }
            }
        }

        [Test]
        public void CreateMeshSetsNormalsProperly()
        {
            MeshCreatorCube creator = new MeshCreatorCube();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 54; //9 per face

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshSetsTangentsProperly()
        {
            MeshCreatorCube creator = new MeshCreatorCube();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 54; //9 per face

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshAppliesTransformOperationsProperly()
        {
            MeshCreatorCube creator = new MeshCreatorCube();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 54; //9 per face
            meshDetails.translation = new Vector3(-5, 100f, 23f);
            meshDetails.rotation = Quaternion.Euler(30f, -230f, 76.23f);
            meshDetails.scale = new Vector3(0.23f, 4f, 1.423f);

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /*   3 --- 0 <- (-0.5, 0.5, -0.5)
             *  /     /|
             * 2 --- 1 |
             * | 7---|-4 <- (-0.5, -0.5, -0.5)
             * |/    |/
             * 6 --- 5 <- (-0.5, -0.5, 0.5)
             */
            float a = 0.5f;
            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(-a, a, -a);
            points[1] = new Vector3(-a, a, a);
            points[2] = new Vector3(a, a, a);
            points[3] = new Vector3(a, a, -a);
            points[4] = new Vector3(-a, -a, -a);
            points[5] = new Vector3(-a, -a, a);
            points[6] = new Vector3(a, -a, a);
            points[7] = new Vector3(a, -a, -a);

            //transform the framing vertices
            for (int p = 0; p < points.Length; p++)
            {
                points[p] = Vector3.Scale(points[p], meshDetails.scale);
                points[p] = meshDetails.rotation * points[p];
                points[p] += meshDetails.translation;
            }

            Vector3[] framingVertices = new Vector3[24] //6 faces, 4 vertices per face (6 * 4)
            {
            points[0], points[3], points[1], points[2], //top-side
            points[4], points[7], points[0], points[3], //back-side
            points[7], points[6], points[3], points[2], //left-side
            points[6], points[5], points[2], points[1], //front-side
            points[5], points[4], points[1], points[0], //right-side
            points[7], points[4], points[6], points[5], //bottom-side
            };

            int faceCount = 6;
            int sideLength = 3; //number of vertices along a side

            int outVert = 0; //output mesh vertex
            for (int f = 0; f < faceCount; f++)
            {
                Vector3 v0 = framingVertices[4 * f];
                Vector3 v1 = framingVertices[4 * f + 1];
                Vector3 v2 = framingVertices[4 * f + 2];
                Vector3 v3 = framingVertices[4 * f + 3];

                for (int y = 0; y < sideLength; y++)
                {
                    float v = (float)y / (float)(sideLength - 1);
                    for (int x = 0; x < sideLength; x++)
                    {
                        float u = (float)x / (float)(sideLength - 1);
                        Vector3 expectedVert = MathExtensions.LerpBilinear(u, v, v0, v1, v3, v2);
                        Assert.Less(Vector3.Distance(expectedVert, outputMesh.vertices[outVert++]), 0.01f);
                    }
                }
            }

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region GetClosestViableVertexCount(int)

        [Test]
        public void GetClosestViableVertexCountReturns24WhenRequestedIsLessThan0()
        {
            MeshCreatorCube creator = new MeshCreatorCube();

            int expected = 24;
            int actual = creator.GetClosestViableVertexCount(-4);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns24WhenRequestIs0()
        {
            MeshCreatorCube creator = new MeshCreatorCube();

            int expected = 24;
            int actual = creator.GetClosestViableVertexCount(0);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns54WhenRequestIs60()
        {
            MeshCreatorCube creator = new MeshCreatorCube();

            int expected = 24;
            int actual = creator.GetClosestViableVertexCount(0);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns1734WhenRequestIs1800()
        {
            MeshCreatorCube creator = new MeshCreatorCube();

            int expected = 1734;
            int actual = creator.GetClosestViableVertexCount(1800);

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}