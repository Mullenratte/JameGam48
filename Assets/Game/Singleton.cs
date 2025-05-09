using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance;

    protected virtual void Awake() {
        if (instance == null) {
            instance = this as T;
        } else {
            Destroy(gameObject);
        }
    }

}
