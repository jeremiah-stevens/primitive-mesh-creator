using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using PrimitiveCreator.Tests;

/*TODO:
 * • CreateMesh(MeshDetails)
 *    > Custom details
 */
namespace PrimitiveCreator.MeshCreators
{
    public class MeshCreatorPolygonTests : MonoBehaviour
    {
        #region CreateMesh()

        [Test]
        public void CreateMeshReturnsDefaultMesh()
        {
            MeshCreatorPolygon creator = new MeshCreatorPolygon();
            Mesh outputMesh = creator.CreateMesh();

            Vector3 vert = Vector3.up * 0.5f;

            //get the set of vertex points around the circle
            float angleValue = 72f; //360 / 5
            Vector3[] points = new Vector3[6];
            for (int p = 0; p < points.Length - 1; p++)
                points[p] = Quaternion.AngleAxis(angleValue * p, Vector3.forward * -1f) * vert;
            points[5] = Vector3.zero; //center vertex

            //set the vertex faces
            Vector3[] vertices = new Vector3[5 * 3]
            {
            points[1], points[0], points[5],
            points[2], points[1], points[5],
            points[3], points[2], points[5],
            points[4], points[3], points[5],
            points[0], points[4], points[5]
            };

            //set the uv coordinates
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int v = 0; v < uvs.Length; v++)
                uvs[v] = vertices[v] + new Vector3(0.5f, 0.5f);

            TestUtilities.AssertAreApproximatelyEqual(vertices, outputMesh.vertices);
            TestUtilities.AssertAreApproximatelyEqual(uvs, outputMesh.uv);
            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region CreateMesh(MeshDetails)

