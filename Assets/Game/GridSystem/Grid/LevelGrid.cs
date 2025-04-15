using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour {
    public static LevelGrid Instance;

    [SerializeField] int width, height;
    [SerializeField] float cellSize;
    public PathGenerator Generator { get; private set; }
    [SerializeField] public List<int> startXs = new List<int> { };

    public GridSystem GridSystem { get; private set; }

    [SerializeField] Transform debug_tilePrefab1, debug_tilePrefab2;
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private Transform flyPrefab;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        GridSystem = new GridSystem(width, height, cellSize);
    }






    private void Start() {
        Generator = new PathGenerator(width, height);
        //Generator.GeneratePaths(startXs);
        Tile[,] grid = Generator.GeneratePaths(startXs, 0);
        Debug.Log("Grid generated!");

        Visualization visualization = GetComponent<Visualization>();
        visualization.Generator = Generator;

        for (int z = 0; z < grid.GetLength(1); z++) {
            for (int x = 0; x < grid.GetLength(0); x++) {
                GridPosition gridPosition = new GridPosition(x, z);
                GridObject gridObject = new GridObject(GridSystem, gridPosition, grid[x, z]);
                Transform debugObjTransform = Instantiate(gridDebugObjectPrefab, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                DEBUG_GridObject debugObj = debugObjTransform.GetComponent<DEBUG_GridObject>();
                debugObj.SetGridObject(gridObject);
                if (debugObjTransform != null) {
                    debugObjTransform.SetParent(this.transform);
                    debugObjTransform.gameObject.layer = LayerMask.NameToLayer("Grid");
                }

                if (grid[x,z].blockType == Tile.BlockType.FlySpawn)
                {
                    Transform flyObjTransform = Instantiate(flyPrefab, GridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                }

            }
        }

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
}
