using System;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public static Inventory Instance;
    ItemData _heldItem;
    public event Action<ItemData> OnItemCollected;
    public event Action OnItemUsed;

    [SerializeField] AudioClip AnyItemActivatedClip;
    [SerializeField] AudioClip AnyItemCollectedClip;

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
        SoundFXManager.instance.PlaySoundFXClip(AnyItemCollectedClip, transform.position, 0.5f, 1f, 1f);
    }

    private void Update() {
        //Debug.Log(_heldItem);
    }


    private void UseItem() {
        if (_heldItem == null) return;
        _heldItem.effect.Activate();
        OnItemUsed?.Invoke();

        switch (_heldItem.name) {
            case "Jump":
                CinemachineCameraShake.Instance.ScreenshakeDefault(.3f, 25f);

                break;
            case "Speed Boost":
                CinemachineCameraShake.Instance.ScreenshakeDefault(((Effect_SpeedBoost)_heldItem.effect).config.duration, 5f);

                break;
            case "Time Stopper":
                CinemachineCameraShake.Instance.ScreenshakeDefault(0.1f, 8f);

                break;
            default:
                break;
        }

        SoundFXManager.instance.PlaySoundFXClip(AnyItemActivatedClip, transform.position, 0.35f, 1f, 1f);
        RemoveItem();

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
