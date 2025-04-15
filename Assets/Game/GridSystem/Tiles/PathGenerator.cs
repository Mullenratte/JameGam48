﻿using System.Collections.Generic;
using System.Linq;
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
            //GeneratePathFrom(start);
            GenerateFlowAwarePathFrom(start);
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
                directions = GetShuffledDirections(current);
            }

            foreach (var dir in directions)
            {
                Vector3Int next = current + ToVector(dir);

                if (!InBounds(next)) continue;
                if (next.z == 0) continue;

                Tile fromTile = grid[current.x, current.z];
                Tile toTile = grid[next.x, next.z];

                float chance = Random.Range(0f, 1f);
                if (toTile.GetConnections().Count > 0 && chance < 0.5f) continue;

                chance = Random.Range(0f, 1f);
                if (toTile.GetConnections().Count > 2 && chance < 0.9f) continue;


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

    private void GenerateFlowAwarePathFrom(Vector3Int start)
    {
        Stack<Vector3Int> stack = new();
        HashSet<Vector3Int> visited = new();
        stack.Push(start);
        visited.Add(start);

        while (stack.Count > 0)
        {
            Vector3Int current = stack.Peek();
            List<(Vector3Int pos, Direction dir, int score)> candidates = new();

            if (current.z == depth - 1)
                break;

            foreach (Direction direction in GetShuffledDirections(current))
            {
                Vector3Int next = current + ToVector(direction);
                if (!InBounds(next)) continue;
                if (next.z == 0) continue;
                if (visited.Contains(next) && Random.Range(0f, 1f) < 0.9f) continue;

                var tile = grid[next.x, next.z];
                if (tile.GetConnections().Count >= 3) continue;

                int score = Score(direction, next, start);
                candidates.Add((next, direction, score));
            }

            if (candidates.Count == 0)
            {
                stack.Pop(); // Dead End
                continue;
            }

            var sorted = candidates.OrderByDescending(c => c.score).ToList();
            int topCount = Mathf.Min(3, sorted.Count);

            // weighted selection: bigger score higher chance
            float totalWeight = 0f;
            List<float> cumulativeWeights = new();

            for (int i = 0; i < topCount; i++)
            {
                float weight = sorted[i].score + 1;
                totalWeight += weight;
                cumulativeWeights.Add(totalWeight);
            }

            float rand = Random.Range(0f, totalWeight);

            int chosenIndex = 0;
            for (int i = 0; i < topCount; i++)
            {
                if (rand <= cumulativeWeights[i])
                {
                    chosenIndex = i;
                    break;
                }
            }

            var best = sorted[chosenIndex];

            Vector3Int nextPos = best.pos;
            Direction dir = best.dir;
            Direction opp = Tile.Opposite(dir);

            Tile fromTile = grid[current.x, current.z];
            Tile toTile = grid[nextPos.x, nextPos.z];

            fromTile.SetNeighbor(dir, toTile);
            toTile.SetNeighbor(opp, fromTile);

            fromTile.MarkEntry(dir);
            toTile.MarkEntry(opp);

            visited.Add(nextPos);
            stack.Push(nextPos);
        }
    }

    private int Score(Direction dir, Vector3Int nextPos, Vector3Int start)
    {
        var tile = grid[nextPos.x, nextPos.z];
        int connections = tile.GetConnections().Count;
        float centerDist = Vector3.Distance(new Vector3(width / 2f, 0, depth / 2f), nextPos);
        float startDist = Vector3.Distance(start, nextPos);

        int score = 0;
        score += (2 - connections); // less connections = better
        //score += Mathf.FloorToInt(startDist);   // distance to start
        score -= Mathf.FloorToInt(centerDist) * 2;  // wide distance to center = worse
        score -= dir switch
        {
            Direction.North => 0,
            Direction.East => 0,
            Direction.South => 2, // less chance to go south
            Direction.West => 0,
            _ => 0
        };

        return score;
    }

    private Vector3Int FindNearbyUnconnectedTile(Vector3Int start)
    {
        List<Direction> directions = GetShuffledDirections(start);
        foreach (var dir in directions)
        {
            Vector3Int next = start + ToVector(dir);
            if (InBounds(next) && grid[next.x, next.z].GetConnections().Count == 0)
            {
                return next;
            }
        }
        return Vector3Int.zero;
    }

    private bool InBounds(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.z >= 0 && pos.z < depth;
    }

    private List<Direction> GetShuffledDirections(Vector3Int position)
    {
        List<Direction> dirs = new List<Direction> {
            Direction.North, Direction.East, Direction.West, Direction.South
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

    private Direction ToDirection(Vector3Int vec)
    {
        if (vec == Vector3Int.forward) return Direction.North;
        if (vec == Vector3Int.back) return Direction.South;
        if (vec == Vector3Int.right) return Direction.East;
        if (vec == Vector3Int.left) return Direction.West;
        return Direction.none;
    }
}
