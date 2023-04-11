using Unity.Mathematics;
using UnityEngine;

public static class NumbersExtensions
{
    public enum Overlapping
    {
        Less = -1,
        None = 0,
        Greater = 1
    }
    
    public static Overlapping IsOverlapping(this float value, float min, float max)
    {
        if(value < min) return Overlapping.Less;
        if(value > max) return Overlapping.Greater;
        return Overlapping.None;
    }
       

    public static bool IsOverlapping(this float value, float min, float max, out float clampedValue)
    {
        clampedValue = math.clamp(value, min, max);
        return math.abs(clampedValue - value) > 0.0001;
    } 
}

public static class BoolExtensions
{
    public static bool IsFalse(this bool boolean) =>
        boolean == false;
        
    public static bool IsTrue(this bool boolean) =>
        boolean == true;
}

public struct Range
{
    public Range(Vector2 minMax)
    {
        Minimum = minMax.x;
        Maximum = minMax.y;
    }
    
    public Range(float min, float max)
    {
        Minimum = min;
        Maximum = max;
    }

    //is as идут нахуй
    public static implicit operator Range(Vector2 asMinMax) => new Range(asMinMax);//типа можно передать в него Vector2
    public static implicit operator Vector2(Range range) => new Vector2(range.Minimum, range.Maximum); //или получить как Vector2
    
    public float Minimum;
    public float Maximum;
}