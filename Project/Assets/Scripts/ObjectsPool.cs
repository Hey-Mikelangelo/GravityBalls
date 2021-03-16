using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectsPool", menuName = "Game/ObjectsPool")]
public class ObjectsPool : ScriptableObject
{
    public GameObject objectToSpawn;
    public int poolSize = 10;

    private bool _initedObjectPool;
    private Stack<GameObject> ObjectPool;


    public void InitObjectsPool()
    {
        if (_initedObjectPool)
        {
            return;
        }
        ObjectPool = new Stack<GameObject>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            GameObject spawnedObject = Instantiate(objectToSpawn);
            spawnedObject.SetActive(false);
            ObjectPool.Push(spawnedObject);
        }
    }
    /// <summary>
    /// returns enabled GO
    /// </summary>
    /// <param name="resetTransform"></param>
    /// <returns></returns>
    public GameObject GetObject(bool resetScale = true, bool returnEnabled = true)
    {
        GameObject objectToReturn;
        //if there is pre-instantiated GameObject - just enable it and return
        if (ObjectPool.Count > 0)
        {
            objectToReturn = ObjectPool.Pop();
            if (returnEnabled)
            {
                objectToReturn.SetActive(true);
            }
        }
        else
        {
            objectToReturn = Instantiate(objectToSpawn);
            if (!returnEnabled)
            {
                objectToReturn.SetActive(false);
            }
        }
        if (resetScale)
        {
            objectToReturn.transform.localScale = new Vector3(1, 1, 1);
        }
        return objectToReturn;
    }

    public void ReturnToThePool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        ObjectPool.Push(objectToReturn);
    }
}
