using UnityEngine;

public static class NumbersExtensions
{
    public static float ClampAngle(float angle, float from, float to)
    {
        if (angle < 0f) 
            angle = 360 + angle;
            
        return angle > 180f ? 
            Mathf.Max(angle, 360 + from) : 
            Mathf.Min(angle, to);
    }
}

public static class BoolExtensions
{
    public static bool IsFalse(this bool boolean) =>
        boolean == false;
        
    public static bool IsTrue(this bool boolean) =>
        boolean == true;
}