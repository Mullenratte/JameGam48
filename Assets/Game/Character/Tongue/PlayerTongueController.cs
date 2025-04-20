using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTongueController : MonoBehaviour {

    [SerializeField] TongueConfigSO config;
    float range;
    [SerializeField] float rangeIncresePer100;
    [SerializeField] Transform mouthTransform;
    Vector3 mouseWorldPosOnGrid;
    Vector3 tongueTarget;
    LineRenderer _lineRenderer;

    [SerializeField] Light tongueLight;
    GameObject attachedObject;

    public static event Action<int> OnScoreObjectEaten;

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
        range = config.range;
        _lineRenderer.SetPosition(0, mouthTransform.position);   // Always follow frog mouth
        _lineRenderer.SetPosition(1, mouthTransform.position);

        currentState = TongueState.Default;
        HighScoreManager.Instance.OnScoreChange += HighScoreManager_OnScoreChange;
    }

    private void OnDestroy() {
        HighScoreManager.Instance.OnScoreChange -= HighScoreManager_OnScoreChange;
    }

    private void HighScoreManager_OnScoreChange(int score) {
        range = config.range + (score / 100) * rangeIncresePer100;
    }

    private void FixedUpdate() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, 1 << LayerMask.NameToLayer("Grid"))) {
            mouseWorldPosOnGrid = new Vector3(hit.point.x, 0, hit.point.z);
        }
    }

    private void Update() {
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
            if (Vector3.Distance(tongueTarget, mouthTransform.position) <= range) {
                RaycastHit[] hits = Physics.SphereCastAll(mouseWorldPosOnGrid, config.thickness, Vector3.up, 25f);
                foreach (var hit in hits) {
                    if (hit.collider.TryGetComponent<ILickable>(out _)) {
                        Vector3 targetXZ = new Vector3(hit.collider.transform.position.x, mouthTransform.position.y, hit.collider.transform.position.z);
                        if (Vector3.Distance(targetXZ, mouthTransform.position) <= range) {
                            tongueTarget = hit.collider.gameObject.transform.position;
                            attachedObject = hit.collider.gameObject;
                            break;
                        }
                    }
                }
            }


            _lineRenderer.SetPosition(0, mouthTransform.position);
            _lineRenderer.SetPosition(1, mouthTransform.position);
            tongueLight.gameObject.SetActive(true);
            tongueLight.transform.position = _lineRenderer.GetPosition(1);
            currentState = TongueState.Shooting;

            if (attachedObject != null) {
                SoundFXManager.instance.PlayRandomSoundFXClipPitchVariation(config.hitClips, transform.position, .9f, 0.7f, 1.2f);
            } else {
                SoundFXManager.instance.PlayRandomSoundFXClipPitchVariation(config.missClips, transform.position, .9f, 0.7f, 1f);
            }
        }
    }



    private void ExecuteShootingState() {
        // Update LineRenderer positions
        _lineRenderer.enabled = true;

        _lineRenderer.SetPosition(0, mouthTransform.position);
        _lineRenderer.SetPosition(1, Vector3.MoveTowards(_lineRenderer.GetPosition(1), tongueTarget, config.snapSpeed * Time.deltaTime));
        tongueLight.transform.position = _lineRenderer.GetPosition(1);

        // stop when at max radius or when target is reached
        if (Vector3.Distance(_lineRenderer.GetPosition(1), mouthTransform.position) >= range
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
        tongueLight.transform.position = _lineRenderer.GetPosition(1);
        if (attachedObject) {
            attachedObject.transform.position = _lineRenderer.GetPosition(1);
        }


        if (Vector3.Distance(_lineRenderer.GetPosition(1), mouthTransform.position) < 0.1f) {
            _lineRenderer.enabled = false;
            if (attachedObject) {
                attachedObject.GetComponent<ILickable>().TriggerOnCollectedAction();

                CinemachineCameraShake.Instance.ScreenshakeDefault(0.25f, 7f);

                if (attachedObject.TryGetComponent<IScoreObject>(out IScoreObject scoreObj)) {
                    OnScoreObjectEaten?.Invoke(scoreObj.GetScoreAmount());
                }
                attachedObject = null;
            }
            tongueLight.gameObject.SetActive(false);
            currentState = TongueState.Default;
        }
    }
}

