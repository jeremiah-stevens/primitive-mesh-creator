using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PrimitiveCreator.Tests;

/*TODO:
 * • CreateMesh(MeshDetails)
 *    > Custom details
 */
namespace PrimitiveCreator.MeshCreators
{
    public class MeshCreatorPyramidTests : MonoBehaviour
    {
        #region CreateMesh()

        [Test]
        public void CreateMeshReturnsDefaultMesh()
        {
            MeshCreatorPyramid creator = new MeshCreatorPyramid();
            Mesh outputMesh = creator.CreateMesh();

            /*     0 <- (0, 1, 0)
             *    /| \
             *   4 --- 1 <- (-0.5, -0.5, -0.5)
             *  /     /
             * 3 --- 2 <- (-0.5, -0.5, 0.5)
             */
            Vector3[] points = new Vector3[5];
            points[0] = new Vector3(0f, 0.5f, 0f);
            points[1] = new Vector3(-0.5f, -0.5f, -0.5f);
            points[2] = new Vector3(-0.5f, -0.5f, 0.5f);
            points[3] = new Vector3(0.5f, -0.5f, 0.5f);
            points[4] = new Vector3(0.5f, -0.5f, -0.5f);

            //confirm the vertices (each face is set left-to-right, bottom-to-top)
            Vector3[] vertices = new Vector3[16] //4 triangle faces, 1 quad face (4 * 3 + 1 * 4)
            {
            points[2], points[1], points[0],
            points[3], points[2], points[0],
            points[4], points[3], points[0],
            points[1], points[4], points[0],
            points[4], points[1], points[3], points[2]
            };
            TestUtilities.AssertAreApproximatelyEqual(vertices, outputMesh.vertices);

            //confirms the uv coordinates
            Vector2[] pointUvs = new Vector2[5];
            pointUvs[0] = new Vector2(0f, 0f);
            pointUvs[1] = new Vector2(0f, 1f);
            pointUvs[2] = new Vector2(1f, 1f);
            pointUvs[3] = new Vector2(1f, 0f);
            pointUvs[4] = new Vector2(0.5f, 1f);

            Vector2[] uvs = new Vector2[16]  //4 triangle faces, 1 quad face (4 * 3 + 1 * 4)
            {
            pointUvs[0], pointUvs[3], pointUvs[4],
            pointUvs[0], pointUvs[3], pointUvs[4],
            pointUvs[0], pointUvs[3], pointUvs[4],
            pointUvs[0], pointUvs[3], pointUvs[4],
            pointUvs[0], pointUvs[3], pointUvs[1], pointUvs[2]
            };
            TestUtilities.AssertAreApproximatelyEqual(uvs, outputMesh.uv);

            //confirms the tri coordinates
            int[] tris = new int[18] //4 triangle faces, 1 quad faces (4 * 1 * 3 + 1 * 2 * 3)
            {
            0, 2, 1,
            3, 5, 4,
            6, 8, 7,
            9, 11, 10,
            12, 14, 15, 15, 13, 12
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
            MeshCreatorPyramid creator = new MeshCreatorPyramid();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 33;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /*     0 <- (0, 1, 0)
             *    /| \
             *   4 --- 1 <- (-0.5, -0.5, -0.5)
             *  /     /
             * 3 --- 2 <- (-0.5, -0.5, 0.5)
             */
            float a = 0.5f;
            Vector3[] points = new Vector3[5];
            points[0] = new Vector3(0f, a, 0f);
            points[1] = new Vector3(-a, -a, -a);
            points[2] = new Vector3(-a, -a, a);
            points[3] = new Vector3(a, -a, a);
            points[4] = new Vector3(a, -a, -a);

            Vector3[] framingVertices = new Vector3[16] //5 faces, 4 triangles + 1 quad
            {
            points[2], points[1], points[0],
            points[3], points[2], points[0],
            points[4], points[3], points[0],
            points[1], points[4], points[0],
            points[4], points[1], points[3], points[2]
            };
            int faceCount = 5;
            int sideLength = 3;

            int outVert = 0; //output mesh vertex
            for (int f = 0; f < faceCount; f++)
            {
                if (f < faceCount - 1) //triangle faces
                {
                    Vector3 v0 = framingVertices[3 * f];
                    Vector3 v1 = framingVertices[3 * f + 1];
                    Vector3 v2 = framingVertices[3 * f + 2];

                    for (int y = 0; y < sideLength; y++)
                    {
                        float v = (float)y / (float)(sideLength - 1);
                        for (int x = 0; x < sideLength - y; x++)
                        {
                            float u = (float)x / (float)(sideLength - 1);
                            Vector3 expectedVert = MathExtensions.LerpBarycentric(u, v, v0, v1, v2);
                            Assert.Less(Vector3.Distance(expectedVert, outputMesh.vertices[outVert++]), Mathf.Epsilon);
                        }
                    }
                }
                else //quad face
                {
                    Vector3 v0 = framingVertices[3 * f];
                    Vector3 v1 = framingVertices[3 * f + 1];
                    Vector3 v2 = framingVertices[3 * f + 2];
                    Vector3 v3 = framingVertices[3 * f + 3];

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
        }

        [Test]
        public void CreateMeshSetsUVsProperly()
        {
            MeshCreatorPyramid creator = new MeshCreatorPyramid();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 33;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /*     0 <- (0, 1, 0)
             *    /| \
             *   4 --- 1 <- (-0.5, -0.5, -0.5)
             *  /     /
             * 3 --- 2 <- (-0.5, -0.5, 0.5)
             */
            int faceCount = 5;
            int sideLength = 3;

            int outVert = 0; //output mesh vertex
            for (int f = 0; f < faceCount; f++)
            {
                if (f < faceCount - 1) //triangle faces
                {
                    Vector3 v0 = new Vector2(0f, 0f);
                    Vector3 v1 = new Vector2(1f, 0f);
                    Vector3 v2 = new Vector2(0.5f, 1f);

                    for (int y = 0; y < sideLength; y++)
                    {
                        float v = (float)y / (float)(sideLength - 1);
                        for (int x = 0; x < sideLength - y; x++)
                        {
                            float u = (float)x / (float)(sideLength - 1);
                            Vector3 expectedVert = MathExtensions.LerpBarycentric(u, v, v0, v1, v2);
                            Assert.Less(Vector3.Distance(expectedVert, outputMesh.uv[outVert++]), Mathf.Epsilon);
                        }
                    }
                }
                else //quad face
                {
                    Vector3 v0 = new Vector2(0f, 0f);
                    Vector3 v1 = new Vector2(1f, 0f);
                    Vector3 v2 = new Vector2(0f, 1f);
                    Vector3 v3 = new Vector2(1f, 1f);

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
        }

        [Test]
        public void CreateMeshSetsTrisProperly()
        {
            MeshCreatorPyramid creator = new MeshCreatorPyramid();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 33;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /*     0 <- (0, 1, 0)
             *    /| \
             *   4 --- 1 <- (-0.5, -0.5, -0.5)
             *  /     /
             * 3 --- 2 <- (-0.5, -0.5, 0.5)
             */
            int faceCount = 5;
            int sideLength = 2;
            int vertsPerSide = 3;

            int outTri = 0; //index in outputMesh.triangle we're currently checking
            int t = 0; //index of the expected
            for (int f = 0; f < faceCount; f++)
            {
                if (f < faceCount - 1) //triangle face
                {
                    /*     5
                     *    / \
                     *   3 - 4
                     *  / \ / \
                     * 0 - 1 - 2
                     */
                    for (int y = 0; y < sideLength; y++)
                    {
                        for (int x = 0; x < sideLength - y; x++)
                        {
                            if (x < sideLength - y - 1) //quad
                            {
                                Assert.AreEqual(t + 1, outputMesh.triangles[outTri++]);
                                Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                                Assert.AreEqual(t + vertsPerSide, outputMesh.triangles[outTri++]);
                                Assert.AreEqual(t + vertsPerSide, outputMesh.triangles[outTri++]);
                                Assert.AreEqual(t + vertsPerSide + 1, outputMesh.triangles[outTri++]);
                                Assert.AreEqual(t + 1, outputMesh.triangles[outTri++]);
                                t += 1;
                            }
                            else //triangle (i.e. last tri on the current row)
                            {
                                Assert.AreEqual(t + y, outputMesh.triangles[outTri++]);
                                Assert.AreEqual(t + vertsPerSide, outputMesh.triangles[outTri++]);
                                Assert.AreEqual(t + y + 1, outputMesh.triangles[outTri++]);
                                t += 1;
                            }
                        }

                        if (y == sideLength - 1) //on the last row of the face, move to the next face
                        {
                            t += vertsPerSide;
                        }
                    }
                }
                else //quad face
                {
                    /* 6 -- 7 -- 8
                     * |  / |  / |
                     * 3 -- 4 -- 5
                     * |  / |  / |
                     * 0 -- 1 -- 2
                     */
                    for (int y = 0; y < sideLength; y++)
                    {
                        for (int x = 0; x < sideLength; x++)
                        {
                            Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t + vertsPerSide, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t + vertsPerSide + 1, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t + vertsPerSide + 1, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t + 1, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t, outputMesh.triangles[outTri++]);

                            t += (x == sideLength - 1) ? 2 : 1; //if we're on the last quad of the face, skip to the first of the next row
                        }

                        if (y == sideLength - 1) //on the last row of the face, move to the next face
                        {
                            t += vertsPerSide;
                        }
                    }
                }
            }
        }

        [Test]
        public void CreateMeshSetsNormalsProperly()
        {
            MeshCreatorPyramid creator = new MeshCreatorPyramid();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 33;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshSetsTangentsProperly()
        {
            MeshCreatorPyramid creator = new MeshCreatorPyramid();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 33;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshAppliesTransformOperationsProperly()
        {
            MeshCreatorPyramid creator = new MeshCreatorPyramid();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 33; //3 verts per side
            meshDetails.translation = new Vector3(-5, 100f, 23f);
            meshDetails.rotation = Quaternion.Euler(30f, -230f, 76.23f);
            meshDetails.scale = new Vector3(0.23f, 4f, 1.423f);

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /*     0 <- (0, 1, 0)
             *    /| \
             *   4 --- 1 <- (-0.5, -0.5, -0.5)
             *  /     /
             * 3 --- 2 <- (-0.5, -0.5, 0.5)
             */
            float a = 0.5f;
            Vector3[] points = new Vector3[5];
            points[0] = new Vector3(0f, a, 0f);
            points[1] = new Vector3(-a, -a, -a);
            points[2] = new Vector3(-a, -a, a);
            points[3] = new Vector3(a, -a, a);
            points[4] = new Vector3(a, -a, -a);

            //transform the framing vertices
            for (int p = 0; p < points.Length; p++)
            {
                points[p] = Vector3.Scale(points[p], meshDetails.scale);
                points[p] = meshDetails.rotation * points[p];
                points[p] += meshDetails.translation;
            }

            Vector3[] framingVertices = new Vector3[16] //5 faces, 4 triangles + 1 quad
            {
            points[2], points[1], points[0],
            points[3], points[2], points[0],
            points[4], points[3], points[0],
            points[1], points[4], points[0],
            points[4], points[1], points[3], points[2]
            };
            int faceCount = 5;
            int sideLength = 3;

            int outVert = 0; //output mesh vertex
            for (int f = 0; f < faceCount; f++)
            {
                if (f < faceCount - 1) //triangle faces
                {
                    Vector3 v0 = framingVertices[3 * f];
                    Vector3 v1 = framingVertices[3 * f + 1];
                    Vector3 v2 = framingVertices[3 * f + 2];

                    for (int y = 0; y < sideLength; y++)
                    {
                        float v = (float)y / (float)(sideLength - 1);
                        for (int x = 0; x < sideLength - y; x++)
                        {
                            float u = (float)x / (float)(sideLength - 1);
                            Vector3 expectedVert = MathExtensions.LerpBarycentric(u, v, v0, v1, v2);
                            Assert.Less(Vector3.Distance(expectedVert, outputMesh.vertices[outVert++]), 0.01f);
                        }
                    }
                }
                else //quad face
                {
                    Vector3 v0 = framingVertices[3 * f];
                    Vector3 v1 = framingVertices[3 * f + 1];
                    Vector3 v2 = framingVertices[3 * f + 2];
                    Vector3 v3 = framingVertices[3 * f + 3];

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
            }

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region GetClosestViableVertexCount(int)

        [Test]
        public void GetClosestViableVertexCountReturns16WhenRequestedIsLessThan0()
        {
            MeshCreatorPyramid creator = new MeshCreatorPyramid();

            int expected = 16;
            int actual = creator.GetClosestViableVertexCount(-4);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns16WhenRequestIs0()
        {
            MeshCreatorPyramid creator = new MeshCreatorPyramid();

            int expected = 16;
            int actual = creator.GetClosestViableVertexCount(0);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns33WhenRequestIs40()
        {
            MeshCreatorPyramid creator = new MeshCreatorPyramid();

            int expected = 33;
            int actual = creator.GetClosestViableVertexCount(40);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns85WhenRequestIs104()
        {
            MeshCreatorPyramid creator = new MeshCreatorPyramid();

            int expected = 85;
            int actual = creator.GetClosestViableVertexCount(104);

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}