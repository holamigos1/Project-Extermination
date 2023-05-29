using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Misc.Extensions
{
    public static class NumbersExtensions
    {
        public enum Overlapping
        {
            Less = -1,
            None = 0,
            Greater = 1
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Overlapping IsOverlapping(this float value, float min, float max)
        {
            if(value < min) 
                return Overlapping.Less;
            
            return value > max ?
                Overlapping.Greater :
                Overlapping.None;
        }
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOverlapping(this float value, float min, float max, out float clampedValue)
        {
            clampedValue = math.clamp(value, min, max);
            return math.abs(clampedValue - value) > 0.0001;//TODO лишний просчёт
        } 
    }

    public static class BoolExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFalse(this bool boolean) =>
            boolean == false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTrue(this bool boolean) =>
            boolean == true;
    }

    /// <summary> Диапазон от значений от минимума к максимуму. </summary>
    public struct MinMaxDiapason
    {
        public MinMaxDiapason(Vector2 minMax)
        {
            Minimum = minMax.x;
            Maximum = minMax.y;
        }
    
        public MinMaxDiapason(float min, float max)
        {
            Minimum = min;
            Maximum = max;
        }
        
        public float Minimum { get; set; }
        public float Maximum { get; set; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator MinMaxDiapason(Vector2 asMinMax) => 
            new MinMaxDiapason(asMinMax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2(MinMaxDiapason minMaxDiapason) => 
            new Vector2(minMaxDiapason.Minimum, minMaxDiapason.Maximum);
    }
}