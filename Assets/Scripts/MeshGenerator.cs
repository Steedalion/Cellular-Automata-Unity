using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MeshGenerator : MonoBehaviour
{
        private List<Vector3> vertices;
        private List<int> triangels;
        private Mesh mesh;
        private SquareGrid squareGrid;
        private Dictionary<int, List<Triangle>> TriangelsUsingVertex =  new Dictionary<int, List<Triangle>>();

        private List<List<int>> outlines = new List<List<int>>();
        private HashSet<int> checkedOutlines = new HashSet<int>();
        [SerializeField] private MeshFilter walls;

        private void Awake()
        {
            vertices = new List<Vector3>();
        triangels = new List<int>();
        mesh  = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // mesh = GetComponent<MeshFilter>().mesh;
        // mesh = new Mesh();
        }


    public void GenerateMesh(int[,] map, float squareSize)
    {
        outlines.Clear();
        checkedOutlines.Clear();
        squareGrid = new SquareGrid(map, squareSize);

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x,y]);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangels.ToArray();
        mesh.RecalculateNormals();

        CreateWallMesh();
    }

    private void CreateWallMesh()
    {
        CalculateMeshOutlines();
        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 5;
        foreach (List<int> outline in outlines)
        {
            for (int i = 0; i < outline.Count-1; i++)
            {
                int startIndex = wallVertices.Count;
                wallVertices.Add(vertices[outline[i]]); //left
                wallVertices.Add(vertices[outline[i+1]]);
                wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); //bottom Left
                wallVertices.Add(vertices[outline[i+1]] - Vector3.up * wallHeight);
                
                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);
                
                
                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);
            }
        }

        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        walls.mesh = wallMesh;
    }

    void TriangulateSquare(Square square)
    {
        switch (square.configuration)
        {
           // 1 points:
        case 1:
            MeshFromPoints(square.centerLeft, square.bottomMiddle, square.bottomLeft);
            break;
        case 2:
            MeshFromPoints(square.bottomRight, square.bottomMiddle, square.centreRight);
            break;
        case 4:
            MeshFromPoints(square.topRight, square.centreRight, square.topMiddle);
            break;
        case 8:
            MeshFromPoints(square.topLeft, square.topMiddle, square.centerLeft);
            break;

        // 2 points:
        case 3:
            MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centerLeft);
            break;
        case 6:
            MeshFromPoints(square.topMiddle, square.topRight, square.bottomRight, square.bottomMiddle);
            break;
        case 9:
            MeshFromPoints(square.topLeft, square.topMiddle, square.bottomMiddle, square.bottomLeft);
            break;
        case 12:
            MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centerLeft);
            break;
        case 5:
            MeshFromPoints(square.topMiddle, square.topRight, square.centreRight, square.bottomMiddle, square.bottomLeft, square.centerLeft);
            break;
        case 10:
            MeshFromPoints(square.topLeft, square.topMiddle, square.centreRight, square.bottomRight, square.bottomMiddle, square.centerLeft);
            break;

        // 3 point:
        case 7:
            MeshFromPoints(square.topMiddle, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
            break;
        case 11:
            MeshFromPoints(square.topLeft, square.topMiddle, square.centreRight, square.bottomRight, square.bottomLeft);
            break;
        case 13:
            MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.bottomMiddle, square.bottomLeft);
            break;
        case 14:
            MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomMiddle, square.centerLeft);
            break;

        // 4 point:
        case 15:
            MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
            checkedOutlines.Add(square.topLeft.vertexIndex);
            checkedOutlines.Add(square.topRight.vertexIndex);
            checkedOutlines.Add(square.bottomRight.vertexIndex);
            checkedOutlines.Add(square.bottomLeft.vertexIndex);
            break;
        }

    }

    void MeshFromPoints(params Node[] nodes)
    {
        AssignVertices(nodes);
        if (nodes.Length >= 3)
            CreateTriangels(nodes[0], nodes[1], nodes[2]);
        if (nodes.Length >= 4)
        CreateTriangels(nodes[0], nodes[2], nodes[3]);
        if (nodes.Length >= 5)
        CreateTriangels(nodes[0], nodes[3], nodes[4]);
        if (nodes.Length >= 6)
        CreateTriangels(nodes[0], nodes[4], nodes[5]);
    }

    void CreateTriangels(Node a, Node b, Node c)
    {
        triangels.Add(a.vertexIndex);
        triangels.Add(b.vertexIndex);
        triangels.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(a.vertexIndex, triangle);
        AddTriangleToDictionary(b.vertexIndex, triangle);
        AddTriangleToDictionary(c.vertexIndex, triangle);
    }

    /// <summary>
    /// Gets the other vertex of an outline edge
    /// </summary>
    /// <param name="vertexIndex"></param>
    /// <returns>the other vertex, -1 if none exists</returns>
    int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = TriangelsUsingVertex[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];
            for (int j = 0; j < 3; j++)
            {
                int VertexB = triangle[j];
                if(VertexB == vertexIndex || checkedOutlines.Contains(vertexIndex)) continue;

                if (isOutlineEdge(vertexIndex, VertexB)) return VertexB;

            }
        }

        return -1;
    }

    void CalculateMeshOutlines()
    {
        for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
        {
            if(checkedOutlines.Contains(vertexIndex)) continue;
            int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
            if (newOutlineVertex == -1) continue;
            checkedOutlines.Add(newOutlineVertex);

            List<int> newOutline = new List<int>();
            newOutline.Add(vertexIndex);
            outlines.Add(newOutline);
            FollowOutLine(newOutlineVertex, outlines.Count - 1);
            
            outlines[outlines.Count - 1].Add(vertexIndex); // why not add directly to newOutline?
        }
    }

    private void FollowOutLine(int vertexIndex, int outlineIndex)
    {
        outlines[outlineIndex].Add(vertexIndex);
        checkedOutlines.Add(vertexIndex);

        int nextVertex = GetConnectedOutlineVertex(vertexIndex);
        if (nextVertex != -1)
        {
            FollowOutLine(nextVertex, outlineIndex);
        }
    }

    bool isOutlineEdge(int vertexA, int vertexB)
    {
        List<Triangle> trianglesContainingA = TriangelsUsingVertex[vertexA];
        int sharedTriangles = 0;
        for (int i = 0; i < trianglesContainingA.Count; i++)
        {
            if (trianglesContainingA[i].Contains(vertexB))
            {
                sharedTriangles++;
                if (sharedTriangles > 1)  return false;
            }
        }
        return true;

    }

    void AddTriangleToDictionary(int vertexIndex, Triangle triangle)
    {
        if (TriangelsUsingVertex.TryGetValue(vertexIndex, out List<Triangle> existingTriangels))
        {
            existingTriangels.Add(triangle);
        }
        else
        {
            List<Triangle> newTriangles = new List<Triangle>();
            newTriangles.Add(triangle);
            TriangelsUsingVertex.Add(vertexIndex, newTriangles);
        }
    }


    private void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                // Debug.Log(vertices);
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }
    
    
    private void OnDrawGizmos()
    {
        if (squareGrid != null)
        {
            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                {
    
                     Gizmos.color = (squareGrid.squares[x,y].topLeft.active)?Color.black :Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].topLeft.position, Vector3.one * 0.4f);
                    
                    Gizmos.color = (squareGrid.squares[x,y].topRight.active)?Color.black :Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].topRight.position, Vector3.one * 0.4f);
    
                    Gizmos.color = (squareGrid.squares[x,y].bottomLeft.active)?Color.black :Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].bottomLeft.position, Vector3.one * 0.4f);
                    
                    
                    Gizmos.color = (squareGrid.squares[x,y].bottomRight.active)?Color.black :Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].bottomRight.position, Vector3.one * 0.4f);
                    
                    Gizmos.color = Color.gray;
                    Gizmos.DrawCube(squareGrid.squares[x,y].centerLeft.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x,y].centreRight.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x,y].topMiddle.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x,y].bottomMiddle.position, Vector3.one * .15f);
    
                }
                
            }
        }
    }


}