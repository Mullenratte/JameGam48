using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DEBUG_GridObject : MonoBehaviour {
    [SerializeField] private Color n1_c, n2_c, n3_c, n4_c;
    
    private GridObject gridObject;    
    public Sprite[] sprites;
    private Color activeColor;

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
        Vector3 zonePosition = GameOverZone.Instance.GetPosition();
        if (zonePosition.z > transform.position.z)
        {
            if (gridObject.GetGridPosition().x == LevelGrid.Instance.GetWidth() - 1)
            {
                //LevelGrid.Instance.RemoveFirstRow(); -> TODO: Fix Player Movement when used
            }
            Destroy(gameObject);
        } 
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