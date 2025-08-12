using System.Collections.Generic;

public class Pool<T> : IPool<T>
    where T : IPoolable, new()
{
    public Queue<T> pool;

    private int currentSize = 0;
    private int growSize    = 0;
    private int maxSize     = 0;

    public void Initialize(int defaultSize, int growSize, int maxSize)
    {
        pool = new Queue<T>();

        this.growSize   = growSize;
        this.maxSize    = maxSize;
        currentSize     = defaultSize;

        Grow(defaultSize);
    }

    public T Get()
    {
        bool flag = false;

        if (pool.Count == 0)
            flag = Grow(growSize);

        // Grow ½ÇÆÐ
        if (flag)
        {
            DebugEx.CLog($"{typeof(T).Name}: Fail to get poolable", "red");
            return default;
        }

        var obj = pool.Dequeue();
        obj.OnGet();
        return obj;
    }

    public void Return(T obj)
    {
        obj.OnReturn();
        pool.Enqueue(obj);
    }

    private bool Grow(int growSize)
    {
        if (currentSize + growSize > maxSize)
        {
            DebugEx.CLog($"{typeof(T)} pool can't resize size reach to maxSize", "red");
            return false;
        }

        for (int i = 0; i < growSize; i++)
        {
            pool.Enqueue(MakeInstance());
        }

        DebugEx.CLog($"Type[{typeof(T)}] pool growed, ++{growSize}", "green");
        currentSize += growSize;
        return true;
    }

    private T MakeInstance()
    {
        T instance = new T();

        return instance;
    }
}

public interface IPool<T>
{
    T Get();
    void Return(T obj);
}

public interface IPoolable
{
    void OnGet();
    void OnReturn();
}

