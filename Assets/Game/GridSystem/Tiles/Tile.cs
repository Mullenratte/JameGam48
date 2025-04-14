using UnityEngine;

public class Tile
{
    private int type;
    private bool visited;
    public Vector2Int gridPosition;
    private Tile north, east, south, west;
    private bool hasBridge;

    public bool Visited { get => visited; set => visited = value; }
    public bool HasBridge { get => hasBridge; set => hasBridge = value; }
}
