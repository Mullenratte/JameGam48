using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public enum Direction { North, East, South, West }

public enum BridgeOrientation
{
    None,
    NSOver_EWUnder,
    EWOver_NSUnder
}

public class Tile
{
    public Tile north, east, south, west;
    public Vector3Int gridPosition;

    public HashSet<Direction> enteredFrom = new HashSet<Direction>();
    public bool hasBridge = false;
    public BridgeOrientation bridgeVisual = BridgeOrientation.None;

    public Dictionary<Direction, Tile> GetConnections()
    {
        var dict = new Dictionary<Direction, Tile>();
        if (north != null) dict[Direction.North] = north;
        if (east != null) dict[Direction.East] = east;
        if (south != null) dict[Direction.South] = south;
        if (west != null) dict[Direction.West] = west;
        return dict;
    }

    public void SetNeighbor(Direction dir, Tile neighbor)
    {
        switch (dir)
        {
            case Direction.North: north = neighbor; break;
            case Direction.East: east = neighbor; break;
            case Direction.South: south = neighbor; break;
            case Direction.West: west = neighbor; break;
        }
    }

    public static Direction Opposite(Direction dir)
    {
        return dir switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            _ => dir
        };
    }

    public void MarkEntry(Direction fromDir)
    {
        enteredFrom.Add(fromDir);

        if (enteredFrom.Count == 4)
        {
            hasBridge = UnityEngine.Random.Range(0f, 1f) < 0.5f;

            if (hasBridge)
            {
                bridgeVisual = UnityEngine.Random.Range(0, 2) == 0
                    ? BridgeOrientation.NSOver_EWUnder
                    : BridgeOrientation.EWOver_NSUnder;
            }
            else
            {
                bridgeVisual = BridgeOrientation.None;
            }
        }
        else
        {
            hasBridge = false;
            bridgeVisual = BridgeOrientation.None;
        }
    }

    public int GetTileTypeIndex()
    {
        int baseValue = 0;

        if (north != null) baseValue |= 1 << 0;
        if (east != null) baseValue |= 1 << 1;
        if (south != null) baseValue |= 1 << 2;
        if (west != null) baseValue |= 1 << 3;

        if (baseValue != 15)
        {
            return baseValue;
        }

        if (!hasBridge) return 15;
        if (hasBridge && bridgeVisual != BridgeOrientation.EWOver_NSUnder) return 16;
        if (hasBridge && bridgeVisual == BridgeOrientation.EWOver_NSUnder) return 17;

        throw new InvalidOperationException("Unerwartete Kombination");
    }

}
