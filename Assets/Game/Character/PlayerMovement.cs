using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public static PlayerMovement Instance { get; private set; } = null;

    private GridPosition gridPosition;
    private GridPosition targetGridPosition;
    private Tile highlightTile;
    private Vector3 targetWorldPos;
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

    GridPosition jumpTarget;

    AudioSource vehicleAudioSource;
    [SerializeField] AudioClip vehicleLoopClip;
    [SerializeField] float vehicleLoopBasePitch;
    [SerializeField] float vehicleLoopBaseVolume;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            rb = GetComponent<Rigidbody>();
        } else {
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
        GameOverZone.Instance.OnHitPlayer += GameOverZone_OnHitPlayer;

        vehicleAudioSource = SoundFXManager.instance.PlaySoundFXClipContinuously(vehicleLoopClip, transform, vehicleLoopBaseVolume, vehicleLoopBasePitch, vehicleLoopBasePitch);
    }

    private void OnDestroy() {
        Effect_SpeedBoost.OnActionTriggered -= Effect_SpeedBoost_OnActionTriggered;
        Effect_Jump.OnActionTriggered -= Effect_Jump_OnActionTriggered;
        HighScoreManager.Instance.OnScoreChange -= HighScoreManager_OnScoreChange;
        GameOverZone.Instance.OnHitPlayer -= GameOverZone_OnHitPlayer;
    }

    private void GameOverZone_OnHitPlayer() {
        StartCoroutine(HandleGameOverState(1.5f));
    }

    IEnumerator HandleGameOverState(float secondsToSceneChange) {
        this.rb.useGravity = true;
        this.canMove = false;
        float t = 0;

        // start animation

        while (t < secondsToSceneChange) {
            t += Time.deltaTime;
            yield return null;
        }

        GameManager.Instance.EndGame();
    }

    private void HighScoreManager_OnScoreChange(int score) {
        moveSpeedModifier = 1 + (score / 100) * speedIncreasePer100; // increase per 100 points

    }

    private void Effect_Jump_OnActionTriggered(ItemConfigSO_Jump cfg) {
        canMove = false;

        this.targetGridPosition = jumpTarget;
        JumpToGridPosition(jumpTarget);
        highlightTile?.HideHighlight();
        highlightTile = null;
    }

    private void Effect_SpeedBoost_OnActionTriggered(ItemConfigSO_SpeedBoost obj) {
        StartCoroutine(HandleSpeedBoost(obj));
    }


    private void Update() {
        if (!canMove) return;

        UpdateTargetGridPosition();

        targetWorldPos = LevelGrid.Instance.GridSystem.GetWorldPosition(targetGridPosition);

        this.gridPosition = LevelGrid.Instance.GridSystem.GetGridPosition(transform.position);

        if (Inventory.Instance.GetItemData()?.itemName == "Jump") {
            Direction dir = bufferedDirection != Direction.none ? bufferedDirection : currentDirection;
            ItemConfigSO_Jump cfg = ((Effect_Jump)Inventory.Instance.GetItemData()?.effect).config;
            GridPosition jumpTargetGridPos = this.gridPosition + directionMapping[dir] *  cfg.maxTiles;

            for (int tileTested = cfg.maxTiles; tileTested >= 0; tileTested--) {
                Vector3 targetWorldPos = new Vector3(transform.position.x + directionMapping[dir].x * tileTested, transform.position.y, transform.position.z + directionMapping[dir].z * tileTested);
                jumpTargetGridPos = new GridPosition((int)Mathf.Round(targetWorldPos.x), (int)Mathf.Round(targetWorldPos.z));



                if (LevelGrid.Instance.GetTileAt(jumpTargetGridPos) != null
                && !LevelGrid.Instance.GetTileAt(jumpTargetGridPos).isBlocked) {
                    break;
                }
            }

            highlightTile?.HideHighlight();
            highlightTile = LevelGrid.Instance.GetTileAt(jumpTargetGridPos);
            highlightTile?.ShowHighlight();
            this.jumpTarget = jumpTargetGridPos;
        }
        currentTile = LevelGrid.Instance.GetTileAt(gridPosition);

        if (currentTile.hasBridge) {
            enterDirection = currentDirection;
            if (currentTile.bridgeVisual == BridgeOrientation.NSOver_EWUnder && (enterDirection == Direction.North || enterDirection == Direction.South)) {
                if (currentDirection == Direction.North
                    && transform.position.z - LevelGrid.Instance.GridSystem.GetWorldPosition(gridPosition).z < 0) {
                    targetWorldPos = new Vector3(targetWorldPos.x, .4f, targetWorldPos.z); // update target world pos Y to reach ground height
                } else if (currentDirection == Direction.North
                    && transform.position.z - LevelGrid.Instance.GridSystem.GetWorldPosition(gridPosition).z > 0) {
                    targetWorldPos = new Vector3(targetWorldPos.x, -.4f, targetWorldPos.z); // update target world pos Y to reach bridge height
                } else if (currentDirection == Direction.South
                    && transform.position.z - LevelGrid.Instance.GridSystem.GetWorldPosition(gridPosition).z < 0) {
                    targetWorldPos = new Vector3(targetWorldPos.x, -.4f, targetWorldPos.z); // update target world pos Y to reach ground height
                } else if (currentDirection == Direction.South
                    && transform.position.z - LevelGrid.Instance.GridSystem.GetWorldPosition(gridPosition).z > 0) {
                    targetWorldPos = new Vector3(targetWorldPos.x, .4f, targetWorldPos.z); // update target world pos Y to reach bridge height
                }
            } 
            
            else if (currentTile.bridgeVisual == BridgeOrientation.EWOver_NSUnder && (enterDirection == Direction.West || enterDirection == Direction.East)) {
                if (currentDirection == Direction.East
                    && transform.position.x - LevelGrid.Instance.GridSystem.GetWorldPosition(gridPosition).x < 0) {
                    targetWorldPos = new Vector3(targetWorldPos.x, .4f, targetWorldPos.z); // update target world pos Y to reach ground height
                } else if (currentDirection == Direction.East
                    && transform.position.x - LevelGrid.Instance.GridSystem.GetWorldPosition(gridPosition).x > 0) {
                    targetWorldPos = new Vector3(targetWorldPos.x, -.4f, targetWorldPos.z); // update target world pos Y to reach bridge height
                } else if (currentDirection == Direction.West
                    && transform.position.x - LevelGrid.Instance.GridSystem.GetWorldPosition(gridPosition).x < 0) {
                    targetWorldPos = new Vector3(targetWorldPos.x, -.4f, targetWorldPos.z); // update target world pos Y to reach ground height
                } else if (currentDirection == Direction.West
                    && transform.position.x - LevelGrid.Instance.GridSystem.GetWorldPosition(gridPosition).x > 0) {
                    targetWorldPos = new Vector3(targetWorldPos.x, .4f, targetWorldPos.z); // update target world pos Y to reach bridge height
                }
            }
        }

        if (Vector3.Distance(transform.position, targetWorldPos) <= cutCornerDistance) {
            canUpdateTargetPosition = true;
        }

        // check for new generation
        if (this.gridPosition.z > LevelGrid.Instance.GetDepth() - 10) {
            Debug.Log("new section");
            LevelGrid.Instance.TryAppendAsync();
        }

        if (transform.position.y < 0) {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, Time.deltaTime * MoveSpeed * moveSpeedModifier);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(new Vector3(directionMapping[currentDirection].x, 0f, directionMapping[currentDirection].z)),
            Time.deltaTime * RotateSpeed);

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

        vehicleAudioSource.pitch = Mathf.Min(2f, vehicleLoopBasePitch + GetEffectiveMoveSpeed() * 0.1f);
        vehicleAudioSource.volume = Mathf.Min(0.15f, vehicleLoopBaseVolume + GetEffectiveMoveSpeed() * 0.015f);
    }

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
            if (currentTile.hasBridge) Debug.Log("can move in dir " + dir);
        } else if (!CanMoveInDirection(dir)) {
            if (currentTile.hasBridge) Debug.Log("can NOT move in dir " + dir);

            return;
        }

        GridPosition newPos = gridPosition + directionMapping[dir];
        if (LevelGrid.Instance.GridSystem.IsValidGridPosition(newPos)) {
            this.targetGridPosition = newPos;
            //StartCoroutine(LerpToNewTile(newPos));
        }
    }

    public GridPosition GetGridPosition() {
        return this.gridPosition;
    }

    public GridPosition GetTargetGridPosition() {
        return this.targetGridPosition;
    }

    public void UpdateGridPositions(GridPosition gridPos, GridPosition targetGridPos) {
        this.gridPosition = gridPos;
        this.targetGridPosition = targetGridPos;
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
        float addedSpeedBoost = (MoveSpeed * config.speedMultiplier) - MoveSpeed;
        MoveSpeed += addedSpeedBoost;
        while (t < duration) {
            t += Time.deltaTime;
            yield return null;
        }
        MoveSpeed -= addedSpeedBoost;
    }

    public float GetEffectiveMoveSpeed() {
        return MoveSpeed * moveSpeedModifier;
    }
}
