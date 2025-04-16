using System;
using UnityEngine;

public class Item_SpeedBoost : BaseItem {


    public static event EventHandler<ItemConfigSO_SpeedBoost> OnActionTriggered;
    public override void TriggerOnCollectedAction() {
        base.TriggerOnCollectedAction();
    }
}
