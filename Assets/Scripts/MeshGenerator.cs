﻿using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private Mesh mapMesh;
        public SquareGrid squareGrid;
        public bool is2D;
        public MeshFilter cave;

        [Tooltip("The amount of UV's repeated over the map")]
        public int uvTileAmount = 3;
        private Dictionary<int, List<Triangle>> trianglesUsingVertex =  new Dictionary<int, List<Triangle>>();

        private readonly List<List<int>> outlines = new List<List<int>>();
        private readonly HashSet<int> checkedOutlines = new HashSet<int>();
        [SerializeField] private MeshFilter walls;
        private MeshFilter meshFilter;
        



        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        public void GenerateMesh(int[,] map, float squareSize)
    {
        outlines.Clear();
        checkedOutlines.Clear();
        trianglesUsingVertex.Clear();
        vertices.Clear();
        triangles.Clear();
        squareGrid = new SquareGrid(map, squareSize);

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x,y]);
            }
        }

        mapMesh  = new Mesh();
        cave.mesh = mapMesh;
        
        mapMesh.vertices = vertices.ToArray();
        mapMesh.triangles = triangles.ToArray();
        mapMesh.RecalculateNormals();
        // create texture
        mapMesh.uv = CreateUvs(map, squareSize, uvTileAmount);

        if (is2D)
        {
            Generate2DColliders();
        }
        else
        {
            CreateWallMesh();
        }
    }

        private Vector2[] CreateUvs(int[,] map, float squareSize, int tileAmount)
        {
            Vector2[] uvs = new Vector2[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                float widthOfUnit;
                widthOfUnit = map.GetLength(map.GetLength(0) > map.GetLength(1) ? 0 : 1);
                widthOfUnit *= 0.5f * squareSize;
                float percentX = Mathf.InverseLerp(-widthOfUnit, widthOfUnit, vertices[i].x) * tileAmount;
                float percentY = Mathf.InverseLerp(-widthOfUnit, widthOfUnit, vertices[i].z) * tileAmount;
                // percentX *= tileAmount; percentY*=tileAmount;
                uvs[i] = new Vector2(percentX, percentY);
                
            }

            return uvs;
        }

        private void Generate2DColliders()
        {
            EdgeCollider2D[] oldColliders = gameObject.GetComponents<EdgeCollider2D>();
            foreach (EdgeCollider2D collider in oldColliders)
            {
                Destroy(collider);
            }
            
            CalculateMeshOutlines();
            
            foreach (List<int> outline in outlines)
            {
                EdgeCollider2D edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
                Vector2[] edgePoints = new Vector2[outline.Count];

                for (int i = 0; i < outline.Count; i++)
                {
                    float x = vertices[outline[i]].x;
                    float z = vertices[outline[i]].z;
                    edgePoints[i] = new Vector2(x,z);
                }

                edgeCollider2D.points = edgePoints;
            }
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
                
                //First triangle
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

        Destroy(walls.gameObject.GetComponent<MeshCollider>());
        MeshCollider wallCollider = walls.gameObject.AddComponent<MeshCollider>();
    }

    void TriangulateSquare(Square square)
    {
        switch (square.configuration) {
        case 0:
            break;

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
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

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
    private int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = trianglesUsingVertex[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];
            for (int j = 0; j < 3; j++)
            {
                int VertexB = triangle[j];
                if(VertexB == vertexIndex || checkedOutlines.Contains(VertexB)) continue;

                if (isOutlineEdge(vertexIndex, VertexB)) return VertexB;

            }
        }

        return -1;
    }

    void CalculateMeshOutlines()
    {
        for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
        {
            if (!checkedOutlines.Contains(vertexIndex))
            {
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if (newOutlineVertex != -1)
                {
                    checkedOutlines.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);
                    FollowOutLine(newOutlineVertex, outlines.Count - 1);

                    outlines[outlines.Count - 1].Add(vertexIndex); // why not add directly to newOutline?
                }
            }
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
        List<Triangle> trianglesContainingA = trianglesUsingVertex[vertexA];
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
        if (trianglesUsingVertex.TryGetValue(vertexIndex, out List<Triangle> existingTriangles))
        {
            existingTriangles.Add(triangle);
        }
        else
        {
            List<Triangle> newTriangles = new List<Triangle>();
            newTriangles.Add(triangle);
            trianglesUsingVertex.Add(vertexIndex, newTriangles);
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
}