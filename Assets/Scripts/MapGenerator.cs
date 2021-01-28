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
    public int randomSeed;
    private MeshGenerator meshGenerator;
    [Range(0,10)]
    public int smooths;

    public int smallestRegion;
    private int[,] map;

    private void Start()
    {
        if (randomSeed != 0)
        {
            Random.seed = randomSeed;
        }
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
        for (int i = 0; i < smooths; i++)
        {
                 SmoothMap();

        }

                RemoveSmallCavesAndCavities(smallestRegion);

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
        //tODO Update map width and height
        meshGenerator.GenerateMesh(map, 1);
    }

    private bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
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

void RemoveSmallCavesAndCavities(int wallThreshold)
{
    RemoveSmallRegions(wallThreshold,1);
    RemoveSmallRegions(wallThreshold,0);
}
[ContextMenu(nameof(RemoveSmallRegions))]
private void RemoveSmallRegions(int wallThreshold, int tileType)
{
    List<List<Coord>> wallRegions = GetRegions(tileType);
    int otherTileType = tileType == 1 ?0:1;
    foreach (List<Coord> wallRegion in wallRegions)
    {
        if (wallRegion.Count < wallThreshold)
        {
            foreach (Coord coord in wallRegion)
            {
                map[coord.tileX, coord.tileY] = otherTileType;
            }
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
                wallCount += isBorder(x, y) ? 1 : map[neighborX,neighborY];
            }  
        }
        return wallCount;
    }


    public bool isBorder(int x, int y)
    {
        return (x == 0 || y == 0 || x == width - 1 || y == height - 1);
    }
    
       private List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tilesInRegion = new List<Coord>();
        int[,] visitied = new int[width, height];
        int tileType = map[startX, startY];

        Queue < Coord > queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX,startY));
        visitied[startX, startY] = 1;
        while (queue.Count > 0)
        {
            Coord current = queue.Dequeue();
            tilesInRegion.Add(current);

            for (int x = current.tileX - 1; x <= current.tileX + 1; x++)
            {
                for (int y = current.tileY - 1; y <= current.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == current.tileY || x == current.tileX))
                    {
                        if (visitied[x, y] == 0 && map[x, y] == tileType)
                        {
                            visitied[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tilesInRegion;
    }

       private List<List<Coord>> GetRegions(int tileType)
       {
           List<List<Coord>> regions = new List<List<Coord>>();
           int[,] visited = new int[width, height];

           for (int x = 0; x < width; x++)
           {
               for (int y = 0; y < height; y++)
               {
                   if (visited[x, y] == 0 && map[x, y] == tileType)
                   {
                       List<Coord> newRegion = GetRegionTiles(x, y);
                       regions.Add(newRegion);

                       foreach (Coord coord in newRegion)
                       {
                           visited[coord.tileX, coord.tileY] = 1;
                       }
                   }
               }
           }

           return regions;
       }
 
}

   public class Coord
   {
       public int tileX, tileY;

       public Coord(int tileX, int tileY)
       {
           this.tileX = tileX;
           this.tileY = tileY;
       }
   }
