using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTongueController : MonoBehaviour {

    [SerializeField] TongueConfigSO config;
    private Vector3 lookDir;
    [SerializeField] Transform tongueTransform;

    Vector3 mouseWorldPosOnGrid;
    [SerializeField] GameObject debugObj;

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1 << LayerMask.NameToLayer("Grid"))) {
            mouseWorldPosOnGrid = new Vector3(hit.point.x, 0, hit.point.z);
            debugObj.transform.position = mouseWorldPosOnGrid;
            lookDir = new Vector3(mouseWorldPosOnGrid.x - tongueTransform.position.x, 0, mouseWorldPosOnGrid.z - tongueTransform.position.z).normalized;
        }


    }

    private void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            dist = Mathf.Min(config.radius, Mathf.Abs(mouseWorldPosOnGrid.z - tongueTransform.position.z));
            RaycastHit[] hits = Physics.SphereCastAll(tongueTransform.position, config.thickness, lookDir, config.radius, ~LayerMask.GetMask("Grid", "Player"));
            
            foreach (RaycastHit hit in hits) {
                Debug.Log(hit.collider.name);
                if (hit.collider.TryGetComponent<ILickable>(out ILickable target)) {
                    target.TriggerOnLickedAction();
                    StartCoroutine(DragTargetIn(hit.collider.gameObject));
                }


        }
        }

    }

    IEnumerator DragTargetIn(GameObject target) {
        Vector3 startPos = target.transform.position;
        Vector3 endPos = tongueTransform.position;
        float t = 0;
        while (t < 1) {
            t += Time.deltaTime * config.snapSpeed / 2;

            if (t > 1) {
                t = 1;
            }

            target.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;

        }

        Destroy(target.gameObject);

    }

    float dist;

    void OnDrawGizmos() {
        Gizmos.color = UnityEngine.Color.red;
        Vector3 origin = tongueTransform.position;
        Vector3 end = origin + lookDir.normalized * dist;

        // Draw the start and end spheres
        Gizmos.DrawWireSphere(origin, config.thickness);
        Gizmos.DrawWireSphere(end, config.thickness);

        // Draw lines between the edges of the start and end spheres
        DrawConnectingLines(origin, end, config.thickness);
    }

    void DrawConnectingLines(Vector3 start, Vector3 end, float radius) {
        Vector3[] offsets = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        foreach (var offset in offsets) {
            Vector3 startOffset = start + offset * radius;
            Vector3 endOffset = end + offset * radius;
            Gizmos.DrawLine(startOffset, endOffset);
        }
    }



}

[CreateAssetMenu(fileName = "TongueConfigSO", menuName = "Scriptable Objects/TongueConfigSO")]
public class TongueConfigSO : ScriptableObject {
    public float radius;
    public float thickness;
    public float snapSpeed;



}

