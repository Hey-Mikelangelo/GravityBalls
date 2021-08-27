using UnityEngine;

public class RandomPointInCameraView
{
    public int maxCheckCount = 5;

    private Collider[] obstructionColliders = new Collider[1];
    public Vector3 GetPoint(Camera cam, float minDistance, float maxDistance, 
        float checkSurroundingsRadius = 1, int maxCheckCount = 5)
    {
        if (maxDistance < minDistance)
        {
            maxDistance = minDistance;
        }

        Vector2 viewportRandPoint;
        Vector3 pointPos = cam.transform.position + cam.transform.forward * maxDistance;
        
        int checkCount = 0;
        //try several times to find position without any obstructions 
        for (int i = 0; i < maxCheckCount; i++)
        {
            viewportRandPoint = MathHelper.GetRandomVector2(0,1);

            Ray ray = cam.ViewportPointToRay(viewportRandPoint, Camera.MonoOrStereoscopicEye.Mono);
            pointPos = ray.origin + ray.direction * Random.Range(minDistance, maxDistance);

            //if there is no obstructions in radius or tried to find valid point too many times 
            //- return current point as valid
            if (Physics.OverlapSphereNonAlloc(pointPos, checkSurroundingsRadius, obstructionColliders) <= 0 
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
            checkCount++;
        }
        return pointPos;
    }


}
