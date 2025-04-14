using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour {
    [SerializeField] int width, height;
    [SerializeField] float cellSize;
    private GridSystem gridSystem;

    [SerializeField] Transform debug_tilePrefab1, debug_tilePrefab2;

    private void Awake() {
        gridSystem = new GridSystem(width, height, cellSize);
    }

    private void Start() {
        for (int z = 0; z < gridSystem.GetHeight(); z++) {
            for (int x = 0; x < gridSystem.GetWidth(); x++) {
                GridPosition gridPos = new GridPosition(x, z);
                Transform gridObj;
                if (x % 2 == 0 && z % 2 != 0 || x % 2 != 0 && z % 2 == 0) {
                    gridObj = gridSystem.CreateObjectAtGridPos(gridPos, debug_tilePrefab1);
                } else {
                    gridObj = gridSystem.CreateObjectAtGridPos(gridPos, debug_tilePrefab2);
                }

                gridObj?.SetParent(this.transform);
            }
        }


        transform.position += new Vector3(-width / 2, 0, -height / 2);

    }
}
