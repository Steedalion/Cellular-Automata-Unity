using System.Collections.Generic;

public class Room
{
    private readonly List<Coord> tiles;
    public readonly List<Coord> edgeTiles;
    private readonly List<Room> connectedRooms;
    int roomSize;


    public Room()
    {
    }

    public Room(List<Coord> roomTiles, int[,] map)
    {
        this.tiles = roomTiles;
        roomSize = tiles.Count;
        connectedRooms = new List<Room>();
        edgeTiles = new List<Coord>();
        foreach (Coord tile in tiles)
        {
            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (x == tile.tileX || y == tile.tileY)
                    {
                        if (map[x, y] == MAP.wall)
                        {
                            edgeTiles.Add(tile);
                        }
                    }
                }
            }
        }
    }


    public static void ConnectRooms(Room A, Room B)
    {
        A.connectedRooms.Add(B);
        B.connectedRooms.Add(A);
    }

    public bool IsConnected(Room other)
    {
        return connectedRooms.Contains(other);
    }
}