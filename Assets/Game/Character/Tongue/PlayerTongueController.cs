using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTongueController : MonoBehaviour {

    [SerializeField] TongueConfigSO config;

    [SerializeField] Transform mouthTransform;
    Vector3 mouseWorldPosOnGrid;
    Vector3 tongueTarget;
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
            if (Vector3.Distance(tongueTarget, mouthTransform.position) <= config.range) {
                RaycastHit[] hits = Physics.SphereCastAll(mouseWorldPosOnGrid, config.thickness, Vector3.up, 25f);
                foreach (var hit in hits) {
                    if (hit.collider.TryGetComponent<ILickable>(out _)) {
                        Vector3 targetXZ = new Vector3(hit.collider.transform.position.x, mouthTransform.position.y, hit.collider.transform.position.z);
                        if (Vector3.Distance(targetXZ, mouthTransform.position) <= config.range) {
                            tongueTarget = hit.collider.gameObject.transform.position;
                            attachedObject = hit.collider.gameObject;
                            Debug.Log("obj: " + attachedObject);
                            break;
                        }
                    }
                }
            }


            _lineRenderer.SetPosition(0, mouthTransform.position);
            _lineRenderer.SetPosition(1, mouthTransform.position);
            currentState = TongueState.Shooting;

            if (attachedObject != null) {
                SoundFXManager.instance.PlayRandomSoundFXClipPitchVariation(config.hitClips, transform.position, 1f, 0.7f, 1.2f);
            } else {
                SoundFXManager.instance.PlayRandomSoundFXClipPitchVariation(config.missClips, transform.position, 1f, 0.7f, 1f);
            }
        }
    }



    private void ExecuteShootingState() {
        // Update LineRenderer positions
        _lineRenderer.enabled = true;

        _lineRenderer.SetPosition(0, mouthTransform.position);
        _lineRenderer.SetPosition(1, Vector3.MoveTowards(_lineRenderer.GetPosition(1), tongueTarget, config.snapSpeed * Time.deltaTime));


        // stop when at max radius or when target is reached
        if (Vector3.Distance(_lineRenderer.GetPosition(1), mouthTransform.position) >= config.range
            || Vector3.Distance(_lineRenderer.GetPosition(1), tongueTarget) < 0.01f) {
            if (attachedObject) {
                attachedObject.GetComponent<ILickable>().TriggerOnHitAction();
            }
            currentState = TongueState.Retracting;
            return;
        }
    }

    private void ExecuteRetractingState() {
        _lineRenderer.SetPosition(0, mouthTransform.position);
        _lineRenderer.SetPosition(1, Vector3.MoveTowards(_lineRenderer.GetPosition(1), mouthTransform.position, config.retractSpeed * Time.deltaTime));
        if (attachedObject) {
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

