using System;
using UnityEngine;

[RequireComponent(typeof(MeshGenerator))]
public class DrawSuperGridGizmo : MonoBehaviour
{
    private MeshGenerator meshGenerator;


    private void OnDrawGizmos()
    {
        meshGenerator = GetComponent<MeshGenerator>();
        if (meshGenerator.squareGrid != null)
        {
            for (int x = 0; x < meshGenerator.squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < meshGenerator.squareGrid.squares.GetLength(1); y++)
                {
    
                    Gizmos.color = (meshGenerator.squareGrid.squares[x,y].topLeft.active)?Color.black :Color.white;
                    Gizmos.DrawCube(meshGenerator.squareGrid.squares[x,y].topLeft.position, Vector3.one * 0.4f);
                    
                    Gizmos.color = (meshGenerator.squareGrid.squares[x,y].topRight.active)?Color.black :Color.white;
                    Gizmos.DrawCube(meshGenerator.squareGrid.squares[x,y].topRight.position, Vector3.one * 0.4f);
    
                    Gizmos.color = (meshGenerator.squareGrid.squares[x,y].bottomLeft.active)?Color.black :Color.white;
                    Gizmos.DrawCube(meshGenerator.squareGrid.squares[x,y].bottomLeft.position, Vector3.one * 0.4f);
                    
                    
                    Gizmos.color = (meshGenerator.squareGrid.squares[x,y].bottomRight.active)?Color.black :Color.white;
                    Gizmos.DrawCube(meshGenerator.squareGrid.squares[x,y].bottomRight.position, Vector3.one * 0.4f);
                    
                    Gizmos.color = Color.gray;
                    // Gizmos.DrawCube(meshGenerator.squareGrid.squares[x,y].centerLeft.position, Vector3.one * .15f);
                    // Gizmos.DrawCube(meshGenerator.squareGrid.squares[x,y].centreRight.position, Vector3.one * .15f);
                    // Gizmos.DrawCube(meshGenerator.squareGrid.squares[x,y].topMiddle.position, Vector3.one * .15f);
                    // Gizmos.DrawCube(meshGenerator.squareGrid.squares[x,y].bottomMiddle.position, Vector3.one * .15f);
    
                }
                
            }
        }
    }
}