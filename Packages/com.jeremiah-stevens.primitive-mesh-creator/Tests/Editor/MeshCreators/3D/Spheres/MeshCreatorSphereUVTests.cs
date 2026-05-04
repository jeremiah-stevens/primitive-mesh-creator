using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PrimitiveCreator;
using PrimitiveCreator.Tests;

/*TODO:
 * • CreateMesh(MeshDetails)
 *    > Custom details
 */
namespace PrimitiveCreator.MeshCreators
{
    public class MeshCreatorSphereUVTests : MonoBehaviour
    {
        #region CreateMesh()

        [Test]
        public void CreateMeshReturnsDefaultMesh()
        {
            MeshCreatorSphereUV creator = new MeshCreatorSphereUV();
            Mesh outputMesh = creator.CreateMesh();

            /*    24,19      <- ( 0,  r, 0) 
             *   /  |   \
             * 17   |    14
             *  |\18,13-/|
             *  |   |    |
             * 11  12,7  8
             *  \   |   /
             *     6,0     <- ( 0, -r, 0)
             */
            float r = 0.5f;
            int meridianCount = 5;
            int parallelCount = 5;

            List<Vector3> vertices = new List<Vector3>(30);
            for (int y = 0; y < meridianCount; y++) //meridian line (i.e. y change)
            {
                float t_y = (float)y / (meridianCount - 1f);
                float phi = Mathf.PI * t_y;
                for (int x = 0; x <= parallelCount; x++) //parallel line (i.e. x change)
                {
                    float t_x = (float)x / parallelCount;
                    float theta = 2f * Mathf.PI * t_x;
                    vertices.Add(MathExtensions.SphericalToCartesian(r, theta, phi));
                }
            }

            Quaternion rotation = Quaternion.Euler(90f, 270f, 0f);
            for (int v = 0; v < vertices.Count; v++)
                vertices[v] = rotation * vertices[v];
            TestUtilities.AssertAreApproximatelyEqual(vertices.ToArray(), outputMesh.vertices);

            //verifies uv coordinates
            int c = 0;
            for (int y = 0; y < meridianCount; y++)
            {
                if (y == 0 || y == meridianCount - 1) //top and bottom rings
                {
                    float offset = (1f / (float)parallelCount) * 0.5f;
                    for (int x = 0; x <= parallelCount; x++)
                    {
                        Vector2 uv = new Vector2(Mathf.Clamp(((float)x / (float)parallelCount) + offset, 0f, 1f), (float)y / (float)(meridianCount - 1f));
                        Assert.Less(Vector3.Distance(uv, outputMesh.uv[c++]), 0.01f);
                    }
                }
                else //middle rings
                {
                    for (int x = 0; x <= parallelCount; x++)
                    {
                        Vector2 uv = new Vector2((float)x / parallelCount, (float)y / (meridianCount - 1f));
                        Assert.Less(Vector3.Distance(uv, outputMesh.uv[c++]), Mathf.Epsilon);
                    }
                }
            }

            //verifies tris
            int vertsPerRing = parallelCount + 1; //number of vertices in the parallel line
            int outTri = 0; //index in outputMesh.triangle we're checking
            int t = 0; //index of the expected
            for (int y = 0; y < meridianCount - 1; y++)
            {
                if (y == 0) //bottom rings
                {
                    for (int x = 0; x < parallelCount; x++)
                    {
                        Assert.AreEqual(t + vertsPerRing + 1, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerRing, outputMesh.triangles[outTri++]);
                        t += 1;
                    }
                }
                else if (y == meridianCount - 2) //top ring
                {
                    for (int x = 0; x < parallelCount; x++)
                    {
                        Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerRing, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + 1, outputMesh.triangles[outTri++]);
                        t += 1;
                    }
                }
                else //middle rings
                {
                    for (int x = 0; x < parallelCount; x++)
                    {
                        Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerRing, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerRing + 1, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerRing + 1, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + 1, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                        t += 1;
                    }
                }
                t += 1;
            }

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region CreateMesh(MeshDetails)

