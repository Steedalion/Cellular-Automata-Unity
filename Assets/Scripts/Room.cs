using System;using System.Collections.Generic;

public class Room: IComparable<Room>
{
    private readonly List<Coord> tiles;
    public readonly List<Coord> edgeTiles;
    public readonly List<Room> connectedRooms;
    public bool isConnectedToMain, isMainRoom;
    public int roomSize;


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
        if (A.isConnectedToMain)
        {
            B.SetAccessableFromMainRoom();
        } else if (B.isConnectedToMain)
        {
            A.SetAccessableFromMainRoom();
        }
        A.connectedRooms.Add(B);
        B.connectedRooms.Add(A);
    }
    
    void SetAccessableFromMainRoom()
    {
        if (!isConnectedToMain)
        {
            isConnectedToMain = true;
            foreach (Room connectedRoom in connectedRooms)
            {
                connectedRoom.SetAccessableFromMainRoom();
            }
        }
        
    }

    public bool IsConnected(Room other)
    {
        return connectedRooms.Contains(other);
    }

    public int CompareTo(Room other)
    {
        if (ReferenceEquals(this, other)) return 1;
        if (ReferenceEquals(null, other)) return 0;
        return other.roomSize.CompareTo(roomSize);
    }
}