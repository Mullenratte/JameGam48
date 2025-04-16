using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PathGenerator
{
    private int width;
    private int depth;
    private Tile[,] grid;

    public int startZOfNextGeneration;

    public PathGenerator(int width, int depth)
    {
        this.width = width;
        this.depth = depth;
        grid = new Tile[width, depth];
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

    public List<int> GetConnectXs()
    {
        List<int> connectionXs = new List<int>();
        for (int x = 0; x < width; x++)
        {
            if (grid[x, (depth-1) - startZOfNextGeneration].north != null)
            {
                connectionXs.Add(x);
            }
        }
        return connectionXs;
    }

    public Tile[,] GeneratePaths(List<int> startXs, int startZ)
    {
        InitGrid();
        ScatterBlocks(startXs);
        foreach (int x in startXs)
        {
            Vector3Int start = new Vector3Int(x, 0, 0);
            GenerateFlowAwarePathFrom(start);
        }
        OverwriteFreeTiles();
        startZOfNextGeneration = Random.Range(1, 4);
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Tile tile = grid[x, z];
                bool isStart = z == 0;
                bool isEnd = z == (depth - 1) - startZOfNextGeneration;

                if (isStart || isEnd)
                {
                    if (tile.north == null)
                    {
                        tile.VisType = isStart ? Tile.VisualType.SectionBeginning : Tile.VisualType.SectionEnding;
                    }
                    else
                    {
                        tile.isConnection = true;
                        tile.VisType = Tile.VisualType.SectionConnection;
                    }
                }

                Vector3Int oldPos = tile.gridPosition;
                Vector3Int newPos = new Vector3Int(oldPos.x, oldPos.y, oldPos.z + startZ);
                tile.gridPosition = newPos;
            }
        }
        return CopyReducedGrid(grid);
    }

    // return a copy of the grid, not the reference
    private Tile[,] CopyReducedGrid(Tile[,] original)
    {
        int width = original.GetLength(0);
        int depth = original.GetLength(1) - startZOfNextGeneration;
        Tile[,] copy = new Tile[width, depth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                copy[x, z] = original[x, z];
            }
        }

        return copy;
    }


    private void ScatterBlocks(List<int> Xs)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (z < 2 && Xs.Contains(x))
                {
                    continue;
                }
                Tile tile = grid[x, z];
                if (Random.Range(0f, 1f) < 0.1f)
                {
                    tile.isBlocked = true;
                }else {
                    tile.blockType = 0; 
                }
            }
        }
    }

    private void OverwriteFreeTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Tile tile = grid[x, z];
                if (tile.GetConnections().Count == 0)
                {
                    tile.isBlocked = true;
                    if (z == 0 || z == depth - 1) continue; // skip first and last row, but set to blocked
                    if (HasStreetNeighbour(tile.gridPosition))
                    {
                        // Verteilung:
                        // 0 = none          -> 40 %
                        // 3 = deco element  -> 40 %
                        // 1 = fly-spawn     -> 15 %
                        // 2 = item-spawn    -> 5 %
                        float r = Random.value;
                        if (r < 0.4f)
                            tile.blockType = Tile.BlockType.None;
                        else if (r < 0.8f)
                            tile.blockType = Tile.BlockType.DecoElementSpawn;
                        else if (r < 0.95f)
                            tile.blockType = Tile.BlockType.FlySpawn;
                        else
                            tile.blockType = Tile.BlockType.ItemSpawn;
                    }
                    else
                    {
                        tile.blockType = Tile.BlockType.None; // 0 = none
                    }
                }
            }
        }
    }

    public bool HasStreetNeighbour(Vector3Int pos)
    {
        if (InBounds(pos + Vector3Int.forward) && grid[pos.x, pos.z + 1].GetConnections().Count > 0) return true;
        if (InBounds(pos + Vector3Int.back) && grid[pos.x, pos.z - 1].GetConnections().Count > 0) return true;
        if (InBounds(pos + Vector3Int.right) && grid[pos.x + 1, pos.z].GetConnections().Count > 0) return true;
        if (InBounds(pos + Vector3Int.left) && grid[pos.x - 1, pos.z].GetConnections().Count > 0) return true;
        return false;
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
                if (tile.isBlocked) continue; // skip spawn tiles
                if (tile.GetConnections().Count >= 3 && !(current.z == 0 && direction == Direction.North))
                {
                    continue;                    
                }

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

            ConnectTiles(current, best.pos, best.dir);

            visited.Add(best.pos);
            stack.Push(best.pos);
        }
    }

    private void ConnectTiles(Vector3Int fromPos, Vector3Int toPos, Direction dir)
    {
        Direction opp = Tile.Opposite(dir);

        Tile fromTile = grid[fromPos.x, fromPos.z];
        Tile toTile = grid[toPos.x, toPos.z];

        fromTile.SetNeighbor(dir, toTile);
        toTile.SetNeighbor(opp, fromTile);

        fromTile.MarkEntry(dir);
        toTile.MarkEntry(opp);
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
