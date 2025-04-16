using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Visualization : MonoBehaviour
{
    [SerializeField]public bool visualizeGrid = true;
    [SerializeField]public bool visualizeGridPos = true;
    [SerializeField]public int yOffset;
    private PathGenerator generator;
    private GUIStyle handleTextStyle = new GUIStyle();


    public PathGenerator Generator { get => generator; set => generator = value; }

    private void Awake()
    {
        handleTextStyle.normal.textColor = Color.black;
        handleTextStyle.fontSize = 14;
        handleTextStyle.fontStyle = FontStyle.Bold;
        handleTextStyle.alignment = TextAnchor.MiddleCenter;
    }
    void Start()
    {
    }

    void OnDrawGizmos()
    {
        if (!visualizeGrid || !Application.isPlaying) return;
        Tile[,] grid = LevelGrid.Instance.GetTileGrid();
        List<int> connectXs = LevelGrid.Instance.GetConnectXs();
        if (grid == null) return;

        float size = 0.4f;
        int width = grid.GetLength(0);
        int depth = grid.GetLength(1);

        for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
            {
                Tile tile = grid[x, z];
                Vector3 center = new Vector3(tile.gridPosition.x, yOffset, tile.gridPosition.z);

                switch(tile.VisType)
                {
                    case Tile.VisualType.SectionBeginning:
                        Gizmos.color = Color.yellow;
                        break;
                    case Tile.VisualType.SectionEnding:
                        Gizmos.color = Color.blue;
                        break;
                    case Tile.VisualType.SectionConnection:
                        Gizmos.color = Color.red;
                        break;
                    default:
                        Gizmos.color = Color.white;
                        break;
                }

                if (visualizeGridPos) Handles.Label(center + Vector3.up + Vector3.back * 0.01f, tile.gridPosition.ToString(), this.handleTextStyle);
                Gizmos.DrawWireCube(center, Vector3.one * size);

                Gizmos.color = Color.green;

                if (tile.north != null)
                    Gizmos.DrawLine(center, center + Vector3.forward * 0.5f);
                if (tile.south != null)
                    Gizmos.DrawLine(center, center + Vector3.back * 0.5f);
                if (tile.east != null)
                    Gizmos.DrawLine(center, center + Vector3.right * 0.5f);
                if (tile.west != null)
                    Gizmos.DrawLine(center, center + Vector3.left * 0.5f);

                Gizmos.color = Color.white;

                if (tile.hasBridge)
                {
                    Gizmos.color = tile.bridgeVisual == BridgeOrientation.NSOver_EWUnder ? Color.red : Color.blue;
                    Gizmos.DrawSphere(center + Vector3.up * 0.1f, 0.15f);
                    Gizmos.color = Color.white;
                }
            }
    }
}