        [Test]
        public void CreateMeshSetsVerticesProperly()
        {
            MeshCreatorPolygon creator = new MeshCreatorPolygon();
            MeshCreatorPolygon.MeshDetailsPolygon meshDetails = new MeshCreatorPolygon.MeshDetailsPolygon();
            meshDetails.vertexCount = 9;
            meshDetails.n = 3;

            //gets actual
            Mesh outputMesh = creator.CreateMesh(meshDetails);


            //gets expected

            //get the set of vertex points around the circle
            Vector3 vert = Vector3.up * 0.5f;
            float angleValue = 120f; //360 / 3
            Vector3[] points = new Vector3[4];
            for (int p = 0; p < points.Length - 1; p++)
                points[p] = Quaternion.AngleAxis(angleValue * p, Vector3.forward * -1f) * vert;
            points[3] = Vector3.zero; //center vertex

            /*3 faces; testing just bounds instead of specific vertices
             *      /|\
             *     / | \
             *    /3 | 1\
             *   /  /2\  \
             *  -----------
             */

            //face 1 (0, 1, 2)
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Vector3.Distance(points[1], outputMesh.vertices[0]), Mathf.Epsilon);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Vector3.Distance(points[0], outputMesh.vertices[1]), Mathf.Epsilon);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Vector3.Distance(points[3], outputMesh.vertices[2]), Mathf.Epsilon);

            //face 2 (3, 4, 5)
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Vector3.Distance(points[2], outputMesh.vertices[3]), Mathf.Epsilon);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Vector3.Distance(points[1], outputMesh.vertices[4]), Mathf.Epsilon);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Vector3.Distance(points[3], outputMesh.vertices[5]), Mathf.Epsilon);

            //face 3 (6, 7, 8)
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Vector3.Distance(points[0], outputMesh.vertices[6]), Mathf.Epsilon);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Vector3.Distance(points[2], outputMesh.vertices[7]), Mathf.Epsilon);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Vector3.Distance(points[3], outputMesh.vertices[8]), Mathf.Epsilon);
        }

        [Test]
        public void CreateMeshSetsUVsProperly()
        {
            MeshCreatorPolygon creator = new MeshCreatorPolygon();
            MeshCreatorPolygon.MeshDetailsPolygon meshDetails = new MeshCreatorPolygon.MeshDetailsPolygon();
            meshDetails.vertexCount = 9;
            meshDetails.n = 3;

            //gets actual
            Mesh outputMesh = creator.CreateMesh(meshDetails);


            //gets expected

            //get the set of vertex points around the circle
            Vector3 uvOffset = new Vector3(0.5f, 0.5f);
            Vector3 vert = Vector3.up * 0.5f;
            float angleValue = 120f; //360 / 3
            Vector3[] points = new Vector3[4];
            for (int p = 0; p < points.Length - 1; p++)
                points[p] = Quaternion.AngleAxis(angleValue * p, Vector3.forward * -1f) * vert + uvOffset;
            points[3] = Vector3.zero + uvOffset; //center vertex

            Vector3[] framingVertices = new Vector3[9]
            {
            points[1], points[0], points[3],
            points[2], points[1], points[3],
            points[0], points[2], points[3]
            };

            for (int v = 0; v < framingVertices.Length; v++)
            {
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(Vector3.Distance(framingVertices[v], outputMesh.uv[v]), Mathf.Epsilon);
            }
        }

        [Test]
        public void CreateMeshSetsTrisProperly()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();

            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 6;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            /* Polygon currently generates as 3 separate faces
             *     5           11          17
             *    / \          / \         / \
             *   3 - 4        9 -10      15 - 16
             *  / \ / \      / \ / \     / \ / \
             * 0 - 1 - 2    6 - 7 - 8   12 -13 -14
             */
            int[] triangles = new int[36] //3 faces, 4 triangles, 1 tri per triangle, 3 verts per tri -> 3 * 4 * 1 * 3 = 36
                {
                0, 3, 1, 1, 3, 4, 1, 4, 2, 3, 5, 4,
                6, 9, 7, 7, 9, 10, 7, 10, 8, 9, 11, 10,
                12, 15, 13, 13, 15, 16, 13, 16, 14, 15, 17, 16
                };

            TestUtilities.Equals(triangles, outputMesh.triangles);
        }

        [Test]
        public void CreateMeshSetsNormalsProperly()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 6;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshSetsTangentsProperly()
        {
            MeshCreatorQuad creator = new MeshCreatorQuad();
            MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
            meshDetails.vertexCount = 6;

            Mesh outputMesh = creator.CreateMesh(meshDetails);

            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateMeshAppliesTransformOperationsProperly()
        {
            MeshCreatorPolygon creator = new MeshCreatorPolygon();
            MeshCreatorPolygon.MeshDetailsPolygon meshDetails = new MeshCreatorPolygon.MeshDetailsPolygon();
            meshDetails.vertexCount = 15;
            meshDetails.n = 5;
            meshDetails.translation = new Vector3(-5, 100f, 23f);
            meshDetails.rotation = Quaternion.Euler(30f, -230f, 76.23f);
            meshDetails.scale = new Vector3(0.23f, 4f, 1.423f);

            //gets actual
            Mesh outputMesh = creator.CreateMesh(meshDetails);


            //gets expected

            //get the set of vertex points around the circle
            Vector3 vert = Vector3.up * 0.5f;
            float angleValue = 72f; //360 / 5
            Vector3[] points = new Vector3[6];
            for (int p = 0; p < points.Length - 1; p++)
                points[p] = Quaternion.AngleAxis(angleValue * p, Vector3.forward * -1f) * vert;
            points[5] = Vector3.zero; //center vertex

            //set the vertex faces
            Vector3[] vertices = new Vector3[5 * 3]
            {
            points[1], points[0], points[5],
            points[2], points[1], points[5],
            points[3], points[2], points[5],
            points[4], points[3], points[5],
            points[0], points[4], points[5]
            };

            //set the uv coordinates
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int v = 0; v < uvs.Length; v++)
                uvs[v] = vertices[v] + new Vector3(0.5f, 0.5f);

            //applies transformation operations
            for (int v = 0; v < vertices.Length; v++)
            {
                vertices[v] = Vector3.Scale(vertices[v], meshDetails.scale);
                vertices[v] = meshDetails.rotation * vertices[v];
                vertices[v] += meshDetails.translation;
            }

            TestUtilities.AssertAreApproximatelyEqual(vertices, outputMesh.vertices);
            TestUtilities.AssertAreApproximatelyEqual(uvs, outputMesh.uv);
            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region GetClosestViableVertexCount(int)

        [Test]
        public void GetClosestViableVertexCountReturns15WhenRequestedIsLessThan0()
        {
            MeshCreatorPolygon creator = new MeshCreatorPolygon();

            int expected = 15;
            int actual = creator.GetClosestViableVertexCount(-5);

            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns15WhenRequestIs0()
        {
            MeshCreatorPolygon creator = new MeshCreatorPolygon();

            int expected = 15;
            int actual = creator.GetClosestViableVertexCount(0);

            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns30WhenRequestIs35()
        {
            MeshCreatorPolygon creator = new MeshCreatorPolygon();

            int expected = 30;
            int actual = creator.GetClosestViableVertexCount(35);

            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns50WhenRequestIs54()
        {
            MeshCreatorPolygon creator = new MeshCreatorPolygon();

            int expected = 50;
            int actual = creator.GetClosestViableVertexCount(54);

            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns45WhenRequestedIs54AndNIs3()
        {
            MeshCreatorPolygon creator = new MeshCreatorPolygon();

            int expected = 45;
            int actual = creator.GetClosestViableVertexCount(54, 3);

            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetClosestViableVertexCountReturns102WhenRequestedIs0AndNIs34()
        {
            MeshCreatorPolygon creator = new MeshCreatorPolygon();

            int expected = 102;
            int actual = creator.GetClosestViableVertexCount(0, 34);

            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}