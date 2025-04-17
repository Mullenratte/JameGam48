using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DEBUG_GridObject : MonoBehaviour {
    [SerializeField] private Color n1_c, n2_c, n3_c, n4_c;
    
    private GridObject gridObject;    
    public Sprite[] sprites;
    private Color activeColor;
    private bool hasFallen = false;

    GameObject spriteDisplayObject;
    MeshRenderer _meshRenderer;
    SpriteRenderer _spriteRenderer;

    public void SetGridObject(GridObject gridObject) {
        this.gridObject = gridObject;

        int neighbors = 0;
        Dictionary<Direction, Tile> tileConnections = gridObject.tile.GetConnections();
        foreach (var key in tileConnections.Keys) {
            if (tileConnections[key] != null) {
                neighbors++;
            }
        }

        switch (neighbors) {
            case 1: activeColor = n1_c; break;
            case 2: activeColor = n2_c; break;
            case 3: activeColor = n3_c; break;
            case 4: activeColor = n4_c; break;
        }
        _meshRenderer.material.color = activeColor;

        SetTopSprite();
    }

    private void SetTopSprite()
    {
        int tileTypeIndex = gridObject.tile.GetTileType();

        switch (tileTypeIndex)
        {
            // Free tile
            case 0:
                _spriteRenderer.sprite = sprites[0];
                spriteDisplayObject.SetZRotation(0f);
                break;

            // Deadend tiles
            case 1:
                _spriteRenderer.sprite = sprites[1];
                spriteDisplayObject.SetZRotation(0f);
                break;
            case 2:
                _spriteRenderer.sprite = sprites[1];
                spriteDisplayObject.SetZRotation(90f);
                break;
            case 3:
                _spriteRenderer.sprite = sprites[1];
                spriteDisplayObject.SetZRotation(180f);
                break;
            case 4:
                _spriteRenderer.sprite = sprites[1];
                spriteDisplayObject.SetZRotation(270f);
                break;

            // Corner tiles
            case 5:
                _spriteRenderer.sprite = sprites[2];
                spriteDisplayObject.SetZRotation(0f);
                break;
            case 6:
                _spriteRenderer.sprite = sprites[2];
                spriteDisplayObject.SetZRotation(90f);
                break;
            case 7:
                _spriteRenderer.sprite = sprites[2];
                spriteDisplayObject.SetZRotation(180f);
                break;
            case 8:
                _spriteRenderer.sprite = sprites[2];
                spriteDisplayObject.SetZRotation(270f);
                break;

            // Straight tiles
            case 9:
                _spriteRenderer.sprite = sprites[3];
                spriteDisplayObject.SetZRotation(0f);
                break;
            case 10:
                _spriteRenderer.sprite = sprites[3];
                spriteDisplayObject.SetZRotation(90f);
                break;

            // T-junction tiles
            case 11:
                _spriteRenderer.sprite = sprites[4];
                spriteDisplayObject.SetZRotation(0f);
                break;
            case 12:
                _spriteRenderer.sprite = sprites[4];
                spriteDisplayObject.SetZRotation(90f);
                break;
            case 13:
                _spriteRenderer.sprite = sprites[4];
                spriteDisplayObject.SetZRotation(180f);
                break;
            case 14:
                _spriteRenderer.sprite = sprites[4];
                spriteDisplayObject.SetZRotation(270f);
                break;

            // Intersection tile
            case 15:
                _spriteRenderer.sprite = sprites[5];
                spriteDisplayObject.SetZRotation(0f);
                break;

            // Bridge tiles
            case 16:
                _spriteRenderer.sprite = sprites[6];
                spriteDisplayObject.SetZRotation(90f);
                break;
            case 17:
                _spriteRenderer.sprite = sprites[6];
                spriteDisplayObject.SetZRotation(0f);
                break;
            
            // Blocked tiles
            case 18:
                _spriteRenderer.sprite = sprites[7];
                spriteDisplayObject.SetZRotation(0f);
                break;
            case 19:
                _spriteRenderer.sprite = sprites[8];
                spriteDisplayObject.SetZRotation(0f);
                break;
            case 20:
                _spriteRenderer.sprite = sprites[0];
                spriteDisplayObject.SetZRotation(0f);
                break;

            default:
                Debug.LogError("Invalid tile type index: " + tileTypeIndex);
                break;
        }
    }

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        this.spriteDisplayObject = transform.Find("SpriteDisplay").gameObject;
        _spriteRenderer = this.spriteDisplayObject.GetComponent<SpriteRenderer>();
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
            float offsetX = Mathf.Sin(time * 40f) * shakeStrength;
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

public static class TransformExtensions
{
    /// <summary>
    /// Setzt nur die Z-Rotation eines GameObjects, ohne X- und Y-Rotation zu verändern.
    /// </summary>
    /// <param name="transform">Das Transform-Objekt des GameObjects</param>
    /// <param name="zAngle">Die neue Rotation in Grad um die Z-Achse</param>
    public static void SetZRotation(this GameObject toTransform, float zAngle)
    {
        Vector3 currentEuler = toTransform.transform.rotation.eulerAngles;
        currentEuler.z = zAngle;
        toTransform.transform.rotation = Quaternion.Euler(currentEuler);
    }
}