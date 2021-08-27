using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider), typeof(LightWeightRigidbody))]
public class MergeBallsBehavior : MonoBehaviour, IPoolable
{
    public static bool IsGloballyEnabled { get; set; } = true;

    private new LightWeightRigidbody rigidbody;
    private SphereCollider sphereCollider;
    private MergeBallsBehavior ballToMergeInto;
    private MergeBallsBehavior ballToMerge;
    private const float PI4 = Mathf.PI * 4;

    public void BeforeReturnToPool()
    {
        ballToMerge = null;
        ballToMergeInto = null;
        rigidbody.Mass = 1;
        rigidbody.Velocity = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    public void OnSpawnFromPool()
    {
        sphereCollider.enabled = true;
        rigidbody.Velocity = Vector3.zero;
    }

    private void Awake()
    {
        rigidbody = GetComponent<LightWeightRigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void FixedUpdate()
    {
        if(!IsGloballyEnabled)
        {
            return;
        }
        if (ballToMerge != null && ballToMergeInto == null)
        {
            MergeBalls();
        }
    }

    private bool IsMergedTo(MergeBallsBehavior otherBall)
    {
        return this.ballToMergeInto == otherBall;
    }

    private bool IsMergedIndirectlyTo(MergeBallsBehavior otherBall)
    {
        if (this.IsMergedTo(otherBall))
        {
            return true;
        }
        if (ballToMergeInto != null)
        {
            return ballToMergeInto.IsMergedIndirectlyTo(otherBall);
        }
        return false;
    }

    private void Merge(MergeBallsBehavior otherBall)
    {
        if (IsMergedIndirectlyTo(otherBall) || otherBall == this)
        {
            return;
        }
        if (ballToMerge != null)
        {
            ballToMerge.Merge(otherBall);
        }
        if (otherBall.ballToMergeInto != null)
        {
            Merge(otherBall.ballToMergeInto);
        }
        else
        {
            ballToMerge = otherBall;
            otherBall.ballToMergeInto = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }
        if (other.gameObject.TryGetComponent(out MergeBallsBehavior otherBall))
        {
            Merge(otherBall);
        }
    }

    private List<MergeBallsBehavior> GetGetBallsToMerge(List<MergeBallsBehavior> list = null)
    {
        if (ballToMerge == null)
        {
            return list;
        }
        if (list == null)
        {
            list = new List<MergeBallsBehavior>();
            list.Add(this);
        }
        list.Add(ballToMerge);
        return ballToMerge.GetGetBallsToMerge(list);
    }

    private void MergeBalls()
    {
        List<MergeBallsBehavior> ballsToMerge = GetGetBallsToMerge();
        for (int i = 0; i < ballsToMerge.Count; i++)
        {
            ballsToMerge[i].sphereCollider.enabled = false;
        }
        GetCenterOfMass(ballsToMerge,
            out Vector3 centerOfMass,
            out float massSum);
        float areaSum = GetSurfaceArea(ballsToMerge);
        Vector3 newVelocity = GetVelocityAfterCollision(ballsToMerge, massSum);
        float newRadius = Mathf.Sqrt(areaSum / PI4);
        float newScale = newRadius * 2;

        MergeBallsBehavior mainBall = this;

        mainBall.rigidbody.Mass = massSum;
        mainBall.transform.position = centerOfMass;
        mainBall.rigidbody.Velocity = newVelocity;

        //scale this ball to make surface area of main as equal to sum of surface areas of all merged balls
        mainBall.transform.localScale = new Vector3(newScale, newScale, newScale);

        //loop through all merged balls (0 index element is mainBall) and destroy them
        for (int i = 1; i < ballsToMerge.Count; i++)
        {
            MergeBallsBehavior otherBall = ballsToMerge[i];
            Poolable.DestroyPoolableDelayed(otherBall.gameObject, true);
        }
        mainBall.ballToMerge = null;
        mainBall.ballToMergeInto = null;
        mainBall.sphereCollider.enabled = true;

    }

    private Vector3 GetVelocityAfterCollision(List<MergeBallsBehavior> mergeBalls, float massSum)
    {
        Vector3 velocitySum = Vector3.zero;
        for (int i = 0; i < mergeBalls.Count; i++)
        {
            LightWeightRigidbody rb = mergeBalls[i].rigidbody;
            velocitySum += rb.Velocity * rb.Mass;
        }
        return velocitySum / massSum;
    }

    private float GetSurfaceArea(List<MergeBallsBehavior> mergeBalls)
    {
        float surfaceAreaSum = 0;
        for (int i = 0; i < mergeBalls.Count; i++)
        {
            float radius = mergeBalls[i].transform.localScale.x / 2;
            surfaceAreaSum += PI4 * radius * radius;
        }
        return surfaceAreaSum;
    }

    private void GetCenterOfMass(List<MergeBallsBehavior> mergeBalls, out Vector3 centerOfMass, out float massSum)
    {
        centerOfMass = new Vector3(0, 0, 0);
        massSum = 0;

        for (int i = 0; i < mergeBalls.Count; i++)
        {
            float mass = mergeBalls[i].rigidbody.Mass;
            Vector3 position = mergeBalls[i].rigidbody.position;
            centerOfMass += position * mass;
            massSum += mass;
        }

        centerOfMass /= massSum;
    }

    
}
