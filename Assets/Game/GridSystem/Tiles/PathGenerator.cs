using System.Collections.Generic;
using UnityEngine;

public class PathGenerator
{
    public int width;
    public int depth;
    public Tile[,] grid;

    public int startNextGeneration;

    public PathGenerator(int width, int height)
    {
        this.width = width;
        this.depth = height;
        grid = new Tile[width, height];
        InitGrid();
    }

    private void InitGrid()
    {
        for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
            {
                grid[x, z] = new Tile
                {
                    gridPosition = new Vector3Int(x, 0, z)
                };
            }
    }

    public void GeneratePaths(List<int> startXs)
    {
        foreach (int x in startXs)
        {
            Vector3Int start = new Vector3Int(x, 0, 0);
            GeneratePathFrom(start);
        }

        startNextGeneration = Random.Range(depth - 3, depth);
    }

    private void GeneratePathFrom(Vector3Int start)
    {
        Vector3Int current = start;

        while (current.z < depth - 1)
        {
            List<Direction> directions;
            if (current.z == 0)
            {
                directions = new List<Direction> { Direction.North };
            }
            else
            {
                directions = GetShuffledDirections();
            }

            foreach (var dir in directions)
            {
                Vector3Int next = current + ToVector(dir);

                if (!InBounds(next)) continue;
                if (next.z == 0) continue;

                Tile fromTile = grid[current.x, current.z];
                Tile toTile = grid[next.x, next.z];

                Direction opposite = Tile.Opposite(dir);
                fromTile.SetNeighbor(dir, toTile);
                toTile.SetNeighbor(opposite, fromTile);

                toTile.MarkEntry(opposite);
                fromTile.MarkEntry(dir);

                current = next;
                break;
            }
        }
    }

    private bool InBounds(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.z >= 0 && pos.z < depth;
    }

    private List<Direction> GetShuffledDirections()
    {
        List<Direction> dirs = new List<Direction> {
            Direction.North, Direction.East, Direction.South, Direction.West
        };
        for (int i = 0; i < dirs.Count; i++)
        {
            int r = Random.Range(i, dirs.Count);
            (dirs[i], dirs[r]) = (dirs[r], dirs[i]);
        }
        return dirs;
    }

    private Vector3Int ToVector(Direction dir)
    {
        return dir switch {
            Direction.North => Vector3Int.forward,
            Direction.South => Vector3Int.back,
            Direction.East  => Vector3Int.right,
            Direction.West  => Vector3Int.left,
            _ => Vector3Int.zero
        };
    }
}
