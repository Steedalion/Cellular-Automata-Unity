using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public int width, height;

    [Range(0,10)]
    public int borderSize = 5;
    [Range(0,100)]
    public int fillPercent;

    private MeshGenerator meshGenerator;
    
    [Range(0,10)]
    public int smooths;

    private int[,] map;

    private void Start()
    {
        meshGenerator = GetComponent<MeshGenerator>();
        GenerateRandomMap();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateRandomMap();
        }
    }

    void GenerateRandomMap()
    {
        FillRandomMap();
        SmoothMap();
        
        int[,] borderedMap = new int[width + 2 * borderSize, height + 2 * borderSize];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                {
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                }
                else
                {
                    borderedMap[x, y] = 1;
                }
            }
        }

        map = borderedMap;
        meshGenerator.GenerateMesh(map, 1);
    }
    
    [ContextMenu("Generate")]
    public void FillRandomMap()
    {
        map = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
                map[i, j] = Random.Range(0, 100) < fillPercent ? 1:0;
        }
    }

    private void OnDrawGizmos()
    {
        // if (map == null) return;
        // for (int i = 0; i < width; i++)
        // {
        //     for (int j = 0; j < height; j++)
        //     {
        //         Gizmos.color = map[i, j] == 1 ? Color.black : Color.white;
        //         Vector3 pos = new Vector3(-width * 0.5f + i + 0.5f,  -height * 0.5f + j + 0.5f, 0);
        //         Gizmos.DrawCube(pos, Vector3.one);
        //     }
        // }
    }

[ContextMenu(nameof(SmoothMap))]
private void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int surroundingWalls = GetSurroundingWallTiles(x,y);
                // Debug.Log(surroundingWalls);
                if (surroundingWalls > 4)
                {
                    map[x, y] = 1;
                } else if (surroundingWalls < 4)
                {
                    map[x, y] = 0;
                }
                    
                // map[x, y] = (surroundingWalls > 4) ? 1 : 0;
            }
        }
    }

    private int GetSurroundingWallTiles(int x, int y)
    {
        int wallCount = 0;
        for (int neighborX = x-1; neighborX <= x+1; neighborX++)
        {
            for (int neighborY = y-1; neighborY <= y+1; neighborY++)
            {
                if (neighborX == x && neighborY == y) continue; //self
                wallCount += isWall(x, y) ? 1 : map[neighborX,neighborY];
            }  
        }
        return wallCount;
    }


    public bool isWall(int x, int y)
    {
        return (x == 0 || y == 0 || x == width - 1 || y == height - 1);
    }
}
