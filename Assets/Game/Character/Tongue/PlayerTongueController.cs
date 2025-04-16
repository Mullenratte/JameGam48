using System;
using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTongueController : MonoBehaviour {

    [SerializeField] TongueConfigSO config;

    [SerializeField] Transform mouthTransform;
    Vector3 mouseWorldPosOnGrid;
    [SerializeField] Transform tongueTip;
    Vector3 tongueTarget;
    [SerializeField] CapsuleCollider capsuleColl;
    LineRenderer _lineRenderer;

    GameObject attachedObject;

    public enum TongueState {
        Default,
        Shooting,
        Retracting
    }

    TongueState currentState;

    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start() {
        _lineRenderer.SetPosition(0, mouthTransform.position);   // Always follow frog mouth
        _lineRenderer.SetPosition(1, mouthTransform.position);

        currentState = TongueState.Default;
        capsuleColl.radius = config.thickness;
        tongueTip.GetComponent<TongueCollider>().OnTriggerEntered += TongueCollider_OnTriggerEntered;
    }

    private void TongueCollider_OnTriggerEntered(Collider obj) {
        if (obj.TryGetComponent<ILickable>(out ILickable lickable)) {
            attachedObject = obj.gameObject;
        }
    }

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1 << LayerMask.NameToLayer("Grid"))) {
            mouseWorldPosOnGrid = new Vector3(hit.point.x, 0, hit.point.z);
        }

        switch (currentState) {
            case TongueState.Default:
                ExecuteDefaultState();
                break;
            case TongueState.Shooting:
                ExecuteShootingState();
                break;
            case TongueState.Retracting:
                ExecuteRetractingState();
                break;
            default:
                break;
        }

    }

    private void ExecuteDefaultState() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            tongueTarget = new Vector3(mouseWorldPosOnGrid.x, mouthTransform.position.y, mouseWorldPosOnGrid.z);
            _lineRenderer.SetPosition(0, mouthTransform.position);
            _lineRenderer.SetPosition(1, mouthTransform.position);
            currentState = TongueState.Shooting;
        }
    }



    private void ExecuteShootingState() {
        // Update LineRenderer positions
        _lineRenderer.enabled = true;
        capsuleColl.enabled = true;

        _lineRenderer.SetPosition(0, mouthTransform.position);
        _lineRenderer.SetPosition(1, Vector3.MoveTowards(_lineRenderer.GetPosition(1), tongueTarget, config.snapSpeed * Time.deltaTime));
        tongueTip.transform.position = _lineRenderer.GetPosition(1);


        // stop when at max radius or when target is reached
        if (Vector3.Distance(_lineRenderer.GetPosition(1), mouthTransform.position) >= config.range
            || Vector3.Distance(_lineRenderer.GetPosition(1), tongueTarget) < 0.01f
            || attachedObject) {
            currentState = TongueState.Retracting;
            return;
        }
    }

    private void ExecuteRetractingState() {
        capsuleColl.enabled = false;
        tongueTip.position = _lineRenderer.GetPosition(0);

        _lineRenderer.SetPosition(0, mouthTransform.position);
        _lineRenderer.SetPosition(1, Vector3.MoveTowards(_lineRenderer.GetPosition(1), mouthTransform.position, config.retractSpeed * Time.deltaTime));
        if (attachedObject != null) {
            attachedObject.transform.position = _lineRenderer.GetPosition(1);
        }


        if (Vector3.Distance(_lineRenderer.GetPosition(1), mouthTransform.position) < 0.1f) {
            _lineRenderer.enabled = false;
            if (attachedObject) {
                attachedObject.GetComponent<ILickable>().TriggerOnCollectedAction();
                attachedObject = null;
            }
            currentState = TongueState.Default;
        }
    }
}

