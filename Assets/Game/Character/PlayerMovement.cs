using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private GridPosition gridPosition;
    private GridPosition direction;

    [SerializeField] float moveSpeed;
    bool isMoving;

    private void Start() {
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
