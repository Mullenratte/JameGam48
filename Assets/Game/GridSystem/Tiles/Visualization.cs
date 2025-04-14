using System.Collections.Generic;
using UnityEngine;

public class Visualization : MonoBehaviour
{
    [SerializeField]public bool visualizeGrid = true;
    [SerializeField]public int width;
    [SerializeField]public int depth;
    [SerializeField]public int yOffset;
    [SerializeField]public List<int> startXs = new List<int> {};
    private PathGenerator generator;

    public PathGenerator Generator { get => generator; set => generator = value; }

    void Start()
    {
        //generator = new PathGenerator(width, depth);
        //generator.GeneratePaths(startXs);
        //Debug.Log("Grid generated!");
    }

    void OnDrawGizmos()
    {
        if (!visualizeGrid) return;
        if (generator?.grid == null) return;

        float size = 0.4f;

        for (int x = 0; x < generator.width; x++)
            for (int z = 0; z < generator.depth; z++)
            {
                Tile tile = generator.grid[x, z];
                Vector3 center = new Vector3(x, yOffset, z);

                if (z == 0)
                {
                    Gizmos.color = Color.yellow;
                }
                else if (z == generator.startNextGeneration)
                {
                    Gizmos.color = Color.blue;
                }
                else{
                    Gizmos.color = Color.white;
                }

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
