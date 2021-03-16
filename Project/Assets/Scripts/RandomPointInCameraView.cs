using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class RandomPointInCameraView : MonoBehaviour
{
    public Camera cam;
    [Range(1, 100)]
    public float minDistance;
    [Range(1, 100)]
    public float maxDistance;
    public float checkSurroundingsRadius = 1;
    public int maxCheckCount = 5;

    private Collider[] _colliders = new Collider[1];
    public Vector3 GetPoint()
    {
        if (maxDistance < minDistance)
        {
            maxDistance = minDistance;
        }

        Vector2 viewportRandPoint;
        Vector3 pointPos = cam.transform.position + cam.transform.forward * maxDistance;
        Ray ray;

        int checkCount = 0;
        //try several times to find position without any obstructions 
        for (int i = 0; i < maxCheckCount; i++)
        {
            viewportRandPoint = MathHelper.GetRandom2(0,1);

            ray = cam.ViewportPointToRay(viewportRandPoint, Camera.MonoOrStereoscopicEye.Mono);

            //Debug.DrawRay(ray.origin, ray.direction * 10);

            pointPos = ray.origin + ray.direction * MathHelper.GetRandom(minDistance, maxDistance);

            //if there is no obstructions in radius or tried to find valid point too many times 
            //- return current point as valid
            if (Physics.OverlapSphereNonAlloc(pointPos, checkSurroundingsRadius, _colliders) <= 0 
                || checkCount >= maxCheckCount)
            {
                //check if point in not obstructed with another object from camera view
                Ray cameraToPointRay = new Ray(cam.transform.position, pointPos - cam.transform.position);
                if (Physics.Raycast(cameraToPointRay) && checkCount < maxCheckCount)
                {
                    continue;
                }
                else
                { 
                    return pointPos;
                }
            }
            else
            {
                //obstruction found
            }
            checkCount++;
        }
        return pointPos;
    }


}
