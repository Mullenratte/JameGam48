using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    Animation anim;

    private void Awake() {
        anim = GetComponent<Animation>();
    }

    private void Update() {
        if (anim.isPlaying) return;

        Destroy(gameObject);
    }
}
