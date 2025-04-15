using System;
using UnityEngine;

public class Item_TimeStopper : BaseItem {

    [SerializeField] ItemConfigSO_TimeStopper config;

    public static event EventHandler<float> OnActionTriggered;
    public override void TriggerOnLickedAction() {
        base.TriggerOnLickedAction();
        OnActionTriggered?.Invoke(this, config.duration);
    }

}