        [Test]
        public void CreateMeshSetsVerticesProperly()
        {
            MeshCreatorSphereUV creator = new MeshCreatorSphereUV();
            MeshCreatorSphereUV.MeshDetailsSphereUV meshDetails = new MeshCreatorSphereUV.MeshDetailsSphereUV();
            int meridianLines = 7;
            int parallelLines = meridianLines + 1;
            meshDetails.vertexCount = meridianLines * parallelLines; //7 meridian and 8 parallel lines (1 extra parallel line for wrapping of uv coordinates)

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /*    55,49      <- ( 0,  r, 0) 
             *   /  |   \
             * 17   |    14
             *  |\18,13-/|
             *  |   |    |
             * 11  12,7  8
             *  \   |   /
             *     6,0     <- ( 0, -r, 0)
             */
            float r = 0.5f;
            int meridianCount = meridianLines;
            int parallelCount = parallelLines - 1;

            List<Vector3> vertices = new List<Vector3>(30);
            for (int y = 0; y < meridianCount; y++) //meridian line (i.e. y change)
            {
                float t_y = (float)y / (meridianCount - 1f);
                float phi = Mathf.PI * t_y;
                for (int x = 0; x <= parallelCount; x++) //parallel line (i.e. x change)
                {
                    float t_x = (float)x / parallelCount;
                    float theta = 2f * Mathf.PI * t_x;
                    vertices.Add(MathExtensions.SphericalToCartesian(r, theta, phi));
                }
            }

            Quaternion rotation = Quaternion.Euler(90f, 270f, 0f);
            for (int v = 0; v < vertices.Count; v++)
                vertices[v] = rotation * vertices[v];
            TestUtilities.AssertAreApproximatelyEqual(vertices.ToArray(), outputMesh.vertices);
        }

        [Test]
        public void CreateMeshSetsUVsProperly()
        {
            MeshCreatorSphereUV creator = new MeshCreatorSphereUV();
            MeshCreatorSphereUV.MeshDetailsSphereUV meshDetails = new MeshCreatorSphereUV.MeshDetailsSphereUV();
            int meridianLines = 7;
            int parallelLines = meridianLines + 1;
            meshDetails.vertexCount = meridianLines * parallelLines; //7 meridian and 8 parallel lines (1 extra parallel line for wrapping of uv coordinates)

            Mesh outputMesh = creator.CreateMesh(meshDetails);


            int meridianCount = meridianLines;
            int parallelCount = parallelLines - 1;

            //verifies uv coordinates
            int c = 0;
            for (int y = 0; y < meridianCount; y++)
            {
                if (y == 0 || y == meridianCount - 1) //top and bottom rings
                {
                    float offset = (1f / (float)parallelCount) * 0.5f;
                    for (int x = 0; x <= parallelCount; x++)
                    {
                        Vector2 uv = new Vector2(Mathf.Clamp(((float)x / (float)parallelCount) + offset, 0f, 1f), (float)y / (float)(meridianCount - 1f));
                        Assert.Less(Vector3.Distance(uv, outputMesh.uv[c++]), 0.01f);
                    }
                }
                else //middle rings
                {
                    for (int x = 0; x <= parallelCount; x++)
                    {
                        Vector2 uv = new Vector2((float)x / parallelCount, (float)y / (meridianCount - 1f));
                        Assert.Less(Vector3.Distance(uv, outputMesh.uv[c++]), Mathf.Epsilon);
                    }
                }
            }
        }

