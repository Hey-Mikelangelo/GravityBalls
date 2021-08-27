using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] private List<TypePrefabPool> pools = new List<TypePrefabPool>();
    public static Pool Instance { get; private set; }

    public PrefabPool GetPool(SpawnableType spawnableType)
    {
        foreach (TypePrefabPool pool in pools)
        {
            if(pool.spawnableType == spawnableType)
            {
                return pool.prefabPool;
            }
        }
        return null;
    }

    private void Awake()
    {
        Instance = this;   
    }
}
