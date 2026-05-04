using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PrimitiveCreator;
using PrimitiveCreator.Tests;


namespace PrimitiveCreator.MeshCreators
{
    /// <summary>
    /// Test suite for MeshCreator (base class for all procedural mesh creators).
    /// </summary>
    public class MeshCreatorTests : MeshCreator
    {
        //Stub items to access internal operations of MeshCreator. Ignore for testing purposes.
        public override PrimitiveCreatorUtility.MeshType MeshType => throw new System.NotImplementedException();

        public override string DisplayName => "MeshCreatorTest";

        public override Mesh CreateMesh(MeshDetails meshDetails) { throw new System.NotImplementedException(); }

        public override int GetClosestViableVertexCount(int proposedVertexCount) { throw new System.NotImplementedException(); }

        public override Mesh CreateColliderMesh(MeshDetails meshDetails) { throw new System.NotImplementedException(); }




        #region SetTri()

        [Test]
        public void SetTriSetsTriangleIndicesProperly()
        {
            int v00 = 1;
            int v01 = 2;
            int v10 = 3;
            int[] triangles = { -1, -1, -1 };

            int output = SetTri(triangles, 0, v00, v01, v10);

            Assert.AreEqual(new int[3] { v00, v01, v10 }, triangles);
            Assert.AreEqual(3, output);
        }

        #endregion



        #region SetQuad()

        [Test]
        public void SetQuadSetsTriangleIndicesProperly()
        {
            int v00 = 1;
            int v01 = 2;
            int v11 = 3;
            int v10 = 4;
            int[] triangles = { -1, -1, -1, -1, -1, -1 };

            int output = SetQuad(triangles, 0, v00, v01, v11, v10);

            Assert.AreEqual(new int[6] { v00, v01, v11, v11, v10, v00 }, triangles);
            Assert.AreEqual(6, output);
        }

        #endregion



        #region TransformMesh()

        [Test]
        public void TransformMeshModifiesAllVerticesProperly()
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[] { Vector3.zero, Vector3.up, Vector3.right, };
            mesh.vertices = vertices;
            Vector3 translation = Vector3.forward;
            Quaternion rotation = Quaternion.Euler(new Vector3(30f, 90f, 0f));
            Vector3 scale = new Vector3(2, 2, 2);

            TransformMesh(ref mesh, translation, rotation, scale);

            Vector3[] expected = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                expected[i] = Vector3.Scale(vertices[i], scale);
                expected[i] = rotation * expected[i];
                expected[i] += translation;
            }

            Assert.AreEqual(expected, mesh.vertices);

            TestUtilities.AssertMeshNormalsAreSetProperly(mesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(mesh);
        }

        #endregion



        #region Triangle Mesh Creation

        [Test]
        public void CreateTriangleFailsIfFramingMeshIsNull()
        {
            Mesh mesh = null;

            Assert.That(() => CreateTriangle(ref mesh, 3, 0, 1, 2),
                              Throws.TypeOf<System.NullReferenceException>());
        }

