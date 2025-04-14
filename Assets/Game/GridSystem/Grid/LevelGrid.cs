using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour {
    public static LevelGrid Instance;

    [SerializeField] int width, height;
    [SerializeField] float cellSize;
    public GridSystem GridSystem { get; private set; }

    [SerializeField] Transform debug_tilePrefab1, debug_tilePrefab2;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        GridSystem = new GridSystem(width, height, cellSize);
    }

    private void Start() {
        for (int z = 0; z < GridSystem.GetHeight(); z++) {
            for (int x = 0; x < GridSystem.GetWidth(); x++) {
                GridPosition gridPos = new GridPosition(x, z);
                Transform gridObj;
                if (x % 2 == 0 && z % 2 != 0 || x % 2 != 0 && z % 2 == 0) {
                    gridObj = GridSystem.CreateObjectAtGridPos(gridPos, debug_tilePrefab1);
                } else {
                    gridObj = GridSystem.CreateObjectAtGridPos(gridPos, debug_tilePrefab2);
                }

                if (gridObj != null) { 
                    gridObj.SetParent(this.transform);
                    gridObj.gameObject.layer = LayerMask.NameToLayer("Grid");
                }

            }
        }
    }
}
