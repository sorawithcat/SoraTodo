using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<Tkey, Tvalue> : Dictionary<Tkey, Tvalue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<Tkey> keys = new List<Tkey>();
    [SerializeField] private List<Tvalue> values = new List<Tvalue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<Tkey, Tvalue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        if (keys.Count != values.Count)
        {
            //Debug.Log("键数不等于值数");
        }
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

    public void RemoveByKey(Tkey keyToRemove)
    {
        Dictionary<Tkey, Tvalue> newDictionary = new Dictionary<Tkey, Tvalue>();

        foreach (KeyValuePair<Tkey, Tvalue> pair in this)
        {
            if (!EqualityComparer<Tkey>.Default.Equals(pair.Key, keyToRemove))
            {
                newDictionary.Add(pair.Key, pair.Value);
            }
        }

        this.Clear();
        foreach (KeyValuePair<Tkey, Tvalue> pair in newDictionary)
        {
            this.Add(pair.Key, pair.Value);
        }
    }
}