        [Test]
        public void CreateTriangleFailsIfVertexIndicesAreOutOfBounds()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] { Vector3.left, Vector3.up, Vector3.right };

            Assert.That(() => CreateTriangle(ref mesh, 3, 0, 1, -1),
                              Throws.TypeOf<System.IndexOutOfRangeException>());
        }

        [Test]
        public void CreateTriangleUses3VertexCountWhenRequestedIs0()
        {
            Vector3[] vertices = new Vector3[] { Vector3.zero, new Vector3(0.5f, 1f), Vector3.right };
            Vector2[] uvs = new Vector2[] { Vector2.zero, new Vector2(0.5f, 1), Vector2.right };

            Mesh inputMesh = new Mesh();
            inputMesh.vertices = vertices;
            inputMesh.uv = uvs;

            Mesh outputMesh = CreateTriangle(ref inputMesh, 0, 0, 1, 2);

            Assert.AreEqual(3, outputMesh.vertexCount);
            Assert.AreEqual(new Vector3[] { Vector3.zero, Vector3.right, new Vector3(0.5f, 1f) }, outputMesh.vertices);
            Assert.AreEqual(3, outputMesh.uv.Length);
            Assert.AreEqual(new Vector2[] { Vector2.zero, Vector2.right, new Vector2(0.5f, 1) }, outputMesh.uv);
            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateTriangleUses6VertexCountWhenRequestedIs8()
        {
            Vector3[] vertices = new Vector3[] { Vector3.zero, new Vector3(0.5f, 1f), Vector3.right };
            Vector2[] uvs = new Vector2[] { Vector2.zero, new Vector2(0.5f, 1), Vector2.right };

            Mesh inputMesh = new Mesh();
            inputMesh.vertices = vertices;
            inputMesh.uv = uvs;

            Mesh outputMesh = CreateTriangle(ref inputMesh, 8, 0, 1, 2);

            /*       v02
             *      /   \
             *    v01 - v11
             *   /   \ /   \
             * v00 - v10 - v20
             */
            Assert.AreEqual(6, outputMesh.vertexCount);

            //tests vertices
            Vector3[] expectedVerts = new Vector3[]
            {
            Vector3.zero, new Vector3(0.5f, 0f), new Vector3(1f, 0f),
            new Vector3(0.3f, 0.5f), new Vector3(0.8f, 0.5f),
            new Vector3(0.5f, 1f)
            };
            for (int i = 0; i < outputMesh.vertices.Length; i++)
                Assert.Less(Vector3.Distance(outputMesh.vertices[i], expectedVerts[i]), 0.1f);

            //tests uv coordinates
            Vector2[] expectedUvs = new Vector2[]
            {
            Vector3.zero, new Vector3(0.5f, 0f), new Vector3(1f, 0f),
            new Vector3(0.3f, 0.5f), new Vector3(0.8f, 0.5f),
            new Vector3(0.5f, 1f)
            };
            for (int i = 0; i < outputMesh.uv.Length; i++)
                Assert.Less(Vector3.Distance(outputMesh.uv[i], expectedUvs[i]), 0.1f);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion



        #region Quad Mesh Creation

        [Test]
        public void CreateQuadFailsIfFramingMeshIsNull()
        {
            Mesh mesh = null;

            Assert.That(() => CreateQuad(ref mesh, 3, 0, 1, 2, 3),
                              Throws.TypeOf<System.NullReferenceException>());
        }

        [Test]
        public void CreateQuadFailsIfVertexIndicesAreOutOfBounds()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] { Vector3.zero, Vector3.up, new Vector3(1, 1), Vector3.right };

            Assert.That(() => CreateQuad(ref mesh, 3, 0, 1, 2, -1),
                              Throws.TypeOf<System.IndexOutOfRangeException>());
        }

        [Test]
        public void CreateQuadUses4VertexCountWhenRequestedIs0()
        {
            Vector3[] vertices = new Vector3[] { Vector3.zero, Vector3.up, new Vector3(1, 1), Vector3.right };
            Vector2[] uvs = new Vector2[] { Vector2.zero, Vector3.up, new Vector3(1, 1), Vector2.right };

            Mesh inputMesh = new Mesh();
            inputMesh.vertices = vertices;
            inputMesh.uv = uvs;

            Mesh outputMesh = CreateQuad(ref inputMesh, 0, 0, 1, 2, 3);

            Assert.AreEqual(4, outputMesh.vertexCount);
            Assert.AreEqual(new Vector3[] { Vector3.zero, Vector3.right, Vector3.up, new Vector3(1, 1) }, outputMesh.vertices);
            Assert.AreEqual(4, outputMesh.uv.Length);
            Assert.AreEqual(new Vector2[] { Vector2.zero, Vector2.right, Vector2.up, new Vector2(1, 1) }, outputMesh.uv);
            Assert.AreEqual(new int[] { 0, 2, 3, 3, 1, 0 }, outputMesh.triangles);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        [Test]
        public void CreateQuadUses9VertexCountWhenRequestedIs11()
        {
            Vector3[] vertices = new Vector3[] { Vector3.zero, Vector3.up, new Vector3(1, 1), Vector3.right };
            Vector2[] uvs = new Vector2[] { Vector2.zero, Vector3.up, new Vector3(1, 1), Vector2.right };

            Mesh inputMesh = new Mesh();
            inputMesh.vertices = vertices;
            inputMesh.uv = uvs;

            Mesh outputMesh = CreateQuad(ref inputMesh, 12, 0, 1, 2, 3);

            /* v02 - v12 - v22
             *  |     |     |
             * v01 - v11 - v21
             *  |     |     |
             * v00 - v10 - v20
             */
            Assert.AreEqual(9, outputMesh.vertexCount);

            //tests vertices
            Vector3[] expectedVerts = new Vector3[]
            {
            Vector3.zero, new Vector3(0.5f, 0f), Vector3.right,
            new Vector3(0f, 0.5f), new Vector3(0.5f, 0.5f), new Vector3(1f, 0.5f),
            Vector3.up, new Vector3(0.5f, 1f), new Vector3(1f, 1f)
            };
            for (int i = 0; i < outputMesh.vertices.Length; i++)
                Assert.Less(Vector3.Distance(outputMesh.vertices[i], expectedVerts[i]), 0.1f);

            //tests uv coordinates
            Vector2[] expectedUvs = new Vector2[]
            {
            Vector3.zero, new Vector3(0.5f, 0f), Vector3.right,
            new Vector3(0f, 0.5f), new Vector3(0.5f, 0.5f), new Vector3(1f, 0.5f),
            Vector3.up, new Vector3(0.5f, 1f), new Vector3(1f, 1f)
            };
            for (int i = 0; i < outputMesh.uv.Length; i++)
                Assert.Less(Vector3.Distance(outputMesh.uv[i], expectedUvs[i]), 0.1f);

            TestUtilities.AssertMeshNormalsAreSetProperly(outputMesh);
            TestUtilities.AssertMeshTangentsAreSetProperly(outputMesh);
        }

        #endregion
    }
}