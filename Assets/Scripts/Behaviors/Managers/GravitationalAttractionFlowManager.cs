using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class GravitationalAttractionFlowManager : MonoBehaviour
{
    public static GravitationalAttractionFlowManager Instance { get; private set; }

    [SerializeField] private int targetBehavioursCount = 250;

    public int BehaviorsCount => behaviorsCount;
    public NativeArray<float> Masses => masses;
    public NativeArray<float3> Positions => positions;

    private int behaviorsCount;
    private List<GravitationalAttractionBehavior> gravitationalAttractionBehaviors = new List<GravitationalAttractionBehavior>();
    private List<GravitationalAttractionBehavior> pendingToRemoveBehaviors = new List<GravitationalAttractionBehavior>(50);
    private List<GravitationalAttractionBehavior> pendingToAddBehaviors = new List<GravitationalAttractionBehavior>(50);

    private NativeArray<float> masses;
    private NativeArray<float3> positions;

    private void Awake()
    {
        gravitationalAttractionBehaviors.Capacity = targetBehavioursCount;

        if (Instance != null)
        {
            Debug.Log($"Instance of {this} is already set");
            this.enabled = false;
            return;
        }
        Instance = this;
    }

    private void OnDisable()
    {
        CompleteJobs();
        DisposeJobArrays();
    }

    private void Update()
    {
        CompleteJobs();
        AddForces();
        DisposeJobArrays();
        RegisterSpawnedBehaviors();
        RemoveDestroyedBehaviors();
        behaviorsCount = gravitationalAttractionBehaviors.Count;
        FillJobArrays();
        CreateJobs();
        SheduleJobs();
    }

    public void OnSpawn(GravitationalAttractionBehavior gravitationalAttractionBehavior)
    {
        if (pendingToAddBehaviors.Contains(gravitationalAttractionBehavior) == false 
            && gravitationalAttractionBehavior.enabled)
        {
            pendingToAddBehaviors.Add(gravitationalAttractionBehavior);
        }
    }

    public void BeforeDestroy(GravitationalAttractionBehavior gravitationalAttractionBehavior)
    {
        if (pendingToRemoveBehaviors.Contains(gravitationalAttractionBehavior) == false)
        {
            pendingToRemoveBehaviors.Add(gravitationalAttractionBehavior);
        }
    }

    private void RegisterSpawnedBehaviors()
    {
        if (pendingToAddBehaviors.Count > 0)
        {
            foreach (var item in pendingToAddBehaviors)
            {
                if (!gravitationalAttractionBehaviors.Contains(item))
                {
                    gravitationalAttractionBehaviors.Add(item);
                }
            }
        }
        pendingToAddBehaviors.Clear();
    }

    private void RemoveDestroyedBehaviors()
    {
        if (pendingToRemoveBehaviors.Count > 0)
        {
            foreach (var item in pendingToRemoveBehaviors)
            {
                gravitationalAttractionBehaviors.Remove(item);
            }
        }
        pendingToRemoveBehaviors.Clear();
    }

    private void FillJobArrays()
    {
        masses = new NativeArray<float>(behaviorsCount, Allocator.TempJob);
        positions = new NativeArray<float3>(behaviorsCount, Allocator.TempJob);
        for (int i = 0; i < behaviorsCount; i++)
        {
            LightWeightRigidbody rb = gravitationalAttractionBehaviors[i].Rb;
            masses[i] = rb.Mass;
            positions[i] = rb.position;
        }
    }

    private void DisposeJobArrays()
    {
        if (masses.IsCreated)
        {
            masses.Dispose();
        }
        if (positions.IsCreated)
        {
            positions.Dispose();
        }

    }

    private void CompleteJobs()
    {
        int count = gravitationalAttractionBehaviors.Count;
        for (int i = 0; i < count; i++)
        {
            gravitationalAttractionBehaviors[i].CompleteJobs();
        }
    }

    private void AddForces()
    {
        int count = gravitationalAttractionBehaviors.Count;
        for (int i = 0; i < count; i++)
        {
            gravitationalAttractionBehaviors[i].AddForces();
        }
    }
    
    private void CreateJobs()
    {
        int count = gravitationalAttractionBehaviors.Count;
        for (int i = 0; i < count; i++)
        {
            gravitationalAttractionBehaviors[i].CreateJobs();
        }
    }

    private void SheduleJobs()
    {
        int count = gravitationalAttractionBehaviors.Count;
        for (int i = 0; i < count; i++)
        {
            gravitationalAttractionBehaviors[i].SheduleJobs();
        }
    }
}
