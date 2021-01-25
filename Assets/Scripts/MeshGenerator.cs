using System;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    private SquareGrid squareGrid;
    public void GenerateMesh(int[,] map, float squareSize)
    {
        squareGrid = new SquareGrid(map, squareSize);
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
                    Gizmos.DrawCube(squareGrid.squares[x,y].centerRight.position, Vector3.one * .15f);
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
        public Node topMiddle, bottomMiddle, centerLeft, centerRight;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomLeft, ControlNode bottomRight)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
            this.topMiddle = topLeft.right;
            this.bottomMiddle = bottomLeft.right;
            this.centerLeft = bottomLeft.above;
            this.centerRight = bottomRight.above;
        }
    }
    public class Node
    {
        public Vector3 position;
        public int vertex = -1;

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
