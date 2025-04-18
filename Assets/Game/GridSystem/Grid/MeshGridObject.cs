using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGridObject : MonoBehaviour {    
    private GridObject gridObject;
    [SerializeField] GameObject[] tileMeshPrefabs;

    [SerializeField] ParticleSystem _onDestroyPartSys;
    GameObject activeMeshPrefab;
    private bool hasFallen = false;

    public void SetGridObject(GridObject gridObject) {
        this.gridObject = gridObject;

        int neighbors = 0;
        Dictionary<Direction, Tile> tileConnections = gridObject.tile.GetConnections();
        foreach (var key in tileConnections.Keys) {
            if (tileConnections[key] != null) {
                neighbors++;
            }
        }

        SetMeshPrefab();
    }

    private void SetMeshPrefab()
    {
        int tileTypeIndex = gridObject.tile.GetTileType();

        GameObject prefabToSpawn = null;
        float yRotation = 0;

        switch (tileTypeIndex)
        {
            // Free tile
            case 0:
                prefabToSpawn = tileMeshPrefabs[0];
                yRotation = 0f;
                break;

            // Deadend tiles
            case 1:
                prefabToSpawn = tileMeshPrefabs[1];
                yRotation = 180f;
                break;
            case 2:
                prefabToSpawn = tileMeshPrefabs[1];
                yRotation = 270f;
                break;
            case 3:
                prefabToSpawn = tileMeshPrefabs[1];
                yRotation = 0f;
                break;
            case 4:
                prefabToSpawn = tileMeshPrefabs[1];
                yRotation = 90f;
                break;

            // Corner tiles
            case 5:
                prefabToSpawn = tileMeshPrefabs[2];
                yRotation = 270f;
                break;
            case 6:
                prefabToSpawn = tileMeshPrefabs[2];
                yRotation = 0f;
                break;
            case 7:
                prefabToSpawn = tileMeshPrefabs[2];
                yRotation = 90f;
                break;
            case 8:
                prefabToSpawn = tileMeshPrefabs[2];
                yRotation = 180f;
                break;

            // Straight tiles
            case 9:
                prefabToSpawn = tileMeshPrefabs[3];
                yRotation = 0f;
                break;
            case 10:
                prefabToSpawn = tileMeshPrefabs[3];
                yRotation = 90f;
                break;

            // T-junction tiles
            case 11:
                prefabToSpawn = tileMeshPrefabs[4];
                yRotation = 0f;
                break;
            case 12:
                prefabToSpawn = tileMeshPrefabs[4];
                yRotation = 90f;
                break;
            case 13:
                prefabToSpawn = tileMeshPrefabs[4];
                yRotation = 180f;
                break;
            case 14:
                prefabToSpawn = tileMeshPrefabs[4];
                yRotation = 270f;
                break;

            // Intersection tile
            case 15:
                prefabToSpawn = tileMeshPrefabs[5];
                yRotation = 0f;
                break;

            // Bridge tiles
            case 16:
                prefabToSpawn = tileMeshPrefabs[6];
                yRotation = 90f;
                break;
            case 17:
                prefabToSpawn = tileMeshPrefabs[6];
                yRotation = 0f;
                break;
            
            // Blocked tiles
            case 18:
                prefabToSpawn = tileMeshPrefabs[0];
                yRotation = 0f;
                break;
            case 19:
                prefabToSpawn = tileMeshPrefabs[0];
                yRotation = 0f;
                break;
            case 20:
                prefabToSpawn = tileMeshPrefabs[0];
                yRotation = 0f;
                break;

            default:
                Debug.LogError("Invalid tile type index: " + tileTypeIndex);
                break;
        }

        this.activeMeshPrefab = Instantiate(prefabToSpawn, this.transform);
        activeMeshPrefab.SetYRotation(yRotation);
    }

    private void Update()
    {
        if (hasFallen) return;

        Vector3 zonePosition = GameOverZone.Instance.GetPosition();
        if (zonePosition.z > transform.position.z)
        {
            hasFallen = true;


            float baseDelay = 0.05f;
            int gridWidth = LevelGrid.Instance.GetWidth();
            int centerX = Mathf.FloorToInt(gridWidth / 2f);
            int x = Mathf.RoundToInt(transform.position.x);
            int dist = Mathf.Abs(x - centerX);

            // Variante 1: innen → außen
            //float delay = dist * baseDelay;

            // Variante 2: außen → innen (optional)
            int maxDist = Mathf.Max(centerX, gridWidth - centerX - 1);
            float delay = (maxDist - dist) * baseDelay;

            MonoBehaviour mb = GetComponent<MonoBehaviour>();
            mb.StartCoroutine(DelayedFallAndDestroy(gameObject, delay));

            _onDestroyPartSys.Play();
        }
    }

    IEnumerator DelayedFallAndDestroy(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Kombinierbare Voranimationen:
        //yield return StartCoroutine(PlayShakeAnimation(obj));
        yield return StartCoroutine(PlayBounceAnimation(obj));

        // Hauptfall
        yield return StartCoroutine(PlayFallAnimation(obj));

        // Danach zerstören (oder deaktivieren)
        GameObject.Destroy(obj);
    }

    IEnumerator PlayShakeAnimation(GameObject obj)
    {
        float shakeDuration = 0.3f;
        float shakeStrength = 0.1f;
        float time = 0f;
        Vector3 originalPos = obj.transform.position;

        while (time < shakeDuration)
        {
            float offsetX = Mathf.Sin(time * 40f * shakeStrength);
            obj.transform.position = originalPos + new Vector3(offsetX, 0f, 0f);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = originalPos;
    }

    IEnumerator PlayBounceAnimation(GameObject obj)
    {
        float bounceHeight = 0.3f;
        float bounceDuration = 0.2f;
        Vector3 originalPos = obj.transform.position;
        Vector3 bounceUpPos = originalPos + Vector3.up * bounceHeight;

        float time = 0f;
        while (time < bounceDuration)
        {
            obj.transform.position = Vector3.Lerp(originalPos, bounceUpPos, time / bounceDuration);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;
        while (time < bounceDuration)
        {
            obj.transform.position = Vector3.Lerp(bounceUpPos, originalPos, time / bounceDuration);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = originalPos;
    }

    IEnumerator PlayFallAnimation(GameObject obj)
    {
        float fallDistance = 2f;
        float fallDuration = 0.5f;

        Vector3 startPos = obj.transform.position;
        Vector3 targetPos = startPos + Vector3.down * fallDistance;

        float time = 0f;
        while (time < fallDuration)
        {
            obj.transform.position = Vector3.Lerp(startPos, targetPos, time / fallDuration);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = targetPos;
    }
}
