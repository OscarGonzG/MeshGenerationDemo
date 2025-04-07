using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public struct GPUBuffer
{
    Vector3Int vector;
}

public static class OctahedronSphere
{
    public static ComputeShader computeShader;

    private static readonly Vector3[] octahedronVerts =
    {
        new Vector3 (0, 1, 0),
        new Vector3 (0, 0, 1),
        new Vector3 (1, 0, 0),
        new Vector3 (0, 0, -1),
        new Vector3 (-1, 0, 0),
        new Vector3 (0, -1, 0),
    };

    private static readonly int[] octahedronTris =
    {
        0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 1,
        4, 3, 5, 3, 2, 5, 2, 1, 5, 1, 4, 5
    };


    /// <summary>
    /// Creates a sphere approximation from a subdivided octahedron.
    /// </summary>
    /// <param name="subdivisions">number of extra vertices to be created in the edges of the octahedron</param>
    /// <param name="radius">the radius of the sphere</param>
    /// <returns></returns>
    public static Mesh GenerateSphere(int subdivisions, float radius)
    {
        if (subdivisions < 0)
        {
            throw new ArgumentException();
        }
        Mesh mesh = new Mesh();
        if (subdivisions == 0)
        {
            mesh.vertices = octahedronVerts;
            mesh.triangles = octahedronTris;
            return mesh;
        }


        LinkedList<Vector3> vertList = new LinkedList<Vector3>();
        for (int i = 0; i < octahedronVerts.Length - 1; i++)    // Adds all vertices except the last one
        {
            Vector3 point = octahedronVerts[i];
            ClampToSphere(ref point, radius);
            vertList.AddLast(point);
        }
        


        #region Top half of the octahedron

        LinkedListNode<Vector3> node = vertList.First;

        
        // Represents the distance between vertices in the edge
        Vector3 verticalSubdivVector = (octahedronVerts[1] - octahedronVerts[0]) / (subdivisions + 1);
        Vector3 horizontalSubdivVector = (octahedronVerts[2] - octahedronVerts[1]) / (subdivisions + 1);
        Vector3 upperVertex = octahedronVerts[0];
        
        for (int i = 1; i <= subdivisions + 1; i++)
        {
            for (int face = 0; face < 4; face++)
            {
                Vector3 point;
                if (i == subdivisions + 1)
                {
                    node = node.Next;
                }
                if (i != subdivisions + 1)
                {
                    point = upperVertex + verticalSubdivVector * i;
                    ClampToSphere(ref point, radius);
                    vertList.AddAfter(node, point);
                    node = node.Next;
                }
                for (int j = 1; j < i; j++)
                {
                    point = upperVertex + (i * verticalSubdivVector) +
                        (j * horizontalSubdivVector);
                    ClampToSphere(ref point, radius);
                    vertList.AddAfter(node, point);
                    node = node.Next;
                }
                verticalSubdivVector = Quaternion.AngleAxis(90, Vector3.up) * verticalSubdivVector;
                horizontalSubdivVector = Quaternion.AngleAxis(90, Vector3.up) * horizontalSubdivVector;
            }
            
        }
        
        List<int> triList = new List<int>();
        triList.Add(0);
        triList.Add(4);
        triList.Add(1);
        triList.Add(0);
        triList.Add(1);
        triList.Add(2);
        triList.Add(0);
        triList.Add(2);
        triList.Add(3);
        triList.Add(0);
        triList.Add(3);
        triList.Add(4);


        int previousVertices = 1;
        // Generates row by row
        for (int row = 1; row <= subdivisions; row++) 
        {
            int verticesInRow = 4 * row;
            int face = 0; // Keeps count of the face the tris are being generated in
            // Uses the local indexes of vertices
            for (int i = 0; i < verticesInRow; i++)
            {
                if (i == verticesInRow / 4 || i == verticesInRow / 2 || i == (verticesInRow*3)/4)
                {
                    face++;
                }
                // Upside down triangles
                triList.Add(previousVertices + i);
                triList.Add(previousVertices + verticesInRow + i + face + 1);
                triList.Add(previousVertices + (i + 1) % verticesInRow);
                // Right side up triangles (except the last one in each side)
                triList.Add(previousVertices + i);
                triList.Add(previousVertices + verticesInRow + i + face);
                triList.Add(previousVertices + verticesInRow + i + face + 1);
            }

            for (int faceIndex = 0; faceIndex < 4; faceIndex++)
            {
                int vertexIndex = (faceIndex + 1) * verticesInRow / 4;
                int verticesInNextRow = 4 * (row + 1);
                // Right side up triangles (last one in each side)
                triList.Add(previousVertices + (vertexIndex % verticesInRow));
                triList.Add(previousVertices + verticesInRow + (vertexIndex + faceIndex) % verticesInNextRow);
                triList.Add(previousVertices + verticesInRow + (vertexIndex + faceIndex + 1) % verticesInNextRow);
            }

            previousVertices += verticesInRow;
        }

        #endregion

        #region Bottom half
        int mirroredVertices = vertList.Count - 4 * (subdivisions + 1);
        node = vertList.First;
        // Mirrors every vertex except the last row along the y axis
        for (int i = 0; i < mirroredVertices; i++)
        {
            Vector3 vert = node.Value;
            vert.y = -vert.y;
            vertList.AddLast(vert);
            node = node.Next;
        }

        // Adds a reversed version of the triList at the end of the list adding the length of the
        // initial list to all indexes except those of the vertices of the last row
        int fistVertexIndexLastRow = previousVertices;
        for (int i = triList.Count - 1; i >= 0; i--)
        {
            int index = triList.ElementAt(i);
            if (index < fistVertexIndexLastRow)
            {
                index += mirroredVertices + 4 * (subdivisions + 1);
            }
            triList.Add(index);
        }
        #endregion

        mesh.vertices = vertList.ToArray<Vector3>();
        mesh.triangles = triList.ToArray<int>();

        return mesh;
    }

    private static void ClampToSphere(ref Vector3 point, float sphereRadius)
    {
        point *= sphereRadius / point.magnitude;
    }
    
}