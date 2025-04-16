using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour {
    public static LevelGrid Instance;

    [SerializeField] int width, depth;
    [SerializeField] float cellSize;
    public PathGenerator Generator { get; private set; }
    [SerializeField] public List<int> startXs = new List<int> { };

    // List of X positions where the sections should be connected together
    private List<int> connectXs = new List<int> { };

    public GridSystem GridSystem { get; private set; }

    private Tile[,] tileGrid;

    [SerializeField] Transform debug_tilePrefab1, debug_tilePrefab2;
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private Transform flyPrefab;

    [SerializeField] private List<BaseItem> items = new List<BaseItem>();

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

        Visualization visualization = GetComponent<Visualization>();
        visualization.Generator = Generator;
        visualization.setConnectXs(connectXs);

        for (int z = 0; z < tileGrid.GetLength(1); z++) {
            for (int x = 0; x < tileGrid.GetLength(0); x++) {
                GridPosition gridPosition = new GridPosition(x, z);
                GridObject gridObject = new GridObject(GridSystem, gridPosition, tileGrid[x, z]);
                Transform debugObjTransform = Instantiate(gridDebugObjectPrefab, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                DEBUG_GridObject debugObj = debugObjTransform.GetComponent<DEBUG_GridObject>();
                debugObj.SetGridObject(gridObject);
                if (debugObjTransform != null) {
                    debugObjTransform.SetParent(this.transform);
                    debugObjTransform.gameObject.layer = LayerMask.NameToLayer("Grid");
                }

                if (tileGrid[x,z].blockType == Tile.BlockType.FlySpawn)
                {
                    Transform flyObjTransform = Instantiate(flyPrefab, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                }

                if (tileGrid[x, z].blockType == Tile.BlockType.ItemSpawn) {
                    int rnd = UnityEngine.Random.Range(0, items.Count);
                    Transform itemTransform = Instantiate(items[rnd].gameObject.transform, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                    itemTransform.position += Vector3.up;
                }
            }
        }

        //AppendNewSection(); -> noch nicht Funktionsfähig

        //for (int z = 0; z < GridSystem.GetHeight(); z++) {
        //    for (int x = 0; x < GridSystem.GetWidth(); x++) {
        //        GridPosition gridPos = new GridPosition(x, z);
        //        Transform gridObj;
        //        if (x % 2 == 0 && z % 2 != 0 || x % 2 != 0 && z % 2 == 0) {
        //            gridObj = GridSystem.CreateObjectAtGridPos(gridPos, debug_tilePrefab1);
        //        } else {
        //            gridObj = GridSystem.CreateObjectAtGridPos(gridPos, debug_tilePrefab2);
        //        }

        //        if (gridObj != null) { 
        //            gridObj.SetParent(this.transform);
        //            gridObj.gameObject.layer = LayerMask.NameToLayer("Grid");
        //        }

        //    }
        //}


    }


    public Tile GetTileAt(GridPosition pos) {
        try {
            return tileGrid[pos.x, pos.z];
        } catch (Exception) {
            return null;
        }

    }

    public void AppendNewSection()
    {
        Tile[,] newSection = Generator.GeneratePaths(connectXs, tileGrid.GetLength(1));


    }

    public Tile[,] GetTileGrid() {
        return tileGrid;
    }
}
