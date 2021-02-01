using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public int width, height;


    [Range(0, 10)] public int borderSize = 5;
    [Range(0, 100)] public int fillPercent;
    public int randomSeed;
    private MeshGenerator meshGenerator;
    [Range(0, 10)] public int smooths;

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

        // int[,] borderedMap = new int[width + 2 * borderSize, height + 2 * borderSize];
        //
        // for (int x = 0; x < borderedMap.GetLength(0); x++)
        // {
        //     for (int y = 0; y < borderedMap.GetLength(1); y++)
        //     {
        //         if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
        //         {
        //             borderedMap[x, y] = map[x - borderSize, y - borderSize];
        //         }
        //         else
        //         {
        //             borderedMap[x, y] = MAP.wall;
        //         }
        //     }
        // }
        // map = borderedMap;
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
                map[i, j] = Random.Range(0, 100) < fillPercent ? MAP.wall : MAP.empty;
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
                int surroundingWalls = GetSurroundingWallTiles(x, y);
                // Debug.Log(surroundingWalls);
                if (surroundingWalls > 4)
                {
                    map[x, y] = MAP.wall;
                }
                else if (surroundingWalls < 4)
                {
                    map[x, y] = MAP.empty;
                }

                // map[x, y] = (surroundingWalls > 4) ? 1 : 0;
            }
        }
    }

    private void RemoveSmallCavesAndCavities(int wallThreshold)
    {
        RemoveSmallRegions(wallThreshold, MAP.wall);
        List<Room> rooms = RemoveSmallRegions(wallThreshold, MAP.empty);
        rooms.Sort();
        rooms[0].isMainRoom = true;
        foreach (Room room in rooms)
        {
            Debug.Log(room.roomSize);
        }
        ConnectClosestRooms(rooms);
    }


    [ContextMenu(nameof(RemoveSmallRegions))]
    private List<Room> RemoveSmallRegions(int wallThreshold, int tileType)
    {
        List<Room> survivingRooms = new List<Room>();
        List<List<Coord>> regions = GetRegions(tileType);
        int otherTileType = tileType == MAP.wall ? MAP.empty : MAP.wall;
        foreach (List<Coord> region in regions)
        {
            if (region.Count < wallThreshold)
            {
                foreach (Coord coord in region)
                {
                    map[coord.tileX, coord.tileY] = otherTileType;
                }
            }
            else if (tileType == MAP.empty)
            {
                survivingRooms.Add(new Room(region, map));
            }
        }

        return survivingRooms;
    }

    void ConnectClosestRooms(List<Room> all, bool forceConnectedToMain = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();
        if (forceConnectedToMain)
        {
            foreach (Room room in all)
            {
                if (room.isConnectedToMain)
                {
                    roomListA.Add(room);
                }
                else
                {
                    roomListB.Add(room);
                }
            }
        }
        else
        {
            roomListA = all;
            roomListB = all;
        }
        
        Coord bestTileA = null, bestTileB = null;
        Room bestRoomA = null, bestRoomB = null;
        float bestDistance = Mathf.Infinity;
        foreach (Room roomA in roomListA)
        {
            if (!forceConnectedToMain)
            {
                            bestDistance = Mathf.Infinity;
                            if (roomA.connectedRooms.Count > 0)
                            {
                                continue;
                            }
                
            }

            foreach (Room roomB in roomListB.Where(roomB => roomA!=roomB).Where(roomB => !roomA.IsConnected(roomB)))
            {
              

                // if (roomA.IsConnected(roomB))
                // {
                //     bestDistance = Mathf.Infinity;
                //     break;
                // }

                foreach (Coord edgeTileA in roomA.edgeTiles)
                {
                    foreach (Coord edgeTileB in roomB.edgeTiles)
                    {
                        float dist = Mathf.Pow(edgeTileA.tileX - edgeTileB.tileX, 2) +
                                     Mathf.Pow(edgeTileA.tileY - edgeTileB.tileY, 2);
                        if (dist < bestDistance)
                        {
                            bestDistance = dist;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                            bestTileA = edgeTileA;
                            bestTileB = edgeTileB;
                        }
                    }
                }
            }

            if (bestDistance < Mathf.Infinity && !forceConnectedToMain) 
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
        }

        if (bestDistance < Mathf.Infinity && forceConnectedToMain)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
             ConnectClosestRooms(all, true);
        }

        if (!forceConnectedToMain)
        {
            ConnectClosestRooms(all, true);
        }
    }

    private void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100);
        List<Coord> passageLine = GetPassageLine(tileA, tileB);
        foreach (Coord coord in passageLine)
        {
            DrawCircle(coord,2);
        }
    }

    void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x < r; x++)
        {
            for (int y = -r; y < r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.tileX + x;
                    int drawY = c.tileY + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        map[drawX, drawY] = MAP.empty;
                    }
                }
            }
            
        }
    }

    List<Coord> GetPassageLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;
        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        
        int step = (int) Mathf.Sign(dx);
        int gradientStep = (int) Mathf.Sign(dy);
        
        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);
            step = (int) Mathf.Sign(dy);
            gradientStep = (int) Mathf.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x,y));

            if (inverted) y += step;
            else x += step;
            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted) x += gradientStep;
                else y += gradientStep;
                            gradientAccumulation -= longest;

            }
        }

        return line;
    }

    Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
    }

    private int GetSurroundingWallTiles(int x, int y)
    {
        int wallCount = 0;
        for (int neighborX = x - 1; neighborX <= x + 1; neighborX++)
        {
            for (int neighborY = y - 1; neighborY <= y + 1; neighborY++)
            {
                if (neighborX == x && neighborY == y) continue; //self
                wallCount += isBorder(x, y) ? 1 : map[neighborX, neighborY];
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

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
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

public static class MAP
{
    public const int wall = 1, empty = 0;
}