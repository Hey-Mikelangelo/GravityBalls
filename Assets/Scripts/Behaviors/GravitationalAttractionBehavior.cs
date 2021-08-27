using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(LightWeightRigidbody)), DisallowMultipleComponent]
public partial class GravitationalAttractionBehavior : MonoBehaviour
{
    [SerializeField] private float localGravityMultiplier = 1;

    public LightWeightRigidbody Rb => rigidbody;
    public static float GlobalGravityMultilplier { get; set; } = 1;

    private int batchSize = 128;
    private CalculateGravitationForcesJob calculateGravitationForcesJob = new CalculateGravitationForcesJob();
    private JobHandle jobHandle;
    private float3 calculatedForce;
    private new LightWeightRigidbody rigidbody;
    private GravitationalAttractionFlowManager flowManager;
    private NativeArray<float3> resultForces;

    public void BeforeReturnToPool()
    {
        calculatedForce = float3.zero;
    }

    public void OnSpawnFromPool()
    {
        rigidbody.Velocity = Vector3.zero;
    }


    private void Awake()
    {
        rigidbody = GetComponent<LightWeightRigidbody>();
        OnSpawnFromPool();
    }

    private void OnDestroy()
    {
        BeforeReturnToPool();
    }

    private void OnEnable()
    {
        StartCoroutine(WaitOneFrame(OnEnableActions));
    }

    private void OnDisable()
    {
        if(flowManager != null)
        {
            flowManager.BeforeDestroy(this);
        }
   
        CompleteJobs();
    }

    private void OnEnableActions()
    {
        flowManager = GravitationalAttractionFlowManager.Instance;
        flowManager.OnSpawn(this);
    }

    private IEnumerator WaitOneFrame(System.Action action)
    {
        yield return null;
        action();
    }

    internal void CreateJobs()
    {
        resultForces = new NativeArray<float3>(flowManager.BehaviorsCount, Allocator.TempJob);
        calculateGravitationForcesJob.rigidbodiesMasses = flowManager.Masses;
        calculateGravitationForcesJob.rigidbodiesPositions = flowManager.Positions;
        calculateGravitationForcesJob.thisRigidbodyMass = rigidbody.Mass;
        calculateGravitationForcesJob.thisRigidbodyPosition = rigidbody.position;
        calculateGravitationForcesJob.gravityMultiplier = localGravityMultiplier * GlobalGravityMultilplier;
        calculateGravitationForcesJob.resultingAttractionForces = resultForces;
    }

    internal void SheduleJobs()
    {
        jobHandle = calculateGravitationForcesJob.Schedule(flowManager.BehaviorsCount, batchSize);
    }

    internal void CompleteJobs()
    {
        jobHandle.Complete();

        if (resultForces.IsCreated)
        {
            int forcesCount = resultForces.Length;
            calculatedForce = float3.zero;

            for (int i = 0; i < forcesCount; i++)
            {
                calculatedForce += resultForces[i];
            }
            if (resultForces.IsCreated)
            {
                resultForces.Dispose();
            }
        }
    }

    internal void AddForces()
    {
        rigidbody.AddForce(calculatedForce);
    }
}
