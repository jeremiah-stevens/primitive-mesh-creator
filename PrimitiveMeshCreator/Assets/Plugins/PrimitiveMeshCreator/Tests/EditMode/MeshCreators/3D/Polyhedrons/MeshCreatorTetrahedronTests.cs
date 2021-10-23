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
    public class MeshCreatorTetrahedronTests : MonoBehaviour
    {
        /*     3 <- (0, 0, 1)
         *    /| \
         *   / 0  \
         *  / /  \ \
         * 1 ------ 2 <- (-d, -a, -c)
         * 
         * a = 1/3
         * b = sqrt(8/9)
         * c = sqrt(2/9)
         * d = sqrt(2/3)
         * 
         * v0 = ( 0,  1,  0)
         * v1 = ( 0, -a,  b)
         * v2 = (-d, -a, -c)
         * v3 = ( d, -a, -c)
         */

        #region CreateMesh()

        [Test]
        public void CreateMeshReturnsDefaultMesh()
        {
            MeshCreatorTetrahedron creator = new MeshCreatorTetrahedron();
            Mesh outputMesh = creator.CreateMesh();

            float a = 1f / 3f;
            float b = Mathf.Sqrt(8f / 9f);
            float c = Mathf.Sqrt(2f / 9f);
            float d = Mathf.Sqrt(2f / 3f);

            //sets the framing points for the shape
            Vector3[] points = new Vector3[4];
            points[0] = new Vector3(b, 0f, -a);
            points[1] = new Vector3(-c, d, -a);
            points[2] = new Vector3(-c, -d, -a);
            points[3] = new Vector3(0f, 0f, 1f);

            //rotates all points so that vertex 0 is in the forward direction
            for (int i = 0; i < points.Length; i++)
                points[i] = Quaternion.Euler(new Vector3(20f, 0f, 90f)) * points[i];

            for (int p = 0; p < points.Length; p++)
                points[p] *= 0.5f;

            //confirm the vertices
            Vector3[] vertices = new Vector3[12]
            {
            points[1], points[2], points[0],
            points[2], points[3], points[0],
            points[3], points[1], points[0],
            points[2], points[1], points[3]
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
            int[] tris = new int[12]
            {
            0, 2, 1,
            3, 5, 4,
            6, 8, 7,
            9, 11, 10
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
            MeshCreatorTetrahedron creator = new MeshCreatorTetrahedron();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 40;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            //sets the framing points for the shape
            float a = 1f / 3f;
            float b = Mathf.Sqrt(8f / 9f);
            float c = Mathf.Sqrt(2f / 9f);
            float d = Mathf.Sqrt(2f / 3f);

            //sets the framing points for the shape
            Vector3[] points = new Vector3[4];
            points[0] = new Vector3(b, 0f, -a);
            points[1] = new Vector3(-c, d, -a);
            points[2] = new Vector3(-c, -d, -a);
            points[3] = new Vector3(0f, 0f, 1f);

            //rotates all points so that vertex 0 is in the forward direction
            for (int i = 0; i < points.Length; i++)
                points[i] = Quaternion.Euler(new Vector3(20f, 0f, 90f)) * points[i];

            for (int p = 0; p < points.Length; p++)
                points[p] *= 0.5f;

            //sets the framing vertices of the faces
            Vector3[] framingVertices = new Vector3[12]
            {
            points[1], points[2], points[0],
            points[2], points[3], points[0],
            points[3], points[1], points[0],
            points[2], points[1], points[3]
            };
            int faceCount = 4;
            int sideLength = 4;

            List<Vector3> vertices = new List<Vector3>();
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
                        vertices.Add(MathExtensions.LerpBarycentric(u, v, v0, v1, v2));
                    }
                }
            }

            TestUtilities.AssertAreApproximatelyEqual(vertices.ToArray(), outputMesh.vertices);
        }

        [Test]
        public void CreateMeshSetsUVsProperly()
        {
            MeshCreatorTetrahedron creator = new MeshCreatorTetrahedron();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 40;

            Mesh outputMesh = creator.CreateMesh(meshDetails);


            //sets the framing points for the shape
            float a = 1f / 3f;
            float b = Mathf.Sqrt(8f / 9f);
            float c = Mathf.Sqrt(2f / 9f);
            float d = Mathf.Sqrt(2f / 3f);

            Vector3[] points = new Vector3[4];
            points[0] = new Vector3(0f, 1f, 0f);
            points[1] = new Vector3(0f, -a, b);
            points[2] = new Vector3(-d, -a, -c);
            points[3] = new Vector3(d, -a, -c);

            for (int p = 0; p < points.Length; p++)
                points[p] *= 0.5f;

            int faceCount = 4;
            int sideLength = 4;

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
        public void CreateMeshSetsNormalsProperly()
        {
            MeshCreatorTetrahedron creator = new MeshCreatorTetrahedron();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 40;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshSetsTangentsProperly()
        {
            MeshCreatorTetrahedron creator = new MeshCreatorTetrahedron();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 40;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshSetsTrisProperly()
        {
            MeshCreatorTetrahedron creator = new MeshCreatorTetrahedron();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 40;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            int faceCount = 4;
            int sideLength = 3;
            int vertsPerSide = 4;

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
        public void CreateMeshAppliesTransformOperationsProperly()
        {
            MeshCreatorTetrahedron creator = new MeshCreatorTetrahedron();
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 40;
            meshDetails.translation = new Vector3(-5, 100f, 23f);
            meshDetails.rotation = Quaternion.Euler(30f, -230f, 76.23f);
            meshDetails.scale = new Vector3(0.23f, 4f, 1.423f);

            Mesh outputMesh = creator.CreateMesh(meshDetails);


            //sets the framing points for the shape
            float a = 1f / 3f;
            float b = Mathf.Sqrt(8f / 9f);
            float c = Mathf.Sqrt(2f / 9f);
            float d = Mathf.Sqrt(2f / 3f);

            //sets the framing points for the shape
            Vector3[] points = new Vector3[4];
            points[0] = new Vector3(b, 0f, -a);
            points[1] = new Vector3(-c, d, -a);
            points[2] = new Vector3(-c, -d, -a);
            points[3] = new Vector3(0f, 0f, 1f);

            //rotates all points so that vertex 0 is in the forward direction
            for (int i = 0; i < points.Length; i++)
                points[i] = Quaternion.Euler(new Vector3(20f, 0f, 90f)) * points[i];

            for (int p = 0; p < points.Length; p++)
                points[p] *= 0.5f;

            //transform the framing vertices
            for (int p = 0; p < points.Length; p++)
            {
                points[p] = Vector3.Scale(points[p], meshDetails.scale);
                points[p] = meshDetails.rotation * points[p];
                points[p] += meshDetails.translation;
            }


            //sets the framing vertices of the faces
            Vector3[] framingVertices = new Vector3[12]
            {
            points[1], points[2], points[0],
            points[2], points[3], points[0],
            points[3], points[1], points[0],
            points[2], points[1], points[3]
            };
            int faceCount = 4;
            int sideLength = 4;

            List<Vector3> vertices = new List<Vector3>();
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
                        vertices.Add(MathExtensions.LerpBarycentric(u, v, v0, v1, v2));
                    }
                }
            }

            TestUtilities.AssertAreApproximatelyEqual(vertices.ToArray(), outputMesh.vertices);
            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region GetClosestViableVertexCount(int)

        [Test]
        public void GetClosestViableVertexCountReturns12WhenRequestedIsLessThan0()
        {
            MeshCreatorTetrahedron creator = new MeshCreatorTetrahedron();

            int expected = 12;
            int actual = creator.GetClosestViableVertexCount(-4);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns12WhenRequestIs0()
        {
            MeshCreatorTetrahedron creator = new MeshCreatorTetrahedron();

            int expected = 12;
            int actual = creator.GetClosestViableVertexCount(-4);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns40WhenRequestIs44()
        {
            MeshCreatorTetrahedron creator = new MeshCreatorTetrahedron();

            int expected = 40;
            int actual = creator.GetClosestViableVertexCount(44);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns312WhenRequestIs330()
        {
            MeshCreatorTetrahedron creator = new MeshCreatorTetrahedron();

            int expected = 312;
            int actual = creator.GetClosestViableVertexCount(330);

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}