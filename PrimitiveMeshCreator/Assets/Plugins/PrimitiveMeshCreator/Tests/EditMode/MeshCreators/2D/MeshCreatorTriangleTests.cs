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
    /// <summary>
    /// Test suite for MeshCreatorTriangle.
    /// </summary>
    public class MeshCreatorTriangleTests : MonoBehaviour
    {

        #region CreateMesh()

        [Test]
        public void CreateMeshReturnsDefaultMesh()
        {
            MeshCreatorTriangle creator = new MeshCreatorTriangle();
            Mesh outputMesh = creator.CreateMesh();

            Vector3[] vertices = new Vector3[] { new Vector3(-0.5f, -0.5f), new Vector3(0.5f, -0.5f), new Vector3(0f, 0.5f) };
            Vector2[] uv = new Vector2[] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 1f) };

            TestUtilities.AssertAreApproximatelyEqual(vertices, outputMesh.vertices);
            TestUtilities.AssertAreApproximatelyEqual(uv, outputMesh.uv);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region CreateMesh(MeshDetails)

        [Test]
        public void CreateMeshSetsVerticesProperly()
        {
            MeshCreatorTriangle creator = new MeshCreatorTriangle();

            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 6;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            Vector3[] vertices = new Vector3[] { new Vector3(-0.5f, -0.5f), new Vector3(0f, -0.5f), new Vector3(0.5f, -0.5f),
                                                        new Vector3(-0.25f, 0f), new Vector3(0.25f, 0f),
                                                                    new Vector3(0f, 0.5f),
                                           };

            TestUtilities.AssertAreApproximatelyEqual(vertices, outputMesh.vertices);
        }

        [Test]
        public void CreateMeshSetsUVsProperly()
        {
            MeshCreatorTriangle creator = new MeshCreatorTriangle();

            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 6;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            Vector2[] uvs = new Vector2[] { new Vector2(0f, 0f), new Vector2(0.5f, 0f), new Vector2(1f, 0f),
                                             new Vector2(0.25f, 0.5f),  new Vector2(0.75f, 0.5f),
                                                             new Vector2(0.5f, 1f)
                                           };

            TestUtilities.AssertAreApproximatelyEqual(uvs, outputMesh.uv);
        }

        [Test]
        public void CreateMeshSetsTrisProperly()
        {
            MeshCreatorTriangle creator = new MeshCreatorTriangle();

            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 6;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /*     5
             *    / \
             *   3 - 4
             *  / \ / \
             * 0 - 1 - 2
             */
            int[] triangles = new int[12] //4 triangles, 1 tri per triangle, 3 verts per tri -> 4 * 1 * 3 = 12
                {
                1, 0, 3,
                3, 4, 1,
                1, 4, 2,
                3, 5, 4
                };

            TestUtilities.AssertAreEqual(triangles, outputMesh.triangles);
        }

        [Test]
        public void CreateMeshSetsNormalsProperly()
        {
            MeshCreatorTriangle creator = new MeshCreatorTriangle();

            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 6;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
        }


        [Test]
        public void CreateMeshSetsTangentsProperly()
        {
            MeshCreatorTriangle creator = new MeshCreatorTriangle();

            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 6;

            Mesh outputMesh = creator.CreateMesh(meshDetails);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshAppliesTransformOperationsProperly()
        {
            MeshCreatorTriangle creator = new MeshCreatorTriangle();

            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 6;
            meshDetails.translation = new Vector3(-5, 100f, 23f);
            meshDetails.rotation = Quaternion.Euler(30f, -230f, 76.23f);
            meshDetails.scale = new Vector3(0.23f, 4f, 1.423f);

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            Vector3[] vertices = new Vector3[] { new Vector3(-0.5f, -0.5f), new Vector3(0f, -0.5f), new Vector3(0.5f, -0.5f),
                                                        new Vector3(-0.25f, 0f), new Vector3(0.25f, 0f),
                                                                    new Vector3(0f, 0.5f),
                                           };

            for (int v = 0; v < vertices.Length; v++)
            {
                vertices[v] = Vector3.Scale(vertices[v], meshDetails.scale);
                vertices[v] = meshDetails.rotation * vertices[v];
                vertices[v] += meshDetails.translation;
            }

            TestUtilities.AssertAreApproximatelyEqual(vertices, outputMesh.vertices);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region GetClosestViableVertexCount(int)

        [Test]
        public void GetClosestViableVertexCountReturns3WhenRequestedIsLessThan0()
        {
            MeshCreatorTriangle creator = new MeshCreatorTriangle();

            int expected = 3;
            int actual = creator.GetClosestViableVertexCount(-5);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns3WhenRequestIs0()
        {
            MeshCreatorTriangle creator = new MeshCreatorTriangle();

            int expected = 3;
            int actual = creator.GetClosestViableVertexCount(0);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns6WhenRequestIs9()
        {
            MeshCreatorTriangle creator = new MeshCreatorTriangle();

            int expected = 6;
            int actual = creator.GetClosestViableVertexCount(9);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns10WhenRequestIs14()
        {
            MeshCreatorTriangle creator = new MeshCreatorTriangle();

            int expected = 10;
            int actual = creator.GetClosestViableVertexCount(14);

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}