using System.Collections.Generic;
using UnityEngine;

public class PrefabPool : MonoBehaviour
{
    [SerializeField] private Poolable poolablePrefab;
    [SerializeField] private int initialInstances;
    [SerializeField] private Queue<Poolable> readyElements = new Queue<Poolable>();

    private void Awake()
    {
        for (int i = 0; i < initialInstances; i++)
        {
            var instance = CreateNewInstance();
            instance.BeforeReturn();
            readyElements.Enqueue(instance);
        }
    }

    public void ReturnObject(Poolable poolable)
    {
        poolable.BeforeReturn();
        readyElements.Enqueue(poolable);
    }

    public Poolable GetInstance(bool returnEnabled = true)
    {
        Poolable instance;
        if (readyElements.Count == 0)
        {
            instance = CreateNewInstance();
        }
        else
        {
            instance = readyElements.Dequeue();
        }
        instance.OnSpawn(returnEnabled);
        return instance;
    }

    private Poolable CreateNewInstance()
    {
        Poolable poolable = Object.Instantiate(poolablePrefab);
        poolable.PrefabPool = this;
        poolable.transform.parent = transform;
        return poolable;
    }

}
