using UnityEngine;

public class Inventory : MonoBehaviour {

    ItemData _heldItem;


    private void Start() {
        InputHandler.OnUseItem += UseItem;
        BaseItem.OnCollected += BaseItem_OnCollected;
    }

    private void BaseItem_OnCollected(ItemData data) {
        UseItem();
        AddItem(data);
        Debug.Log(_heldItem);
    }

    private void Update() {
        //Debug.Log(_heldItem);
    }


    private void UseItem() {
        if (_heldItem == null) return;
        _heldItem.effect.Activate();
        RemoveItem();
    }


    public void AddItem(ItemData itemData) {
        _heldItem = itemData;
    }

    public void RemoveItem() {
        _heldItem = null;
    }
}
