using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravitationalAttraction : MonoBehaviour
{
    public static List<Rigidbody> AttractingRigidbodies = new List<Rigidbody>();
    public float gravityMultiplier = 1;
    private static float _globalGravityMultiplier = 1;
    private Rigidbody _thisRb;
    private void OnEnable()
    {
        _thisRb = GetComponent<Rigidbody>();
        AttractingRigidbodies.Add(_thisRb);
    }
    private void OnDisable()
    {
        AttractingRigidbodies.Remove(_thisRb);
    }
    void FixedUpdate()
    {
        SimulateAttraction();
    }
    public static void SetGlobalGravityMultiplier(float mult)
    {
        _globalGravityMultiplier = mult;
    }
    void SimulateAttraction()
    {
        for (int i = 0; i < AttractingRigidbodies.Count; i++)
        {
            if(AttractingRigidbodies[i] != _thisRb)
            {
                Attract(AttractingRigidbodies[i]);
            }
        }
    }
    void Attract(Rigidbody otherRb)
    {
        if (otherRb == null || otherRb.position == transform.position)
        {
            return;
        }
        Vector3 directionToThisRb = _thisRb.position - otherRb.position;
        float distance = directionToThisRb.magnitude;

        float forceMagnitude = _globalGravityMultiplier * gravityMultiplier * 
            (_thisRb.mass * otherRb.mass) / (distance * distance);

        Vector3 forceDirection = directionToThisRb.normalized * forceMagnitude;
        otherRb.AddForce(forceDirection);
    }
}
