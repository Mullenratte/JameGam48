using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public static PlayerMovement Instance { get; private set; } = null;

    private GridPosition gridPosition;
    private GridPosition targetGridPosition;
    private Dictionary<Direction, GridPosition> directionMapping;
    private Direction currentDirection;
    private Direction bufferedDirection;
    [SerializeField] float maxInputBufferTime;
    IEnumerator _HandleBufferedInputTimer;

    [SerializeField, Range(0.01f, 1f)] float cutCornerDistance;

    Tile currentTile;
    private Direction enterDirection; //im Spieler speichern, von wo er ein neues Tile betreten hat, um die Brücken Logik zu steuern

    private Rigidbody rb;
    [SerializeField] float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    [SerializeField] float speedIncreasePer100 = 0.01f; // 0.01 increase per 100 points
    float moveSpeedModifier = 1f;

    [SerializeField] float rotateSpeed;
    public float RotateSpeed { get { return rotateSpeed; } set { rotateSpeed = value; } }

    bool canUpdateTargetPosition = true;
    bool canMove = true;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            rb = GetComponent<Rigidbody>();
        }
        else
        {
            Destroy(gameObject);
        }        
    }

    private void Start() {
        transform.position = LevelGrid.Instance.GridSystem.GetWorldPosition(new GridPosition(LevelGrid.Instance.GridSystem.GetWidth() / 2, 0));
        this.gridPosition = LevelGrid.Instance.GridSystem.GetGridPosition(transform.position);
        this.targetGridPosition = this.gridPosition;

        this.currentTile = LevelGrid.Instance.GetTileAt(gridPosition);

        directionMapping = new Dictionary<Direction, GridPosition>();
        directionMapping.Add(Direction.none, new GridPosition(0, 0));
        directionMapping.Add(Direction.North, new GridPosition(0, 1));
        directionMapping.Add(Direction.South, new GridPosition(0, -1));
        directionMapping.Add(Direction.West, new GridPosition(-1, 0));
        directionMapping.Add(Direction.East, new GridPosition(1, 0));
        this.currentDirection = Direction.East;

        Effect_SpeedBoost.OnActionTriggered += Effect_SpeedBoost_OnActionTriggered;
        Effect_Jump.OnActionTriggered += Effect_Jump_OnActionTriggered;
        HighScoreManager.Instance.OnScoreChange += HighScoreManager_OnScoreChange;
    }

    private void HighScoreManager_OnScoreChange(int score) {
        moveSpeedModifier += (score / 100) * speedIncreasePer100; // increase per 100 points
    }

    private void Effect_Jump_OnActionTriggered(ItemConfigSO_Jump cfg) {
        canMove = false;

        //Debug.Log("buffered: " + bufferedDirection);
        Direction dir = bufferedDirection != Direction.none ? bufferedDirection : currentDirection;
        GridPosition currentTarget = this.gridPosition + directionMapping[dir] * cfg.maxTiles;
        //Debug.Log("current pos: " + transform.position);
        //Debug.Log("target world pos: " + new Vector3(transform.position.x + directionMapping[dir].x * cfg.maxTiles, transform.position.y, transform.position.z + directionMapping[dir].z * cfg.maxTiles));
        //Debug.Log("nearest GRID pos: " + currentTarget);

        for (int tileTested = cfg.maxTiles; tileTested >= 0; tileTested--) {
            Vector3 targetWorldPos = new Vector3(transform.position.x + directionMapping[dir].x * tileTested, transform.position.y, transform.position.z + directionMapping[dir].z * tileTested);
            currentTarget = new GridPosition((int)Mathf.Round(targetWorldPos.x), (int)Mathf.Round(targetWorldPos.z));



            if (LevelGrid.Instance.GetTileAt(currentTarget) != null
            && !LevelGrid.Instance.GetTileAt(currentTarget).isBlocked) {
                break;
            }
        }

        this.targetGridPosition = currentTarget;
        JumpToGridPosition(currentTarget);
    }

    private void Effect_SpeedBoost_OnActionTriggered(ItemConfigSO_SpeedBoost obj) {
        StartCoroutine(HandleSpeedBoost(obj));
    }

    private void Update() {
        if (transform.position.z <= GameOverZone.Instance.transform.position.z) {
            this.rb.useGravity = true;
            this.canMove = false;
            GameManager.Instance.EndGame();
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            Debug.Log(transform.position);
        }
        if (!canMove) return;

        UpdateTargetGridPosition();

        if (Vector3.Distance(transform.position, LevelGrid.Instance.GridSystem.GetWorldPosition(targetGridPosition)) <= cutCornerDistance) {
            canUpdateTargetPosition = true;
        }

        this.gridPosition = LevelGrid.Instance.GridSystem.GetGridPosition(transform.position);
        currentTile = LevelGrid.Instance.GetTileAt(gridPosition);

        if (currentTile.hasBridge) {
            enterDirection = currentDirection;
        }

        // check for new generation
        if (this.gridPosition.z > LevelGrid.Instance.GetDepth() - 10)
        {
            Debug.Log("new section");
            LevelGrid.Instance.TryAppendAsync();
        }



        transform.position = Vector3.MoveTowards(transform.position, LevelGrid.Instance.GridSystem.GetWorldPosition(targetGridPosition), Time.deltaTime * MoveSpeed * moveSpeedModifier);
        transform.forward = Vector3.MoveTowards(transform.forward, new Vector3(directionMapping[currentDirection].x, 0f, directionMapping[currentDirection].z), Time.deltaTime * RotateSpeed);

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

    //private void FixedUpdate() {
    //    TryMoveOneTile();
    //}

    private void UpdateBufferedDirection(Direction dir) {
        if (_HandleBufferedInputTimer != null) StopCoroutine(_HandleBufferedInputTimer);
        bufferedDirection = dir;
        StartCoroutine(_HandleBufferedInputTimer = HandleBufferedInputTimer());
    }

    private void JumpToGridPosition(GridPosition position) {
        //Debug.Log("jumping to " + position);
        transform.position = LevelGrid.Instance.GridSystem.GetWorldPosition(position);
        //Debug.Log("which is in gridPos: " + LevelGrid.Instance.GridSystem.GetWorldPosition(position));
        this.gridPosition = position;
        this.canMove = true;
    }

    void UpdateTargetGridPosition() {
        if (!canUpdateTargetPosition
            && directionMapping[bufferedDirection] != directionMapping[currentDirection] * -1) {  // allows the player to turn around on the spot, but not cut corners
            return;
        }
        canUpdateTargetPosition = false;
        //isMoving = true;

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
            this.targetGridPosition = newPos;
            //StartCoroutine(LerpToNewTile(newPos));
        }
    }

    public GridPosition GetGridPosition()
    {
        return this.gridPosition;
    }

    public GridPosition GetTargetGridPosition()
    {
        return this.targetGridPosition;
    }

    public void UpdateGridPositions(GridPosition gridPos, GridPosition targetGridPos)
    {
        this.gridPosition = gridPos;
        this.targetGridPosition = targetGridPos;
    }

    //void TryMoveOneTile() {
    //    if (isMoving) return;

    //    //isMoving = true;

    //    Direction dir = currentDirection;
    //    if (CanMoveInDirection(bufferedDirection)
    //        && bufferedDirection != currentDirection) {
    //        currentDirection = bufferedDirection;
    //        dir = bufferedDirection;
    //        bufferedDirection = Direction.none;

    //    } else if (!CanMoveInDirection(dir)) {
    //        return;
    //    }

    //    GridPosition newPos = gridPosition + directionMapping[dir];
    //    if (LevelGrid.Instance.GridSystem.IsValidGridPosition(newPos)) {
    //        //Debug.Log("force: " + new Vector3(directionMapping[dir].x, 0f, directionMapping[dir].z) * moveSpeed);
    //        //Vector3 velocity = new Vector3(directionMapping[dir].x, 0f, directionMapping[dir].z) * moveSpeed * Time.fixedDeltaTime;
    //        //rb.MovePosition(rb.position + velocity);

    //        this.gridPosition = newPos;
    //        //StartCoroutine(LerpToNewTile(newPos));
    //    }
    //    Debug.Log("target Pos: " + newPos);
    //}

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

        canUpdateTargetPosition = true;

        while (t < 1) {
            t += Time.deltaTime * MoveSpeed;

            if (t > 1) {
                t = 1;
            }

            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        canUpdateTargetPosition = false;
    }

    IEnumerator HandleSpeedBoost(ItemConfigSO_SpeedBoost config) {
        float t = 0;
        float duration = config.duration;
        float originalMoveSpeed = MoveSpeed;
        MoveSpeed *= config.speedMultiplier;
        while (t < duration) {
            t += Time.deltaTime;
            yield return null;
        }
        MoveSpeed = originalMoveSpeed;
    }
}
