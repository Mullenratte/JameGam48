using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DEBUG_GridObject : MonoBehaviour {
    private GridObject gridObject;
    [SerializeField] private Color n1_c, n2_c, n3_c, n4_c;
    public Sprite[] sprites;

    private Color activeColor;
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
            case 0:
                _spriteRenderer.sprite = sprites[0];
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;

            case 1:
                _spriteRenderer.sprite = sprites[1];
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2:
                _spriteRenderer.sprite = sprites[1];
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 3:
                _spriteRenderer.sprite = sprites[1];
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 4:
                _spriteRenderer.sprite = sprites[1];
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;

            case 5:
                _spriteRenderer.sprite = sprites[2];
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 6:
                _spriteRenderer.sprite = sprites[2];
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 7:
                _spriteRenderer.sprite = sprites[2];
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 8:
                _spriteRenderer.sprite = sprites[2];
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;

            case 9:
                _spriteRenderer.sprite = sprites[3];
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 10:
                _spriteRenderer.sprite = sprites[3];
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;

            case 11:
                _spriteRenderer.sprite = sprites[4];
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 12:
                _spriteRenderer.sprite = sprites[4];
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 13:
                _spriteRenderer.sprite = sprites[4];
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 14:
                _spriteRenderer.sprite = sprites[4];
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;

            case 15:
                _spriteRenderer.sprite = sprites[5];
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;

            case 16:
                _spriteRenderer.sprite = sprites[6];
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 17:
                _spriteRenderer.sprite = sprites[6];
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            default:
                Debug.LogError("Invalid tile type index: " + tileTypeIndex);
                break;
        }
    }

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _spriteRenderer = transform.Find("SpriteDisplay").GetComponent<SpriteRenderer>();
    }


}