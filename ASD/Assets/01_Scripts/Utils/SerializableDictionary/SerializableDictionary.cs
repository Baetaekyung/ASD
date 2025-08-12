using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SerializableKeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;

    public SerializableKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<SerializableKeyValuePair<TKey, TValue>> keyValuePairList
        = new();

    public void OnBeforeSerialize()
    {
        if (this.Count < keyValuePairList.Count) return;

        keyValuePairList.Clear();

        foreach(var kvp in this)
            keyValuePairList.Add(new SerializableKeyValuePair<TKey, TValue>(kvp.Key, kvp.Value));
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        foreach (var kvp in keyValuePairList)
        {
            if(this.ContainsKey(kvp.Key))
                continue;

            this.TryAdd(kvp.Key, kvp.Value);
        }
    }
}
