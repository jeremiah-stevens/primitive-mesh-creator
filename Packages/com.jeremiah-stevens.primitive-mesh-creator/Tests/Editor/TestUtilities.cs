using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;


namespace PrimitiveCreator.Tests
{
    /// <summary>
    /// Set of supporting functions for test suites.
    /// </summary>
    public static class TestUtilities
    {
        #region Mesh Comparison

        public static void AssertMeshesAreApproximatelyTheSame(Mesh expected, Mesh actual)
        {
            //TODO: look for other attributes to compare for (ex: UV channels)
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            AssertAreApproximatelyEqual(expected.vertices, actual.vertices);
            AssertAreApproximatelyEqual(expected.uv, actual.uv);
            AssertAreApproximatelyEqual(expected.normals, actual.normals);
            AssertAreApproximatelyEqual(expected.tangents, actual.tangents);
            AssertAreEqual(expected.triangles, actual.triangles);
        }

        public static Mesh CopyMesh(Mesh original)
        {
            Mesh copy = new Mesh();

            copy.vertices = original.vertices;
            copy.triangles = original.triangles;
            copy.uv = original.uv;
            copy.normals = original.normals;
            copy.tangents = original.tangents;

            return copy;
        }

        public static void AssertMeshNormalsAreSetProperly(Mesh original)
        {
            Mesh expectedMesh = TestUtilities.CopyMesh(original);
            expectedMesh.RecalculateNormals();
            TestUtilities.AssertAreApproximatelyEqual(expectedMesh.normals, original.normals);
        }

        public static void AssertMeshTangentsAreSetProperly(Mesh original)
        {
            Mesh expectedMesh = TestUtilities.CopyMesh(original);
            expectedMesh.RecalculateTangents();
            TestUtilities.AssertAreApproximatelyEqual(expectedMesh.tangents, original.tangents);
        }

        #endregion

        public static void AssertAreEqual(int[] expected, int[] actual)
        {
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

        }

        public static void AssertAreApproximatelyEqual(Vector4[] expected, Vector4[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (int i = 0; i < expected.Length; i++)
                Assert.AreApproximatelyEqual(Vector4.Distance(expected[i], actual[i]), Mathf.Epsilon);
        }

        public static void AssertAreApproximatelyEqual(Vector3[] expected, Vector3[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (int i = 0; i < expected.Length; i++)
                Assert.AreApproximatelyEqual(Vector3.Distance(expected[i], actual[i]), Mathf.Epsilon);
        }

        public static void AssertAreApproximatelyEqual(Vector2[] expected, Vector2[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (int i = 0; i < expected.Length; i++)
                Assert.AreApproximatelyEqual(Vector2.Distance(expected[i], actual[i]), Mathf.Epsilon);
        }

        public static void ReportArray<T>(T[] array)
        {
            if (array.Length == 0) return;

            string s = $"{array[0]}";
            for (int i = 1; i < array.Length; i++)
                s += $", {array[i]}";
            Debug.Log(s);
        }
    }
}