using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DEBUG_GridObject : MonoBehaviour {
    private GridObject gridObject;
    [SerializeField] private Color n1_c, n2_c, n3_c, n4_c;
    public Sprite[] sprites;

    private Color activeColor;
    MeshRenderer _meshRenderer;

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
    }
    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
    }


}