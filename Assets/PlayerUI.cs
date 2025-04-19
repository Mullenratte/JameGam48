using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] Image storedItemImage;
    [SerializeField] TextMeshProUGUI storedItemNameText;

    [SerializeField] Sprite nullItemSprite;

    private void Start() {
        Inventory.Instance.OnItemCollected += Inventory_OnItemCollected;
        Inventory.Instance.OnItemUsed += Inventory_OnItemUsed;

        ClearStoredItem();
    }

    private void Inventory_OnItemUsed() {
        ClearStoredItem();
    }

    private void Inventory_OnItemCollected(ItemData obj) {
        SetStoredItem(obj);
    }

    void SetStoredItem(ItemData data) {
        storedItemImage.sprite = data.icon;
        storedItemNameText.text = data.itemName;
    }

    void ClearStoredItem() {
        storedItemImage.sprite = nullItemSprite;
        storedItemNameText.text = string.Empty;
    }
}
