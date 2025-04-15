using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Splines;

public enum Direction { North, East, South, West, none}

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
    public bool isBlocked = false;
    public BlockType blockType = BlockType.None; // 0 = none, 1 = fly-spawn, 2 = item-spawn, 3 = deco element spawn
    public enum BlockType
    {
        None,
        FlySpawn,
        ItemSpawn,
        DecoElementSpawn
    }

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

    public int GetTileType()
    {
        // simple checks first -> no connections 
        if (isBlocked && blockType == BlockType.None) return 0; // Free tile
        if (isBlocked) return (17 + (int) blockType); // Spawn tile

        // complex checks later
        if (north != null && east == null && south == null && west == null) return 1; // North connection only
        if (north == null && east != null && south == null && west == null) return 2; // East connection only
        if (north == null && east == null && south != null && west == null) return 3; // South connection only
        if (north == null && east == null && south == null && west != null) return 4; // West connection only

        if (north != null && east != null && south == null && west == null) return 5; // North and East connections
        if (north == null && east != null && south != null && west == null) return 6; // East and South connections
        if (north == null && east == null && south != null && west != null) return 7; // South and West connections
        if (north != null && east == null && south == null && west != null) return 8; // North and West connections

        if (north == null && east != null && south == null && west != null) return 9; // East and West connections
        if (north != null && east == null && south != null && west == null) return 10; // North and South connections

        if (north != null && east != null && south != null && west == null) return 11; // North, East and South connections
        if (north == null && east != null && south != null && west != null) return 12; // East, South and West connections
        if (north != null && east == null && south != null && west != null) return 13; // North, South and West connections
        if (north != null && east != null && south == null && west != null) return 14; // North, East and West connections

        if (!hasBridge) return 15; // No bridge
        if (this.bridgeVisual == BridgeOrientation.NSOver_EWUnder) return 16; // Bridge over NS
        if (this.bridgeVisual == BridgeOrientation.EWOver_NSUnder) return 17; // Bridge over EW

        throw new InvalidOperationException("Unerwartete Kombination");
    }
}
