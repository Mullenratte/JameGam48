using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private GridPosition gridPosition;
    private Dictionary<Direction, GridPosition> directionMapping;
    private Direction currentDirection;
    private Direction bufferedDirection;
    [SerializeField] float maxInputBufferTime;
    IEnumerator _HandleBufferedInputTimer;

    private Direction enteredFrom; //im Spieler speichern, von wo er ein neues Tile betreten hat, um die Br�cken Logik zu steuern

    [SerializeField] float moveSpeed;
    bool isMoving;


    private void Start() {
        transform.position = LevelGrid.Instance.GridSystem.GetWorldPosition(new GridPosition(LevelGrid.Instance.GridSystem.GetWidth() / 2, 0));
        this.gridPosition = LevelGrid.Instance.GridSystem.GetGridPosition(transform.position);


        directionMapping = new Dictionary<Direction, GridPosition>();
        directionMapping.Add(Direction.none, new GridPosition(0, 0));
        directionMapping.Add(Direction.North, new GridPosition(0, 1));
        directionMapping.Add(Direction.South, new GridPosition(0, -1));
        directionMapping.Add(Direction.West, new GridPosition(-1, 0));
        directionMapping.Add(Direction.East, new GridPosition(1, 0));
        this.currentDirection = Direction.East;

        Item_SpeedBoost.OnActionTriggered += Item_SpeedBoost_OnActionTriggered;
    }

    private void Update() {
        TryMoveOneTile();

        if (Input.GetKeyDown(KeyCode.A)) {
            UpdateBufferedDirection(Direction.West);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            UpdateBufferedDirection(Direction.East);
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            UpdateBufferedDirection(Direction.North);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            UpdateBufferedDirection(Direction.South);
        }
    }

    private void UpdateBufferedDirection(Direction dir) {
        if (_HandleBufferedInputTimer != null) StopCoroutine(_HandleBufferedInputTimer);
        bufferedDirection = dir;
        StartCoroutine(_HandleBufferedInputTimer = HandleBufferedInputTimer());
    }

    private void Item_SpeedBoost_OnActionTriggered(object sender, ItemConfigSO_SpeedBoost e) {
        StartCoroutine(HandleSpeedBoost(e));
    }

    IEnumerator HandleSpeedBoost(ItemConfigSO_SpeedBoost config) {
        float t = 0;
        float duration = config.duration;
        float originalMoveSpeed = this.moveSpeed;
        this.moveSpeed = this.moveSpeed * config.speedMultiplier;

        while (t < duration) {
            t += Time.deltaTime;
            yield return null;
        }
        this.moveSpeed = originalMoveSpeed;
    }

    void TryMoveOneTile() {
        if (isMoving) return;

        Direction dir = currentDirection;
        if (CanMoveInDirection(bufferedDirection)
            && bufferedDirection != currentDirection) {
            currentDirection = bufferedDirection;
            dir = bufferedDirection;
            bufferedDirection = Direction.none;

        } else if (!CanMoveInDirection(dir)) {
            return;
        }

        GridPosition newPos = gridPosition + directionMapping[dir];
        if (LevelGrid.Instance.GridSystem.IsValidGridPosition(newPos)) {
            this.gridPosition = newPos;
            StartCoroutine(LerpToNewTile(newPos));
        }
    }

    IEnumerator HandleBufferedInputTimer() {
        float t = 0;

        while (t < maxInputBufferTime) {
            t += Time.deltaTime;
            yield return null;
        }

        bufferedDirection = Direction.none;
    }


    bool CanMoveInDirection(Direction dir) {
        if (dir == Direction.none) return false;
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
