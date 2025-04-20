using System;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public static Inventory Instance;
    ItemData _heldItem;
    public event Action<ItemData> OnItemCollected;
    public event Action OnItemUsed;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        InputHandler.OnUseItem += UseItem;
        BaseItem.OnCollected += BaseItem_OnCollected;
    }

    private void OnDestroy() {
        InputHandler.OnUseItem -= UseItem;
        BaseItem.OnCollected -= BaseItem_OnCollected;
    }

    private void BaseItem_OnCollected(ItemData data) {
        UseItem();
        AddItem(data);
        OnItemCollected?.Invoke(data);
    }

    private void Update() {
        //Debug.Log(_heldItem);
    }


    private void UseItem() {
        if (_heldItem == null) return;
        _heldItem.effect.Activate();
        RemoveItem();
        OnItemUsed?.Invoke();
    }


    public void AddItem(ItemData itemData) {
        _heldItem = itemData;
    }

    public void RemoveItem() {
        _heldItem = null;
    }

    public ItemData GetItemData() {
        return _heldItem;
    }
}
