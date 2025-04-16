using System;
using UnityEngine;

public class Item_TimeStopper : BaseItem {

    public static event EventHandler<float> OnActionTriggered;
    public override void TriggerOnCollectedAction() {
        base.TriggerOnCollectedAction();
    }
}
