using UnityEngine;

[CreateAssetMenu(fileName = "ItemConfigSO_Jump", menuName = "Scriptable Objects/ItemConfigSO_Jump")]
public class ItemConfigSO_Jump : ScriptableObject {
    public float speedMultiplier;
    public int maxTiles = 5;
}