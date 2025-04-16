using System;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    public static event Action OnUseItem;

    private void Update() {

        if (Input.GetKeyDown(KeyCode.E)) {
            OnUseItem?.Invoke();
        }
    }
}
