using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PrimitiveCreator.PrimitiveCreatorUtility;


namespace PrimitiveCreator
{
    //public class MeshCreatorSubdividedTriangle : MeshCreator
    //{
    //    public override PrimitiveCreatorUtility.MeshType MeshType => throw new System.NotImplementedException();

    //    public override Mesh CreateMesh()
    //    {
    //        return CreateMesh(new MeshDetails());
    //    }

    //    public override Mesh CreateMesh(MeshDetails meshDetails)
    //    {
    //        Mesh mesh = new Mesh();

    //        int vertexCount = GetClosestViableVertexCount(meshDetails.vertexCount);

    //        CreateVertices(ref mesh, vertexCount);

    //        return mesh;
    //    }

    //    private void CreateVertices(ref Mesh mesh, int vertexCount)
    //    {
    //        mesh = SubdivideTriangleBase(new Vector3(0.5f, 1f), new Vector3(1f, 0f), new Vector3(0f, 0f));
    //        return;

    //        ///* 1. Create initial triangle
    //        // * 2. Figure out how many subdivisions are necessary (solve GetClosestViableVertexCount() function for sideWidth)
    //        // * 3. Recursively make subdivisions
    //        // * 4. Collect all at end and return
    //        // */
    //        //int vCount = GetClosestViableVertexCount(vertexCount);

    //        //Vector3[] vertices = new Vector3[vCount];
    //        //int[] triangles = new int[GetTriCount(vCount) * 3];

    //        ////Create the initial triangle
    //        //vertices[0] = new Vector3(0.5f, 1f);
    //        //vertices[1] = new Vector3(1f, 0f);
    //        //vertices[2] = new Vector3(0f, 0f);
    //        //SetTri(triangles, 0, 0, 1, 2);
    //        //mesh.vertices = vertices;
    //        //mesh.triangles = triangles;

    //        ////calculate subdivisions based on vertex count
    //        //int subdivisions = 1; //TODO: calculate these dynamically
    //    }

    //    private Mesh SubdivideTriangleBase(Vector3 v0, Vector3 v1, Vector3 v2)
    //    {
    //        Mesh mesh = new Mesh();

    //        int vCount = 6;
    //        int tCount = 4;

    //        Vector3[] vertices = new Vector3[vCount];
    //        int[] triangles = new int[tCount * 3];
    //        Vector2[] uvs = new Vector2[vertices.Length];
    //        Vector4[] tangents = new Vector4[vertices.Length];

    //        //set the vertices
    //        vertices[0] = v0;
    //        vertices[1] = v1;
    //        vertices[2] = v2;
    //        //vertices[3] = Vector3.Lerp(v0, v1, 0.5f);
    //        //vertices[4] = Vector3.Lerp(v1, v2, 0.5f);
    //        //vertices[5] = Vector3.Lerp(v2, v0, 0.5f);

    //        SetVertex(ref vertices, ref uvs, ref tangents, 0, 0, 0);
    //        SetVertex(ref vertices, ref uvs, ref tangents, 1, 1, 1);
    //        SetVertex(ref vertices, ref uvs, ref tangents, 2, 2, 2);
    //        SetVertex(ref vertices, ref uvs, ref tangents, 3, 0, 1);
    //        SetVertex(ref vertices, ref uvs, ref tangents, 4, 1, 2);
    //        SetVertex(ref vertices, ref uvs, ref tangents, 5, 2, 0);

    //        //set the triangles
    //        int t = 0;
    //        t = SetTri(triangles, t, 0, 3, 5);
    //        t = SetTri(triangles, t, 1, 4, 3);
    //        t = SetTri(triangles, t, 2, 5, 4);
    //        t = SetTri(triangles, t, 3, 4, 5);

    //        mesh.vertices = vertices;
    //        mesh.triangles = triangles;
    //        mesh.uv = uvs;
    //        mesh.tangents = tangents;

    //        mesh.RecalculateNormals();

    //        return mesh;
    //    }

    //    private void SetVertex(ref Vector3[] vertices, ref Vector2[] uvs, ref Vector4[] tangents, int i, int a, int b)
    //    {
    //        vertices[i] = Vector3.Lerp(vertices[a], vertices[b], 0.5f);
    //        uvs[i] = Vector2.Lerp(vertices[a], vertices[b], 0.5f);
    //        tangents[i] = new Vector4(1f, 0f, 0f, -1f);
    //    }



    //    public override int GetClosestViableVertexCount(int proposedVertexCount)
    //    {
    //        int minimumVertexCount = 3;
    //        if (proposedVertexCount < minimumVertexCount) return minimumVertexCount;

    //        int sideWidth = GetSideSteps(proposedVertexCount);

    //        return Mathf.FloorToInt(0.5f * Mathf.Pow(sideWidth, 2) + 1.5f * sideWidth + 1);
    //    }

    //    private int GetSideSteps(int vertexCount)
    //    {
    //        /* Triangle can be viewed as a half of a full quad, provided we double the inside edge (|sideWidth|)
    //         * ([quad vertex count] + [sideWidth]) / 2 <= proposedVertexCount
    //         * ([(x + 1) * (x + 1)] + (x + 1)) / 2 <= proposedVertexCount
    //         * (x^2 + 2x + 1 + x + 1) / 2 <= proposedVertexCount
    //         * (x^2 + 3x + 2) / 2 <= proposedVertexCount
    //         * 0.5x^2 + 1.5x + 1 - proposedVertexCount <= 0
    //         */
    //        float rawSideWidth = Mathf.Max(MathExtensions.QuadraticFormula(0.5f, 1.5f, 1 - vertexCount));
    //        int sideWidth = Mathf.FloorToInt(rawSideWidth);

    //        return sideWidth;
    //    }

    //    private int GetTriCount(int vertexCount)
    //    {
    //        int sideSteps = GetSideSteps(vertexCount);

    //        return sideSteps * sideSteps;
    //    }
    //}
}