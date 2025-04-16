using System;
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

    Tile currentTile;
    private Direction enterDirection; //im Spieler speichern, von wo er ein neues Tile betreten hat, um die Brücken Logik zu steuern

    [SerializeField] float moveSpeed;
    bool isMoving;
    bool canMove = true;

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

        Effect_SpeedBoost.OnActionTriggered += Effect_SpeedBoost_OnActionTriggered;
        Effect_Jump.OnActionTriggered += Effect_Jump_OnActionTriggered;
    }

    private void Effect_Jump_OnActionTriggered(ItemConfigSO_Jump cfg) {
        canMove = false;

        Direction dir = currentDirection;
        GridPosition currentTarget = this.gridPosition + directionMapping[dir];
        int tilesTested = 0;

        while (LevelGrid.Instance.GetTileAt(currentTarget) == null 
            || LevelGrid.Instance.GetTileAt(currentTarget).isBlocked) {
            tilesTested++;
            currentTarget += directionMapping[dir];
            if (tilesTested > cfg.maxTiles) {
                currentTarget = this.gridPosition;
                break;
            }
        }
        Debug.Log("tested: " + tilesTested + " tiles");
        JumpToGridPosition(currentTarget);
    }

    private void Effect_SpeedBoost_OnActionTriggered(ItemConfigSO_SpeedBoost obj) {
        StartCoroutine(HandleSpeedBoost(obj));
    }

    private void Update() {
        if (!canMove) return;

        this.gridPosition = LevelGrid.Instance.GridSystem.GetGridPosition(transform.position);
        currentTile = LevelGrid.Instance.GetTileAt(gridPosition);

        if (currentTile.hasBridge) {
            enterDirection = currentDirection;
        }

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

    private void JumpToGridPosition(GridPosition position) {
        transform.position = LevelGrid.Instance.GridSystem.GetWorldPosition(position);
        this.gridPosition = position;
        this.canMove = true;
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

        if (currentTile.hasBridge) {
            if ((enterDirection == Direction.North || enterDirection == Direction.South)
                && (dir == Direction.East || dir == Direction.West)) {
                return false;
            } else if ((enterDirection == Direction.East || enterDirection == Direction.West)
                  && (dir == Direction.North || dir == Direction.South)) {
                return false;
            }
        }

        var neighborConnections = LevelGrid.Instance.GetTileGrid()[gridPosition.x, gridPosition.z].GetConnections();
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
}
