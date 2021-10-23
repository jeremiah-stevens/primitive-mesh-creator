using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PrimitiveCreator
{
    /// <summary>
    /// Provides a set of common math operations not provided in vanilla Unity.
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// Returns the two possible solutions to a polynomial function in the format Ax^2 + Bx + C = 0.
        /// </summary>
        public static float[] QuadraticFormula(float a, float b, float c)
        {
            if (a == 0) throw new System.InvalidOperationException($"a value cannot be 0 (cannot divide by 0).");

            float[] output = new float[2];

            output[0] = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / (2 * a);
            output[1] = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / (2 * a);

            return output;
        }


        /* TODO:
         * • Make up custom variants of these functions for different scale of vectors (ex: Vector2, Vector3); may increase performance?
         */
        #region Interpolations

        //Source for algorithm: https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/barycentric-coordinates
        /// <summary>
        /// Interpolates the value of the 3 vertices (A,B,C) defined by a set of uv coordinates (u -> AB, v -> AC)
        /// </summary>
        /// <param name="u">interpolation value for the vector AB</param>
        /// <param name="v">interpolation value for the vector AC</param>
        /// <param name="a">Starting point of the interpolation.</param>
        /// <param name="b">Horizontal/u-coordinate point of the interpolation.</param>
        /// <param name="c">Vertical/v-coordinate point of the interpolation.</param>
        /// <returns>interpolated value between the triangle.</returns>
        public static Vector4 LerpBarycentric(float u, float v, Vector4 a, Vector4 b, Vector4 c)
        {
            u = Mathf.Clamp(u, 0f, 1f);
            v = Mathf.Clamp(v, 0f, 1f);

            if(1f - u - v < 0f) //bounds v, so u + v = 1
            {
                v = 1 - u;
                Debug.LogWarning($"({u + v}) Barycentric coordinate values (u: {u}, v: {v}) exceed 1 (u + v <= 1). Setting v to {v}...");
            }

            /*     C
             *    /
             * v /
             *  /
             * A ------- B
             *     u->
             */
            //equivalent to A + u(B-A) + v(C-A)
            return a + u * (b - a) + v * (c - a);
            //return (1 - u - v) * a + u * b + v * c;
        }



        //Source for algorithm: https://www.omnicalculator.com/math/bilinear-interpolation
        /// <summary>
        /// Interpolates a value between the 4 vertices (A,B,C,D) defined by a set of uv coordinates (u,v)
        /// </summary>
        /// <param name="u">Horizontal interpolation value for segments AB and CD.</param>
        /// <param name="v">Vertical interpolation value between segments AB and CD).</param>
        /// <param name="a">Starting point of the region (u = 0, v = 0).</param>
        /// <param name="b">Bottom-right point of the region (u = 1, v = 0).</param>
        /// <param name="c">Top-right point of the region (u = 1, v = 1).</param>
        /// <param name="d">Top-left point of the region (u = 0, v = 1).</param>
        /// <returns>Interpolated value between the quad region.</returns>
        public static Vector4 LerpBilinear(float u, float v, Vector4 a, Vector4 b, Vector4 c, Vector4 d)
        {
            /* D - C
             * |   |
             * A - B
             */
            Vector4 ABU = Vector4.Lerp(a, b, u);
            Vector4 DCU = Vector4.Lerp(d, c, u);

            return Vector4.Lerp(ABU, DCU, v);
        }

        #endregion



        #region Coordinate Translation

        /*TODO:
         * • Add in support for converting from degrees to radians
         * • Add in a Cartesian to Spherical
         */
        /// <summary>
        /// Converts a set of spherical coordinates (r, theta, phi) to Cartesian (x, y, z)
        /// </summary>
        /// <param name="r">Radius of the sphere.</param>
        /// <param name="theta">Polar (vertical) angle (in radians) of the coordinates.</param>
        /// <param name="phi">Azimuthal (horizontal) angle (in radians) of the coordinates.</param>
        /// <returns>Cartesian coordinates as a Vector3</returns>
        public static Vector3 SphericalToCartesian(float r, float theta, float phi)
        {
            return new Vector3(
                                r * Mathf.Sin(phi) * Mathf.Cos(theta),
                                r * Mathf.Sin(phi) * Mathf.Sin(theta),
                                r * Mathf.Cos(phi)
                              );
        }

        #endregion
    }
}