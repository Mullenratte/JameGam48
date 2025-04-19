using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    Animator anim;
    [SerializeField] float basePlaybackSpeed;
    float playbackSpeed;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void Start() {
            
    }

    private void Update() {
        playbackSpeed = basePlaybackSpeed + basePlaybackSpeed * PlayerMovement.Instance.GetEffectiveMoveSpeed();
        anim.speed = playbackSpeed;
    }



}
