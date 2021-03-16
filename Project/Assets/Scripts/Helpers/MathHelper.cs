using UnityEngine;

public class MathHelper 
{
    public static Vector2 GetRandom2(Vector2 minValues, Vector2 maxValues)
    {
        return new Vector2(Random.Range(minValues.x, maxValues.x), Random.Range(minValues.y, maxValues.y));
    }
    public static Vector2 GetRandom2(float min, float max)
    {
        return new Vector2(
            Random.Range(min, max),
            Random.Range(min, max));
    }
    public static Vector3 GetRandom3(float min, float max)
    {
        return new Vector3(
            Random.Range(min, max),
            Random.Range(min, max),
            Random.Range(min, max));
    }
    public static float GetRandom(float minValue, float maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}
