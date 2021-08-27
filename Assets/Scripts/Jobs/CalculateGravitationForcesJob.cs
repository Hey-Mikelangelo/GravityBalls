using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public partial class GravitationalAttractionBehavior
{
    private struct CalculateGravitationForcesJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> rigidbodiesPositions;
        [ReadOnly] public NativeArray<float> rigidbodiesMasses;
        [ReadOnly] public float3 thisRigidbodyPosition;
        [ReadOnly] public float thisRigidbodyMass;
        [ReadOnly] public float gravityMultiplier;
        public NativeArray<float3> resultingAttractionForces;

        public void Execute(int index)
        {
            float3 otherRbPosition = rigidbodiesPositions[index];
            float3 thisRbPosition = thisRigidbodyPosition;

            if (otherRbPosition.Equals(thisRbPosition))
            {
                resultingAttractionForces[index] = new float3(0, 0, 0);
                return;
            }

            float3 directionToOtherRb = otherRbPosition - thisRbPosition;
            float distance = math.length(directionToOtherRb);

            float otherMass = rigidbodiesMasses[index];

            float forceMagnitude = gravityMultiplier *
                (thisRigidbodyMass * otherMass) / (distance * distance);


            float3 force = math.normalize(directionToOtherRb) * forceMagnitude;

            resultingAttractionForces[index] = force;
        }
    }
}
