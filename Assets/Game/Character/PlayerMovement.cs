using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private GridPosition gridPosition;
    private GridPosition direction;

    [SerializeField] float moveSpeed;
    bool isMoving;

    private void Start() {
        transform.position = LevelGrid.Instance.GridSystem.GetWorldPosition(new GridPosition(LevelGrid.Instance.GridSystem.GetWidth() / 2, LevelGrid.Instance.GridSystem.GetWidth() / 8));
        this.gridPosition = LevelGrid.Instance.GridSystem.GetGridPosition(transform.position);
        this.direction = new GridPosition(1, 0);
    }

    private void Update() {
        TryMoveOneTile();

        if (Input.GetKeyDown(KeyCode.A)) {
            direction = new GridPosition(-1, 0);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            direction = new GridPosition(1, 0);
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            direction = new GridPosition(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            direction = new GridPosition(0, -1);
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
        if (isMoving) return;

        GridPosition newPos = gridPosition + direction;
        if (LevelGrid.Instance.GridSystem.IsValidGridPosition(newPos)) {
            this.gridPosition = newPos;
            StartCoroutine(LerpToNewTile(newPos));
        }
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
