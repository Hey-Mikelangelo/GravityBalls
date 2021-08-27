using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Poolable : MonoBehaviour
{
    private PrefabPool prefabPool;

    private static List<GameObject> gameObjectsToDestroyAfterFixedUpdate = new List<GameObject>();
    private static List<GameObject> gameObjectsToDestroyAfterUpdate = new List<GameObject>();
    private static bool isDestroyedObjectsAfterUpdate;
    private static bool isDestroyedObjectsAfterFixedUpdate;
    private static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    public PrefabPool PrefabPool { 
        get
        {
            return prefabPool;
        }
        set
        {
            if(prefabPool == null)
            {
                prefabPool = value;
            }
            else
            {
                Debug.LogError("Pool should not be set more than once");
            }
        }
    }

    private IPoolable[] poolableComponents;

    public static void DestroyPoolableDelayed(GameObject gameObjectToDestroy, bool afterFixedUpdate)
    {
        if (afterFixedUpdate)
        {
            gameObjectsToDestroyAfterFixedUpdate.Add(gameObjectToDestroy);
        }
        else
        {
            gameObjectsToDestroyAfterUpdate.Add(gameObjectToDestroy);
        }
    }

    public static void DestroyPoolable(GameObject gameObjectToDestroy)
    {
        if (gameObjectToDestroy.TryGetComponent(out Poolable poolable))
        {
            poolable.ReturnToPool();
        }
        else
        {
            GameObject.Destroy(gameObjectToDestroy);
        }
    }

    public void BeforeReturn()
    {
        for (int i = 0; i < poolableComponents.Length; i++)
        {
            poolableComponents[i].BeforeReturnToPool();
        }
        gameObject.SetActive(false);
    }

    public void OnSpawn(bool returnEnabled)
    {
        for (int i = 0; i < poolableComponents.Length; i++)
        {
            poolableComponents[i].OnSpawnFromPool();
        }
        gameObject.SetActive(returnEnabled);
    }
    public void ReturnToPool()
    {
        if(prefabPool == null)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            prefabPool.ReturnObject(this);
        }
    }

    private void Awake()
    {
        poolableComponents = GetComponentsInChildren<IPoolable>();   
    }

    private void OnEnable()
    {
        StartCoroutine(LateFixedUpdate());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        isDestroyedObjectsAfterUpdate = false;
    }

    private void FixedUpdate()
    {
        isDestroyedObjectsAfterFixedUpdate = false;
    }

    private void LateUpdate()
    {
        if (!isDestroyedObjectsAfterUpdate)
        {
            isDestroyedObjectsAfterUpdate = true;
            foreach (var item in gameObjectsToDestroyAfterUpdate)
            {
                DestroyPoolable(item);
            }
            gameObjectsToDestroyAfterUpdate.Clear();
        }
    }

    private IEnumerator LateFixedUpdate()
    {
        while (enabled)
        {
            yield return waitForFixedUpdate;
            if (!isDestroyedObjectsAfterFixedUpdate)
            {
                isDestroyedObjectsAfterFixedUpdate = true;
                foreach (var item in gameObjectsToDestroyAfterFixedUpdate)
                {
                    DestroyPoolable(item);
                }
                gameObjectsToDestroyAfterFixedUpdate.Clear();
            }
        }
    }

}
    