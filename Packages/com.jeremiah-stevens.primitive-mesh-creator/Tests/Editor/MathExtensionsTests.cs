using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PrimitiveCreator;


namespace PrimitiveCreator
{
    public class MathExtensionsTests : MonoBehaviour
    {
        #region QuadraticFormula()

        [Test]
        public void QuadraticFormulaThrowsInvalidOperationExceptionWhenAIs0()
        {
            Assert.That(() => MathExtensions.QuadraticFormula(0, 1, 1),
                              Throws.TypeOf<System.InvalidOperationException>());
        }

        [Test]
        public void QuadraticFormulaReturnsPositiveThenNegativeSolution()
        {
            //x^2 + 2x + 0 = 0 -> (x = 0 & x = -2)
            float[] expected = new float[2] { 0, -2 };
            float[] actual = MathExtensions.QuadraticFormula(1, 2, 0);

            Assert.AreEqual(expected[0], actual[0], Mathf.Epsilon);
            Assert.AreEqual(expected[1], actual[1], Mathf.Epsilon);
        }

        [Test]
        public void QuadraticFormulaReturnsProperValue()
        {
            float[] expected = new float[] { 5.7966685751165f, -13.934222481189f };
            float[] actual = MathExtensions.QuadraticFormula(2.15421f, 17.53f, -174f);

            Assert.AreEqual(expected[0], actual[0], 0.00001f);
            Assert.AreEqual(expected[1], actual[1], 0.00001f);
        }

        #endregion



        #region LerpBarycentric()

        [Test]
        public void LerpBarycentricClampsUVCoordinatesWhenLessThan0()
        {
            Vector3 a = Vector3.zero;
            Vector3 b = Vector3.right;
            Vector3 c = Vector3.up;

            Assert.Less(Vector3.Distance(a, MathExtensions.LerpBarycentric(-1, -1, a, b, c)), Mathf.Epsilon);
            Assert.Less(Vector3.Distance(a, MathExtensions.LerpBarycentric(0, -1, a, b, c)), Mathf.Epsilon);
            Assert.Less(Vector3.Distance(a, MathExtensions.LerpBarycentric(-1, 0, a, b, c)), Mathf.Epsilon);
        }

        [Test]
        public void LerpBarycentricClampsUVCoordinatesToTotal1()
        {
            Vector3 a = Vector3.zero;
            Vector3 b = Vector3.right;
            Vector3 c = Vector3.up;

            Assert.Less(Vector3.Distance(b, MathExtensions.LerpBarycentric(10, 0, a, b, c)), Mathf.Epsilon); //tests max u/horizontal coord
            Assert.Less(Vector3.Distance(c, MathExtensions.LerpBarycentric(0, 10, a, b, c)), Mathf.Epsilon); //tests max v/vertical coord
            Assert.Less(Vector3.Distance(c * 0.5f, MathExtensions.LerpBarycentric(-5, 0.5f, a, b, c)), Mathf.Epsilon); //tests that negatives do not affect the coordinates
        }

        [Test]
        public void LerpBarycentricReturnsProperValueRandomInput()
        {
            float u = Random.Range(0f, 1f);
            float v = Random.Range(0f, 1f - u);
            Vector3 a = new Vector3(Random.Range(-500f, 500f), Random.Range(-500f, 500f), Random.Range(-500f, 500f));
            Vector3 b = new Vector3(Random.Range(-500f, 500f), Random.Range(-500f, 500f), Random.Range(-500f, 500f));
            Vector3 c = new Vector3(Random.Range(-500f, 500f), Random.Range(-500f, 500f), Random.Range(-500f, 500f));

            Vector3 expected = a + u * (b - a) + v * (c - a);
            Assert.Less(Vector3.Distance(expected, MathExtensions.LerpBarycentric(u, v, a, b, c)), Mathf.Epsilon);
        }

        #endregion



        #region LerpBilinear()

