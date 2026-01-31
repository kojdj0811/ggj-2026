using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance
    {
        get;
        protected set;
    }
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Singleton {typeof(T).Name} has been instantiated more thn once.", this);
            Destroy(gameObject);
            return;
        }

        Instance = (T)this;
        InternalAwake();
    }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    protected virtual void InternalAwake() { }
}