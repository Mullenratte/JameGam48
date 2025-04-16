using System;

public class Item_Jump : BaseItem {


    public static event EventHandler<ItemConfigSO_Jump> OnActionTriggered;
    public override void TriggerOnCollectedAction() {
        base.TriggerOnCollectedAction();
    }
}
