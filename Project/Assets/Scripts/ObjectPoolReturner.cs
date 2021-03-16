using UnityEngine;


/// <summary>
/// On disable returns object to linked object spawner pool
/// </summary>
public class ObjectPoolReturner : MonoBehaviour
{
    public ObjectsPool objectsSpawner;
    [HideInInspector] public bool canReturn = false;

    private void OnEnable()
    {
        canReturn = false;
    }
    private void OnDisable()
    {
        if (canReturn)
        {
            objectsSpawner.ReturnToThePool(gameObject);

        }
    }
}
