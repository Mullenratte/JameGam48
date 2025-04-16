using UnityEngine;

[CreateAssetMenu(fileName = "ItemConfigSO_SpeedBoost", menuName = "Scriptable Objects/ItemConfigSO_SpeedBoost")]
public class ItemConfigSO_SpeedBoost : ScriptableObject {
    public float duration;
    public float speedMultiplier;
}
