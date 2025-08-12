using System;
using System.Reflection;
using UnityEngine;

[Flags]
public enum SingletonFlag
{
    None = 1 << 0,

    DontDestroy = 1 << 1,
    HideInHierarchy = 1 << 2,

    All = DontDestroy | HideInHierarchy
}

public class SingletonAttribute : Attribute
{
    public SingletonFlag flag;

    public SingletonAttribute(SingletonFlag flag)
    {
        this.flag = flag;
    }
}

public abstract class MonoSingleton<T> : MonoBehaviour 
    where T : MonoBehaviour
{
    #region Instance
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_onApplicationQuitted)
                return null;

            if (_isDestroyed)
                return null;

            if (_instance == null)
                _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);

            if (_instance == null)
            {
                _instance = new GameObject($"[Runtime][Singleton][{typeof(T).Name}]").AddComponent<T>();
                DebugEx.CLog($"Make runtime singleton, [{typeof(T).Name}]", "green");
            }

            if (_instance == null)
                DebugEx.CLog("Singleton instance is null", "red");

            return _instance;
        }
    }
    #endregion

    #region Condition
    private static bool _onApplicationQuitted = false;
    private static bool _isDestroyed = false;
    #endregion

    #region Unity LifeCycle
    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            DebugEx.CLog("Two singleton, one singleton removed.", "yellow");
            Destroy(_instance.gameObject);

            return;
        }

        if (_instance == null)
            _instance = this as T;

        var attribute = GetType().GetCustomAttribute<SingletonAttribute>();
        if (attribute != null)
        {
            string attributeName = "";
            if (attribute.flag.HasFlag(SingletonFlag.DontDestroy))
            {
                DontDestroyOnLoad(gameObject);
                attributeName += "DontDestroy & ";
            }
            if (attribute.flag.HasFlag(SingletonFlag.HideInHierarchy))
            {
                gameObject.hideFlags = HideFlags.HideInHierarchy;
                attributeName += "HideInHierarchy";
            }

            DebugEx.CLog($"Singleton attribute applied [{typeof(T).Name}], {attributeName}", "green");
        }

        DebugEx.CLog($"Singleton initialized [{typeof(T).Name}]", "green");
    }

    protected virtual void OnApplicationQuit()
    {
        _onApplicationQuitted = true;
        _instance = null;
    }

    protected virtual void OnDestroy()
    {
        _isDestroyed = true;
        _instance = null;
    }
    #endregion
}
