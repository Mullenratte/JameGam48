using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private GridPosition gridPosition;
    private Dictionary<Direction, GridPosition> directionMapping;
    private Direction currentDirection;

    [SerializeField] float moveSpeed;
    bool isMoving;

    private void Start() {
        transform.position = LevelGrid.Instance.GridSystem.GetWorldPosition(new GridPosition(LevelGrid.Instance.GridSystem.GetWidth() / 2, LevelGrid.Instance.GridSystem.GetWidth() / 8));
        this.gridPosition = LevelGrid.Instance.GridSystem.GetGridPosition(transform.position);


        directionMapping = new Dictionary<Direction, GridPosition>();
        directionMapping.Add(Direction.North, new GridPosition(0, 1));
        directionMapping.Add(Direction.South, new GridPosition(0, -1));
        directionMapping.Add(Direction.West, new GridPosition(-1, 0));
        directionMapping.Add(Direction.East, new GridPosition(1, 0));
        this.currentDirection = Direction.East;
    }

    private void Update() {
        TryMoveOneTile();

        if (Input.GetKeyDown(KeyCode.A)) {
            currentDirection = Direction.West;
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            currentDirection = Direction.East;
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            currentDirection = Direction.North;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            currentDirection = Direction.South;
        }

        //GridPosition newPos = gridPosition + direction;
        //if (!isMoving && LevelGrid.Instance.GridSystem.IsValidGridPosition(newPos)) {
        //    Debug.Log("currentPos: " + gridPosition);
        //    Debug.Log("newPos: " + newPos);
        //    isMoving = true;
        //}
        //if (isMoving) {
        //    transform.position = Vector3.Lerp(LevelGrid.Instance.GridSystem.GetWorldPosition(this.gridPosition), LevelGrid.Instance.GridSystem.GetWorldPosition(newPos), Time.deltaTime * moveSpeed);
            
        //    if (LevelGrid.Instance.GridSystem.GetGridPosition(transform.position) == newPos) {
        //        this.gridPosition = newPos;
        //        isMoving = false;
        //        Debug.Log(isMoving);
        //    }
        //}





    }

    void TryMoveOneTile() {
        if (isMoving
            || !CanMoveInDirection(currentDirection)) return;

        GridPosition newPos = gridPosition + directionMapping[currentDirection];
        if (LevelGrid.Instance.GridSystem.IsValidGridPosition(newPos)) {
            this.gridPosition = newPos;
            StartCoroutine(LerpToNewTile(newPos));
        }
    }

    bool CanMoveInDirection(Direction dir) {
        var neighborConnections = LevelGrid.Instance.Generator.grid[gridPosition.x, gridPosition.z].GetConnections();
        return neighborConnections.ContainsKey(dir);
    }

    IEnumerator LerpToNewTile(GridPosition newPos) {
        float t = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = LevelGrid.Instance.GridSystem.GetWorldPosition(newPos);

        isMoving = true;

        while (t < 1) {
            t += Time.deltaTime * moveSpeed;

            if (t > 1) {
                t = 1;
            }

            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        isMoving = false;
    }
}