        [Test]
        public void LerpBilinearClampsUVCoordinatesFrom0To1()
        {
            Vector3 a = new Vector3(0f, 0f);
            Vector3 b = new Vector3(1f, 0f);
            Vector3 c = new Vector3(1f, 1f);
            Vector3 d = new Vector3(0f, 1f);

            //Checks for negative value clamps
            Assert.Less(Vector3.Distance(a, MathExtensions.LerpBilinear(-1, -1, a, b, c, d)), Mathf.Epsilon);
            Assert.Less(Vector3.Distance(a, MathExtensions.LerpBilinear(0, -1, a, b, c, d)), Mathf.Epsilon);
            Assert.Less(Vector3.Distance(a, MathExtensions.LerpBilinear(-1, 0, a, b, c, d)), Mathf.Epsilon);

            //Checks for positive value clamps
            Assert.Less(Vector3.Distance(c, MathExtensions.LerpBilinear(5, 5, a, b, c, d)), Mathf.Epsilon);
            Assert.Less(Vector3.Distance(d, MathExtensions.LerpBilinear(0, 5, a, b, c, d)), Mathf.Epsilon);
            Assert.Less(Vector3.Distance(b, MathExtensions.LerpBilinear(5, 0, a, b, c, d)), Mathf.Epsilon);
        }

        [Test]
        public void LerpBilinearReturnsProperValueRandomInput()
        {
            float u = Random.Range(0f, 1f);
            float v = Random.Range(0f, 1f - u);
            Vector3 a = new Vector3(Random.Range(-500f, 500f), Random.Range(-500f, 500f), Random.Range(-500f, 500f));
            Vector3 b = new Vector3(Random.Range(-500f, 500f), Random.Range(-500f, 500f), Random.Range(-500f, 500f));
            Vector3 c = new Vector3(Random.Range(-500f, 500f), Random.Range(-500f, 500f), Random.Range(-500f, 500f));
            Vector3 d = new Vector3(Random.Range(-500f, 500f), Random.Range(-500f, 500f), Random.Range(-500f, 500f));

            Vector4 expected = Vector4.Lerp(Vector4.Lerp(a, b, u), Vector4.Lerp(d, c, u), v);
            Assert.Less(Vector3.Distance(expected, MathExtensions.LerpBilinear(u, v, a, b, c, d)), Mathf.Epsilon);
        }

        #endregion



        #region SphericalToCartesian()

        [Test]
        public void SphericalToCartesianReturnsProperValueWhenRLessThan0()
        {
            float r = -1f;

            Assert.Less(Vector3.Distance(new Vector3(0f, 0f, -1f), MathExtensions.SphericalToCartesian(r, 0f, 0f)), 0.00001f);
            Assert.Less(Vector3.Distance(new Vector3(0f, 0f, -1f), MathExtensions.SphericalToCartesian(r, Mathf.PI, 0f)), 0.00001f);
            Assert.Less(Vector3.Distance(new Vector3(0f, 0f, 1f), MathExtensions.SphericalToCartesian(r, 0f, Mathf.PI)), 0.00001f);
            Assert.Less(Vector3.Distance(new Vector3(1f, 0f, 0f), MathExtensions.SphericalToCartesian(r, Mathf.PI, Mathf.PI / 2f)), 0.00001f);
        }

        [Test]
        public void SphericalToCartesianReturnsProperValueWhenRIs0()
        {
            float r = -0f;

            Assert.Less(Vector3.Distance(Vector3.zero, MathExtensions.SphericalToCartesian(r, 0f, 0f)), 0.00001f);
            Assert.Less(Vector3.Distance(Vector3.zero, MathExtensions.SphericalToCartesian(r, Mathf.PI, 0f)), 0.00001f);
            Assert.Less(Vector3.Distance(Vector3.zero, MathExtensions.SphericalToCartesian(r, 0f, Mathf.PI)), 0.00001f);
            Assert.Less(Vector3.Distance(Vector3.zero, MathExtensions.SphericalToCartesian(r, Mathf.PI, Mathf.PI / 2f)), 0.00001f);
        }

        [Test]
        public void SphericalToCartesianReturnsProperValueRandomInput()
        {
            float r = Random.Range(-500f, 500f);
            float theta = Random.Range(-500f, 500f) * Mathf.PI;
            float phi = Random.Range(-500f, 500f) * Mathf.PI;

            Vector3 expected = new Vector3(
                                            r * Mathf.Sin(phi) * Mathf.Cos(theta),
                                            r * Mathf.Sin(phi) * Mathf.Sin(theta),
                                            r * Mathf.Cos(phi)
                                          );
            Assert.Less(Vector3.Distance(expected, MathExtensions.SphericalToCartesian(r, theta, phi)), 0.00001f);
        }

        #endregion
    }
}