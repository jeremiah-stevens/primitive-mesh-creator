using System.Collections;
using System.Collections.Generic;
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
    /// <summary>
    /// Test suite for MeshCreatorQuad.
    /// </summary>
    public class MeshCreatorQuadTests : MonoBehaviour
    {
        #region CreateMesh()

        [Test]
        public void CreateMeshReturnsDefaultMesh()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();
            Mesh outputMesh = creator.CreateMesh();

            Vector3[] vertices = new Vector3[] { new Vector3(-0.5f, -0.5f), new Vector3(0.5f, -0.5f), new Vector3(-0.5f, 0.5f), new Vector3(0.5f, 0.5f) };
            Vector2[] uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };

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
            MeshCreatorQuad creator = new MeshCreatorQuad();

            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 9;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            Vector3[] vertices = new Vector3[] { new Vector3(-0.5f, -0.5f), new Vector3(0f, -0.5f), new Vector3(0.5f, -0.5f),
                                             new Vector3(-0.5f, 0f),  new Vector3(0f, 0f), new Vector3(0.5f, 0f),
                                             new Vector3(-0.5f, 0.5f),  new Vector3(0f, 0.5f), new Vector3(0.5f, 0.5f)
                                           };

            TestUtilities.AssertAreApproximatelyEqual(vertices, outputMesh.vertices);
        }

        [Test]
        public void CreateMeshSetsUVsProperly()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();

            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 9;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            Vector2[] uvs = new Vector2[] { new Vector2(0f, 0f), new Vector2(0.5f, 0f), new Vector2(1f, 0f),
                                        new Vector2(0f, 0.5f),  new Vector3(0.5f, 0.5f), new Vector3(1f, 0.5f),
                                        new Vector2(0f, 1f),  new Vector3(0.5f, 1f), new Vector3(1f, 1f)
                                      };

            TestUtilities.AssertAreApproximatelyEqual(uvs, outputMesh.uv);
        }

        [Test]
        public void CreateMeshSetsNormalsProperly()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 9;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshSetsTangentsProperly()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 9;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshSetsTrisProperly()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();

            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 9;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /* 6 -- 7 -- 8
             * |  / |  / |
             * 3 -- 4 -- 5
             * |  / |  / |
             * 0 -- 1 -- 2
             */
            int[] triangles = new int[24] //4 quadrants/grids, 2 tris per, 3 verts per tri -> 4 * 2 * 3 = 24
                {
                0, 3, 4, 4, 1, 0,
                1, 4, 5, 5, 2, 1,
                3, 6, 7, 7, 4, 3,
                4, 7, 8, 8, 5, 4
                };

            TestUtilities.Equals(triangles, outputMesh.triangles);
        }

        [Test]
        public void CreateMeshAppliesTransformOperationsProperly()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();

            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 9;
            meshDetails.translation = new Vector3(-5, 100f, 23f);
            meshDetails.rotation = Quaternion.Euler(30f, -230f, 76.23f);
            meshDetails.scale = new Vector3(0.23f, 4f, 1.423f);

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            Vector3[] vertices = new Vector3[] { new Vector3(-0.5f, -0.5f), new Vector3(0f, -0.5f), new Vector3(0.5f, -0.5f),
                                             new Vector3(-0.5f, 0f),  new Vector3(0f, 0f), new Vector3(0.5f, 0f),
                                             new Vector3(-0.5f, 0.5f),  new Vector3(0f, 0.5f), new Vector3(0.5f, 0.5f)
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
        public void GetClosestViableVertexCountReturns4WhenRequestedIsLessThan0()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();

            int expected = 4;
            int actual = creator.GetClosestViableVertexCount(-5);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns4WhenRequestIs0()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();

            int expected = 4;
            int actual = creator.GetClosestViableVertexCount(0);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns9WhenRequestIs10()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();

            int expected = 9;
            int actual = creator.GetClosestViableVertexCount(10);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns16WhenRequestIs20()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();

            int expected = 16;
            int actual = creator.GetClosestViableVertexCount(20);

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}