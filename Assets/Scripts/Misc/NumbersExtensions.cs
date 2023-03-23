using System.Runtime.CompilerServices;
using UnityEngine;

namespace Misc
{
    public class UInt32
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(UInt32 value)
        {
            return (int)value;
        }
    }
    
    public class Int32
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(Int32 value)
        {
            return (uint)value;
        }
    }

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
}