using UnityEngine;

public class MathHelper 
{
    public static Vector2 GetRandomVector2(float min, float max)
    {
        return new Vector2(
            Random.Range(min, max),
            Random.Range(min, max));
    }

    public static Vector3 GetRandomVector3(float absMin, float absMax)
    {
        return new Vector3(
            GetRandom(absMin, absMax),
            GetRandom(absMin, absMax),
            GetRandom(absMin, absMax));
    }

    public static float GetRandom(float absMin, float absMax)
    {
        if (RandomBool())
        {
            return Random.Range(absMin, absMax);
        }else
        {
            return Random.Range(-absMax, -absMin);
        }
        
    }

    public static bool RandomBool()
    {
        return Random.Range(0, 2) == 1;
    }

}
