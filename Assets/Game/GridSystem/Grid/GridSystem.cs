using UnityEngine;

public class GridSystem {

    private int width, height;
    private float cellSize;



    public GridSystem(int width, int height, float cellSize) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        for (int z = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                GridPosition gridPos = new GridPosition(x, z);

            }
        }
    }

    public Transform CreateObjectAtGridPos(GridPosition gridPos, Transform debugPrefab) {
        Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPos), Quaternion.identity);
        return debugTransform;
    }

    public Vector3 GetWorldPosition(GridPosition gridPos) {
        return new Vector3(gridPos.x, 0, gridPos.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPos) {
        return new GridPosition(Mathf.RoundToInt(worldPos.x / cellSize), Mathf.RoundToInt(worldPos.z / cellSize));
    }

    public bool IsValidGridPosition(GridPosition gridPos) {
        return gridPos.x >= 0 &&
                gridPos.z >= 0 &&
                gridPos.x < width &&
                gridPos.z < height;
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }
}
