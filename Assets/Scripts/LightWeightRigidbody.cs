using UnityEngine;

public class LightWeightRigidbody : MonoBehaviour
{
    [SerializeField] private float mass;
    [SerializeField] private float drag;

    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity { get => velocity; set => velocity = value; }
    public float Mass { get => mass; set => mass = value; }
    public Vector3 position { get => transform.position; }

    public void AddForce(Vector3 force)
    {
        if(Mass <= 0)
        {
            Debug.LogError("Mass is <= 0");
            return;
        }
        Velocity += (force / Mass) * Time.deltaTime;
    }
    private void FixedUpdate()
    {
        transform.position += Velocity * Time.fixedDeltaTime;
        Velocity = Velocity * (1 - Time.fixedDeltaTime * drag);
    }
}
