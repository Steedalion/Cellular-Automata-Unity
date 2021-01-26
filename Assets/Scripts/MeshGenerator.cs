using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
        private List<Vector3> vertices;
        private List<int> triangels;
        private Mesh mesh;
        private SquareGrid squareGrid;

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


    #region Supporting Classes
    public class SquareGrid
    {
        
        private float squareSize;
        public Square[,] squares;

        
        
        public SquareGrid(int[,] map, float squareSize)
        {
            this.squareSize = squareSize;
            int xnodes = map.GetLength(0);
            int ynodes = map.GetLength(1);
            float mapWidth = xnodes * squareSize;
            float mapHeight = ynodes    *    squareSize;

            ControlNode[,] controlNodes = new ControlNode[xnodes, ynodes];
            for (int x = 0; x < xnodes; x++)
            {
                for (int y = 0; y < ynodes; y++)
                {
                    Vector3 pos = new Vector3(
                        -mapWidth * 0.5f + x * squareSize + squareSize * 0.5f, 
                        0,
                        -mapHeight * 0.5f + y * squareSize + squareSize* 0.5f);
                
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, this.squareSize);
                }
            }
            
            squares = new Square[xnodes - 1, ynodes - 1];
            
            for (int x = 0; x < xnodes - 1; x++)
            {
                for (int y = 0; y < ynodes - 1; y++)
                {
                    squares[x, y] = new Square(
                        controlNodes[x, y + 1],
                        controlNodes[x + 1, y + 1],
                        controlNodes[x, y],
                        controlNodes[x + 1, y]);
                }
            }
        }
    }
    public class Square
    {
        public ControlNode topLeft, topRight, bottomLeft , bottomRight;
        public Node topMiddle, bottomMiddle, centerLeft, centreRight;
        public int configuration;
        // int depicting active nodes; 
        /// <summary>
        /// 1   2
        ///
        /// 4   3 = 1234
        ///
        /// e.g.
        /// 0   1
        ///
        /// 1   0 = 0101 = 1+2^2 = 5
        /// 
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="topRight"></param>
        /// <param name="bottomLeft"></param>
        /// <param name="bottomRight"></param>
        

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomLeft, ControlNode bottomRight)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
            this.topMiddle = topLeft.right;
            this.bottomMiddle = bottomLeft.right;
            this.centerLeft = bottomLeft.above;
            this.centreRight = bottomRight.above;

            configuration += (topLeft.active)?8:0;
            configuration += (this.topRight.active)?4:0;
            configuration += (this.bottomRight.active)?2:0;
            configuration += (this.bottomLeft.active)?1:0;
            
        }
    }
    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 position)
        {
            this.position = position;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node right, above;

        public ControlNode(Vector3 position, bool active, float squareSize) : base(position)
        {
            this.active = active;
            above = new Node(position + Vector3.forward * squareSize * 0.5f);
            right = new Node(position + Vector3.right * squareSize * 0.5f);
        }
    }
        #endregion

}
