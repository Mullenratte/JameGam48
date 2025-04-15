using System;
using UnityEngine;

public class Item_SpeedBoost : BaseItem {

    [SerializeField] ItemConfigSO_SpeedBoost config;

    public static event EventHandler<ItemConfigSO_SpeedBoost> OnActionTriggered;
    public override void TriggerOnLickedAction() {
        base.TriggerOnLickedAction();
        OnActionTriggered?.Invoke(this, config);
    }

}
