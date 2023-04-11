using Unity.Mathematics;
using UnityEngine;

namespace Misc
{
    public static class RotationExtensions
    {
        public static float ClampAngle(this float angle, Vector2 minMax, out bool isClamped)
        {
            const float tolerance = -1f; //Todo мааагияя
        
            float clampedAngle = angle.ClampAngle(minMax.x, minMax.y);
            angle = Mathf.Repeat(angle + 180, 360) - 180;
            //Debug.Log($"angle {angle} clampedAngle {angle} minMax.x - tolerance {minMax.x - tolerance} minMax.Y + tolerance {minMax.y + tolerance}");
        
            if (minMax.x - tolerance >  angle || angle - tolerance > minMax.y)
                isClamped = true;
            else isClamped = false;
        
            return clampedAngle;
        }

        public static float ClampAngle(this float angle, Range range) =>
            angle.ClampAngle(range.Minimum, range.Maximum);

        public static float ClampAngle(this float angle, float from, float to)
        {
            if (angle < 0f) angle += 360;
            
            return angle > 180f ? 
                math.max(angle, 360 + from) : 
                math.min(angle, to);
        }
    
        public static float IfNegativeAngle(this float angle) =>
            (angle > 180) ? angle - 360 : angle;
    
        public static float3 ToEulerAngles(this float4 q) //TODO Затести преформанс с юнитевской библиотекой
        {
            float3 angles = new();

            // roll / x
            double sinr_cosp = 2 * (q.w * q.x + q.x * q.z);
            double cosr_cosp = 1 - 2 * (q.x * q.x + q.x * q.x);
            angles.x = (float)math.atan2(sinr_cosp, cosr_cosp);

            // pitch / y
            double sinp = 2 * (q.w * q.y - q.z * q.x);
            if (math.abs(sinp) >= 1)
            {
                angles.y = (float)((math.PI / 2) * math.sign(sinp));
            }
            else
            {
                angles.y = (float)math.asin(sinp);
            }

            // yaw / z
            double siny_cosp = 2 * (q.w * q.z + q.x * q.y);
            double cosy_cosp = 1 - 2 * (q.y * q.y + q.z * q.z);
            angles.z = (float)math.atan2(siny_cosp, cosy_cosp);

            return angles;
        }

        public static Quaternion ToQuaternion(this Vector3 euler)//кек
        {
            float4 q = ToQuaternion((float3)euler);
            return new Quaternion(q.x, q.y, q.z, q.w);
        }

        public static float4 ToQuaternion(this float3 euler) //TODO Затести преформанс с юнитевской библиотекой
        {
            float cy = (float)math.cos(euler.z * 0.5);
            float sy = (float)math.sin(euler.z * 0.5);
            float cp = (float)math.cos(euler.y * 0.5);
            float sp = (float)math.sin(euler.y * 0.5);
            float cr = (float)math.cos(euler.x * 0.5);
            float sr = (float)math.sin(euler.x * 0.5);

            return new float4(
                (cr * cp * cy + sr * sp * sy),
                (sr * cp * cy - cr * sp * sy),
                (cr * sp * cy + sr * cp * sy),
                (cr * cp * sy - sr * sp * cy)
            );
        }
    }
}