        [Test]
        public void CreateMeshSetsTrisProperly()
        {
            MeshCreatorSphereUV creator = new MeshCreatorSphereUV();
            MeshCreatorSphereUV.MeshDetailsSphereUV meshDetails = new MeshCreatorSphereUV.MeshDetailsSphereUV();
            int meridianLines = 7;
            int parallelLines = meridianLines + 1;
            meshDetails.vertexCount = meridianLines * parallelLines; //7 meridian and 8 parallel lines (1 extra parallel line for wrapping of uv coordinates)

            Mesh outputMesh = creator.CreateMesh(meshDetails);


            int meridianCount = meridianLines;
            int parallelCount = parallelLines - 1;

            //verifies tris
            int vertsPerRing = parallelCount + 1; //number of vertices in the parallel line
            int outTri = 0; //index in outputMesh.triangle we're checking
            int t = 0; //index of the expected
            for (int y = 0; y < meridianCount - 1; y++)
            {
                if (y == 0) //bottom rings
                {
                    for (int x = 0; x < parallelCount; x++)
                    {
                        Assert.AreEqual(t + vertsPerRing + 1, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerRing, outputMesh.triangles[outTri++]);
                        t += 1;
                    }
                }
                else if (y == meridianCount - 2) //top ring
                {
                    for (int x = 0; x < parallelCount; x++)
                    {
                        Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerRing, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + 1, outputMesh.triangles[outTri++]);
                        t += 1;
                    }
                }
                else //middle rings
                {
                    for (int x = 0; x < parallelCount; x++)
                    {
                        Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerRing, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerRing + 1, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + vertsPerRing + 1, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t + 1, outputMesh.triangles[outTri++]);
                        Assert.AreEqual(t, outputMesh.triangles[outTri++]);
                        t += 1;
                    }
                }
                t += 1;
            }
        }

        [Test]
        public void CreateMeshSetsNormalsProperly()
        {
            MeshCreatorSphereUV creator = new MeshCreatorSphereUV();
            MeshCreatorSphereUV.MeshDetailsSphereUV meshDetails = new MeshCreatorSphereUV.MeshDetailsSphereUV();
            int meridianLines = 7;
            int parallelLines = meridianLines + 1;
            meshDetails.vertexCount = meridianLines * parallelLines; //7 meridian and 8 parallel lines (1 extra parallel line for wrapping of uv coordinates)

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshSetsTangentsProperly()
        {
            MeshCreatorSphereUV creator = new MeshCreatorSphereUV();
            MeshCreatorSphereUV.MeshDetailsSphereUV meshDetails = new MeshCreatorSphereUV.MeshDetailsSphereUV();
            int meridianLines = 7;
            int parallelLines = meridianLines + 1;
            meshDetails.vertexCount = meridianLines * parallelLines; //7 meridian and 8 parallel lines (1 extra parallel line for wrapping of uv coordinates)

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshAppliesTransformOperationsProperly()
        {
            MeshCreatorSphereUV creator = new MeshCreatorSphereUV();
            MeshCreatorSphereUV.MeshDetailsSphereUV meshDetails = new MeshCreatorSphereUV.MeshDetailsSphereUV();
            int meridianLines = 7;
            int parallelLines = meridianLines + 1;
            meshDetails.vertexCount = meridianLines * parallelLines; //7 meridian and 8 parallel lines (1 extra parallel line for wrapping of uv coordinates)
            meshDetails.translation = new Vector3(-5, 100f, 23f);
            meshDetails.rotation = Quaternion.Euler(30f, -230f, 76.23f);
            meshDetails.scale = new Vector3(0.23f, 4f, 1.423f);

            Mesh outputMesh = creator.CreateMesh(meshDetails);


            float r = 0.5f;
            int meridianCount = meridianLines;
            int parallelCount = parallelLines - 1;

            List<Vector3> vertices = new List<Vector3>(30);
            for (int y = 0; y < meridianCount; y++) //meridian line (i.e. y change)
            {
                float t_y = (float)y / (meridianCount - 1f);
                float phi = Mathf.PI * t_y;
                for (int x = 0; x <= parallelCount; x++) //parallel line (i.e. x change)
                {
                    float t_x = (float)x / parallelCount;
                    float theta = 2f * Mathf.PI * t_x;
                    vertices.Add(MathExtensions.SphericalToCartesian(r, theta, phi));
                }
            }

            Quaternion rotation = Quaternion.Euler(90f, 270f, 0f);
            for (int v = 0; v < vertices.Count; v++)
                vertices[v] = rotation * vertices[v];

            //applies transformation operations to expected vertices
            for (int v = 0; v < vertices.Count; v++)
            {
                vertices[v] = Vector3.Scale(vertices[v], meshDetails.scale);
                vertices[v] = meshDetails.rotation * vertices[v];
                vertices[v] += meshDetails.translation;
            }
            TestUtilities.AssertAreApproximatelyEqual(vertices.ToArray(), outputMesh.vertices);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region GetClosestViableVertexCount(int)

        [Test]
        public void GetClosestViableVertexCountReturns30WhenRequestedIsLessThan0()
        {
            MeshCreatorSphereUV creator = new MeshCreatorSphereUV();

            int expected = 30;
            int actual = creator.GetClosestViableVertexCount(-4);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns30WhenRequestIs0()
        {
            MeshCreatorSphereUV creator = new MeshCreatorSphereUV();

            int expected = 30;
            int actual = creator.GetClosestViableVertexCount(0);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns110WhenRequestIs103()
        {
            MeshCreatorSphereUV creator = new MeshCreatorSphereUV();

            int expected = 110;
            int actual = creator.GetClosestViableVertexCount(103);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns2550WhenRequestIs2600()
        {
            MeshCreatorSphereUV creator = new MeshCreatorSphereUV();

            int expected = 2550;
            int actual = creator.GetClosestViableVertexCount(2600);

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}