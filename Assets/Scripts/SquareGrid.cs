using UnityEngine;

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