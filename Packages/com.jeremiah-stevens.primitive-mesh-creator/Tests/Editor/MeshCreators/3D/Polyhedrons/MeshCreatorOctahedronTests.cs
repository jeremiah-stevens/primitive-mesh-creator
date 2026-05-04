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
    public class MeshCreatorOctahedronTests : MonoBehaviour
    {
        /*     0     <- (0, 0.5, 0)
         *    /| \
         *   4 --- 1 <- (-0.5, 0, 0)
         *  /     /
         * 3 --- 2 <- (0, 0, 0.5)
         *  \ | /
         *    5     <- (0, -0.5, 0)
         */

        #region CreateMesh()

        [Test]
        public void CreateMeshReturnsDefaultMesh()
        {
            MeshCreatorOctahedron creator = new MeshCreatorOctahedron();
            Mesh outputMesh = creator.CreateMesh();

            float r = 0.5f;
            Vector3[] points = new Vector3[6];
            points[0] = new Vector3(0, r, 0);
            points[1] = new Vector3(-r, 0, 0);
            points[2] = new Vector3(0, 0, -r);
            points[3] = new Vector3(r, 0, 0);
            points[4] = new Vector3(0, 0, r);
            points[5] = new Vector3(0, -r, 0);

            //confirm the vertices
            Vector3[] vertices = new Vector3[24]
            {
            points[1], points[2], points[0],
            points[2], points[3], points[0],
            points[3], points[4], points[0],
            points[4], points[1], points[0],
            points[2], points[1], points[5],
            points[3], points[2], points[5],
            points[4], points[3], points[5],
            points[1], points[4], points[5],
            };
            TestUtilities.AssertAreApproximatelyEqual(vertices, outputMesh.vertices);

            //confirms the uv coordinates
            for (int v = 0; v < vertices.Length; v += 3)
            {
                Vector2 v00 = new Vector2(0f, 0f);
                Vector2 v01 = new Vector2(0.5f, 1f);
                Vector2 v10 = new Vector2(1f, 0f);

                Assert.AreEqual(v00, outputMesh.uv[v]);
                Assert.AreEqual(v10, outputMesh.uv[v + 1]);
                Assert.AreEqual(v01, outputMesh.uv[v + 2]);
            }

            //confirms the tri coordinates
            int[] tris = new int[24]
            {
            0, 2, 1,
            3, 5, 4,
            6, 8, 7,
            9, 11,10,
            12,14,13,
            15,17,16,
            18,20,19,
            21,23,22
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
            MeshCreatorOctahedron creator = new MeshCreatorOctahedron();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 120;
            Mesh outputMesh = creator.CreateMesh(meshDetails);

            float r = 0.5f;
            Vector3[] points = new Vector3[6];
            points[0] = new Vector3(0, r, 0);
            points[1] = new Vector3(-r, 0, 0);
            points[2] = new Vector3(0, 0, -r);
            points[3] = new Vector3(r, 0, 0);
            points[4] = new Vector3(0, 0, r);
            points[5] = new Vector3(0, -r, 0);

            //sets the framing vertices of the faces
            Vector3[] framingVertices = new Vector3[24]
            {
            points[1], points[2], points[0],
            points[2], points[3], points[0],
            points[3], points[4], points[0],
            points[4], points[1], points[0],
            points[2], points[1], points[5],
            points[3], points[2], points[5],
            points[4], points[3], points[5],
            points[1], points[4], points[5],
            };
            int faceCount = 8;
            int sideLength = 5;

            int outVert = 0; //output mesh vertex
            for (int f = 0; f < faceCount; f++)
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
        }

        [Test]
        public void CreateMeshSetsUVsProperly()
        {
            MeshCreatorOctahedron creator = new MeshCreatorOctahedron();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 120;
            Mesh outputMesh = creator.CreateMesh(meshDetails);

            float r = 0.5f;
            Vector3[] points = new Vector3[6];
            points[0] = new Vector3(0, r, 0);
            points[1] = new Vector3(-r, 0, 0);
            points[2] = new Vector3(0, 0, -r);
            points[3] = new Vector3(r, 0, 0);
            points[4] = new Vector3(0, 0, r);
            points[5] = new Vector3(0, -r, 0);

            //confirm the vertices
            Vector3[] framingVertices = new Vector3[24]
            {
            points[1], points[2], points[0],
            points[2], points[3], points[0],
            points[3], points[4], points[0],
            points[4], points[1], points[0],
            points[2], points[1], points[5],
            points[3], points[2], points[5],
            points[4], points[3], points[5],
            points[1], points[4], points[5],
            };
            int faceCount = 8;
            int sideLength = 5;

            int outVert = 0; //output mesh vertex
            for (int f = 0; f < faceCount; f++)
            {
                Vector2 v0 = new Vector2(0f, 0f);
                Vector2 v1 = new Vector2(1f, 0f);
                Vector2 v2 = new Vector2(0.5f, 1f);

                for (int y = 0; y < sideLength; y++)
                {
                    float v = (float)y / (float)(sideLength - 1);
                    for (int x = 0; x < sideLength - y; x++)
                    {
                        float u = (float)x / (float)(sideLength - 1);
                        Vector3 expectedVert = MathExtensions.LerpBarycentric(u, v, v0, v1, v2);
                        Assert.Less(Vector3.Distance(expectedVert, outputMesh.uv[outVert++]), 0.01f);
                    }
                }
            }
        }

        [Test]
        public void CreateMeshSetsTrisProperly()
        {
            MeshCreatorOctahedron creator = new MeshCreatorOctahedron();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 120;
            Mesh outputMesh = creator.CreateMesh(meshDetails);

            int faceCount = 8;
            int sideLength = 4;
            int vertsPerSide = 5;

            int outTri = 0; //index in outputMesh.triangle we're currently checking
            int t = 0; //index of the expected
            for (int f = 0; f < faceCount; f++)
            {
                /*       9
                 *      / \
                 *     7 - 8
                 *    / \ / \
                 *   4 - 5 - 6
                 *  / \ / \ / \
                 * 0 - 1 - 2 - 3
                 */
                for (int y = 0; y < sideLength; y++)
                {
                    int vertsInCurrentRow = vertsPerSide - y;
                    for (int x = 0; x < sideLength - y; x++)
                    {
                        if (x < sideLength - y - 1) //quad
                        {
                            Assert.AreEqual(t + 1, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t + vertsInCurrentRow, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t + vertsInCurrentRow, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t + vertsInCurrentRow + 1, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t + 1, outputMesh.triangles[outTri++]);
                            t += 1;
                        }
                        else //triangle (i.e. last tri on the current row)
                        {
                            Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t + vertsInCurrentRow, outputMesh.triangles[outTri++]);
                            Assert.AreEqual(t + 1, outputMesh.triangles[outTri++]);
                            t += 2;
                        }
                    }

                    if (y == sideLength - 1) //on the last row of the face, move to the next face
                    {
                        t += 1;
                    }
                }
            }
        }

        [Test]
        public void CreateMeshSetsNormalsProperly()
        {
            MeshCreatorOctahedron creator = new MeshCreatorOctahedron();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 120;
            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshSetsTangentsProperly()
        {
            MeshCreatorOctahedron creator = new MeshCreatorOctahedron();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 120;
            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshAppliesTransformOperationsProperly()
        {
            MeshCreatorOctahedron creator = new MeshCreatorOctahedron();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 120;
            meshDetails.translation = new Vector3(-5, 100f, 23f);
            meshDetails.rotation = Quaternion.Euler(30f, -230f, 76.23f);
            meshDetails.scale = new Vector3(0.23f, 4f, 1.423f);

            Mesh outputMesh = creator.CreateMesh(meshDetails);


            float r = 0.5f;
            Vector3[] points = new Vector3[6];
            points[0] = new Vector3(0, r, 0);
            points[1] = new Vector3(-r, 0, 0);
            points[2] = new Vector3(0, 0, -r);
            points[3] = new Vector3(r, 0, 0);
            points[4] = new Vector3(0, 0, r);
            points[5] = new Vector3(0, -r, 0);

            //transform the framing vertices
            for (int p = 0; p < points.Length; p++)
            {
                points[p] = Vector3.Scale(points[p], meshDetails.scale);
                points[p] = meshDetails.rotation * points[p];
                points[p] += meshDetails.translation;
            }


            //sets the framing vertices of the faces
            Vector3[] framingVertices = new Vector3[24]
            {
            points[1], points[2], points[0],
            points[2], points[3], points[0],
            points[3], points[4], points[0],
            points[4], points[1], points[0],
            points[2], points[1], points[5],
            points[3], points[2], points[5],
            points[4], points[3], points[5],
            points[1], points[4], points[5],
            };
            int faceCount = 8;
            int sideLength = 5;

            int outVert = 0; //output mesh vertex
            for (int f = 0; f < faceCount; f++)
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

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region GetClosestViableVertexCount(int)

        [Test]
        public void GetClosestViableVertexCountReturns24WhenRequestedIsLessThan0()
        {
            MeshCreatorOctahedron creator = new MeshCreatorOctahedron();

            int expected = 24;
            int actual = creator.GetClosestViableVertexCount(-4);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns24WhenRequestIs0()
        {
            MeshCreatorOctahedron creator = new MeshCreatorOctahedron();

            int expected = 24;
            int actual = creator.GetClosestViableVertexCount(0);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns48WhenRequestIs50()
        {
            MeshCreatorOctahedron creator = new MeshCreatorOctahedron();

            int expected = 48;
            int actual = creator.GetClosestViableVertexCount(50);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns9800WhenRequestIs9887()
        {
            MeshCreatorOctahedron creator = new MeshCreatorOctahedron();

            int expected = 9800;
            int actual = creator.GetClosestViableVertexCount(9887);

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}