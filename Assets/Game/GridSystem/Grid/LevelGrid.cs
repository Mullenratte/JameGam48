using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour {
    public static LevelGrid Instance;

    [SerializeField] int width, depth;
    [SerializeField] float cellSize;
    public PathGenerator Generator { get; private set; }
    [SerializeField] public List<int> startXs = new List<int> { };
    [SerializeField] public int TestAppends = 0; // Number of sections to append at the start of the game

    // List of X positions where the sections should be connected together
    private List<int> connectXs = new List<int> { };

    public GridSystem GridSystem { get; private set; }
    public int zOffset = 0; // Offset for the Z position of the grid objects

    private Tile[,] tileGrid;
    private Visualization visualization;

    [SerializeField] Transform debug_tilePrefab1, debug_tilePrefab2;
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private Transform flyPrefab;

    [SerializeField] private List<BaseItem> items = new List<BaseItem>();

    private bool isAppending = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        GridSystem = new GridSystem(width, depth, cellSize);
    }

    private void Start() {
        Generator = new PathGenerator(width, depth);
        tileGrid = Generator.GeneratePaths(startXs, 0);
        connectXs = Generator.GetConnectXs();

        Debug.Log("Grid generated!");

        visualization = GetComponent<Visualization>();
        visualization.Generator = Generator;

        GenerateStartObjects(0);

        for (int i = 0; i < TestAppends; i++)
        {
            TryAppendAsync();
        }
    }

    private void GenerateStartObjects(int startZ)
    {
        for (int z = startZ; z < tileGrid.GetLength(1); z++)
        {
            for (int x = 0; x < tileGrid.GetLength(0); x++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                GridObject gridObject = new GridObject(GridSystem, gridPosition, tileGrid[x, z]);

                //Transform model = Instantiate(debug_tilePrefab1, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                //if (model != null) {
                //    model.SetParent(this.transform);
                //    model.gameObject.layer = LayerMask.NameToLayer("Grid");
                //}


                Transform debugObjTransform = Instantiate(gridDebugObjectPrefab, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                DEBUG_GridObject debugObj = debugObjTransform.GetComponent<DEBUG_GridObject>();
                debugObj.SetGridObject(gridObject);
                if (debugObjTransform != null)
                {
                    debugObjTransform.SetParent(this.transform);
                    debugObjTransform.gameObject.layer = LayerMask.NameToLayer("Grid");
                }

                if (tileGrid[x, z].blockType == Tile.BlockType.FlySpawn)
                {
                    Transform flyObjTransform = Instantiate(flyPrefab, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                }

                if (tileGrid[x, z].blockType == Tile.BlockType.ItemSpawn)
                {
                    int rnd = UnityEngine.Random.Range(0, items.Count);
                    Transform itemTransform = Instantiate(items[rnd].gameObject.transform, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                    itemTransform.position += Vector3.up;
                }
            }
        }
    }

    private IEnumerator GenerateObjects(int startZ)
    {
        for (int z = startZ; z < tileGrid.GetLength(1); z++)
        {
            for (int x = 0; x < tileGrid.GetLength(0); x++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                GridObject gridObject = new GridObject(GridSystem, gridPosition, tileGrid[x, z]);

                //Transform model = Instantiate(debug_tilePrefab1, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                //if (model != null) {
                //    model.SetParent(this.transform);
                //    model.gameObject.layer = LayerMask.NameToLayer("Grid");
                //}


                Transform debugObjTransform = Instantiate(gridDebugObjectPrefab, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                DEBUG_GridObject debugObj = debugObjTransform.GetComponent<DEBUG_GridObject>();
                debugObj.SetGridObject(gridObject);
                if (debugObjTransform != null) {
                    debugObjTransform.SetParent(this.transform);
                    debugObjTransform.gameObject.layer = LayerMask.NameToLayer("Grid");
                }

                if (tileGrid[x, z].blockType == Tile.BlockType.FlySpawn)
                {
                    Transform flyObjTransform = Instantiate(flyPrefab, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                }

                if (tileGrid[x, z].blockType == Tile.BlockType.ItemSpawn)
                {
                    int rnd = UnityEngine.Random.Range(0, items.Count);
                    Transform itemTransform = Instantiate(items[rnd].gameObject.transform, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                    itemTransform.position += Vector3.up;
                }
                yield return null;
            }
        }
    }
    public Tile GetTileAt(GridPosition pos) {
        try {
            return tileGrid[pos.x, pos.z];
        } catch (Exception) {
            return null;
        }

    }

    public void TryAppendAsync()
    {   
        if (!isAppending)
        {
            StartCoroutine(AppendNewSectionAsync());
        }
    }

    private IEnumerator AppendNewSectionAsync()
    {
        isAppending = true;

        Tile[,] newSection = null;

        Debug.Log("Appending new section...");
        yield return RunInBackground(() =>
        {
            newSection = Generator.GeneratePaths(connectXs, tileGrid.GetLength(1));
            connectXs = Generator.GetConnectXs();
        });
        Debug.Log("New section generated!");

        int oldDepth = tileGrid.GetLength(1);
        tileGrid = CombineTileGrids(tileGrid, newSection);
        GridSystem.UpdateDimensions(tileGrid.GetLength(0), tileGrid.GetLength(1));

        for (int x = 0; x < tileGrid.GetLength(0); x++)
        {
            if (!tileGrid[x, oldDepth - 1].isConnection) continue;
            Tile southTile = tileGrid[x, oldDepth - 1];
            Tile northTile = tileGrid[x, oldDepth];
            southTile.SetNeighbor(Direction.North, northTile);
            northTile.SetNeighbor(Direction.South, southTile);
        }
        
        StartCoroutine(GenerateObjects(oldDepth));

        isAppending = false;
    }

    private IEnumerator RunInBackground(Action action)
    {
        bool done = false;
        System.Threading.Thread thread = new System.Threading.Thread(() =>
        {
            action();
            done = true;
        });
        thread.Start();

        while (!done)
            yield return null;
    }

    public void RemoveFirstRow()
    {
        Tile[,] newGrid = new Tile[tileGrid.GetLength(0), tileGrid.GetLength(1) - 1];
        for (int x = 0; x < tileGrid.GetLength(0); x++)
        {
            for (int z = 1; z < tileGrid.GetLength(1); z++)
            {
                newGrid[x, z - 1] = tileGrid[x, z];
            }
        }
        tileGrid = newGrid;
        zOffset++;
    }

    Tile[,] CombineTileGrids(Tile[,] original, Tile[,] toAppend)
    {
        int originalRows = original.GetLength(0);
        int originalCols = original.GetLength(1);
        int appendRows = toAppend.GetLength(0);
        int appendCols = toAppend.GetLength(1);

        if (originalRows != appendRows)
        {
            Debug.LogError("Tile grids couldn't be combined!");
            return null;
        }

        Tile[,] combined = new Tile[originalRows, originalCols + appendCols];

        for (int row = 0; row < originalRows; row++)
        {
            for (int col = 0; col < originalCols; col++)
            {
                combined[row, col] = original[row, col];
            }
        }

        for (int row = 0; row < appendRows; row++)
        {
            for (int col = 0; col < appendCols; col++)
            {
                combined[row, originalCols + col] = toAppend[row, col];
            }
        }

        return combined;
    }


    public Tile[,] GetTileGrid() {
        return tileGrid;
    }

    public List<int> GetConnectXs()
    {
        return connectXs;
    }

    public int GetWidth()
    {
        return tileGrid.GetLength(0);
    }

    public int GetDepth()
    {
        return tileGrid.GetLength(1);
    }
}
