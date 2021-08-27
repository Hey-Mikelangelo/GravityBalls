using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider)), DisallowMultipleComponent]
public class GlobalBallParamsChanger : MonoBehaviour
{
    private static List<Collider> colliders = new List<Collider>();
    private static List<Rigidbody> rigidbodies = new List<Rigidbody>();

    private new Collider collider;
    private new Rigidbody rigidbody;
    private void Awake()
    {
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
        colliders.Add(collider);
        rigidbodies.Add(rigidbody);
    }

    private void OnDestroy()
    {
        colliders.Remove(collider);
        rigidbodies.Remove(rigidbody);
    }

    public static void SetCollidersIsTrigger(bool value)
    {
        foreach (var collider in colliders)
        {
            collider.isTrigger = value;
        }
    }

    public static void SetRigidbodiesKinematic(bool value)
    {
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = value;
        }
    }
}
