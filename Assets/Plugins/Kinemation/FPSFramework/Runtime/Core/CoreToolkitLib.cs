// Designed by Kinemation, 2023

using System;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Kinemation.FPSFramework.Runtime.Core
{
    [Serializable]
    public struct VectorCurve
    {
        public AnimationCurve x;
        public AnimationCurve y;
        public AnimationCurve z;

        public float GetLastTime()
        {
            float maxTime = -1f;

            float curveTime = GetMaxTime(x);
            maxTime = curveTime > maxTime ? curveTime : maxTime;
        
            curveTime = GetMaxTime(y);
            maxTime = curveTime > maxTime ? curveTime : maxTime;
        
            curveTime = GetMaxTime(z);
            maxTime = curveTime > maxTime ? curveTime : maxTime;

            return maxTime;
        }
        
        public static float GetMaxTime(AnimationCurve curve)
        {
            return curve[curve.length - 1].time;
        }
        
        public Vector3 Evaluate(float time)
        {
            return new Vector3(x.Evaluate(time), y.Evaluate(time), z.Evaluate(time));
        }

        public bool IsValid()
        {
            return x != null && y != null && z != null;
        }
    }
    
    [Serializable]
    public struct LocRot
    {
        public Vector3 position;
        public Quaternion rotation;

        public LocRot(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
        
        public LocRot(Transform t)
        {
            position = t.position;
            rotation = t.rotation;
        }
    }

    [Serializable]
    public struct SpringData
    {
        public float stiffness;
        public float criticalDamping;
        public float speed;
        public float mass;
        public float maxValue;
        [NonSerialized] public float error;
        [NonSerialized] public float velocity;

        public SpringData(float stiffness, float damping, float speed, float mass)
        {
            this.stiffness = stiffness;
            criticalDamping = damping;
            this.speed = speed;
            this.mass = mass;

            error = 0f;
            velocity = 0f;
            maxValue = 0f;
        }
        
        public SpringData(float stiffness, float damping, float speed)
        {
            this.stiffness = stiffness;
            criticalDamping = damping;
            this.speed = speed;
            mass = 1f;

            error = 0f;
            velocity = 0f;
            maxValue = 0f;
        }
    }

    [Serializable]
    public struct VectorSpringData
    {
        public SpringData x;
        public SpringData y;
        public SpringData z;
        public Vector3 scale;
    }

    [Serializable]
    public struct LocRotSpringData
    {
        public VectorSpringData loc;
        public VectorSpringData rot;
    }
    
    [Serializable]
    public struct FreeAimData
    {
        public float scalar;
        public float maxValue;
        public float speed;
    }

    [Serializable]
    public struct MoveSwayData
    {
        public Vector3 maxMoveLocSway;
        public Vector3 maxMoveRotSway;
    }
    
    [Serializable]
    public struct GunBlockData
    {
        public float weaponLength;
        public float startOffset;
        public float threshold;
        public LocRot restPose;
    }
    
    // Defines weapon-related properties, updated when weapon is equipped/unequipped
    [Serializable]
    public struct WeaponAnimData
    {
        [Header("LeftHandIK")]
        public Transform leftHandTarget;
        
        [Header("AdsLayer")]
        public GunAimData gunAimData;
        public Vector3 handsOffset;
        [Header("SwayLayer")]
        public LocRotSpringData springData;
        public FreeAimData freeAimData;
        public MoveSwayData moveSwayData;
        [Header("WeaponCollision")] 
        public GunBlockData blockData;
    }

    // Defines player input data consumed by Anim Layers
    public struct CharAnimData
    {
        // Input
        public Vector2 deltaAimInput;
        public Vector2 totalAimInput;
        public Vector2 moveInput;
        public int leanDirection;

        public void AddAimInput(Vector2 aimInput)
        {
            deltaAimInput = aimInput;
            totalAimInput += deltaAimInput;
            totalAimInput.x = Mathf.Clamp(totalAimInput.x, -90f, 90f);
            totalAimInput.y = Mathf.Clamp(totalAimInput.y, -90f, 90f);
        }
        
        public void SetAimInput(Vector2 aimInput)
        {
            totalAimInput.x = Mathf.Clamp(aimInput.x, -90f, 90f);
            totalAimInput.y = Mathf.Clamp(aimInput.y, -90f, 90f);
        }
    }

    // Procedural animation
    [Serializable]
    public struct DynamicMotion
    {
        // Curve-based animation
        public VectorCurve rot;
        public VectorCurve loc;
        
        // How fast to blend to another motion
        [SerializeField] private float blendSpeed;
        [SerializeField] private float playRate;
        
        private float playBack;
        private float blendAlpha;
        
        public LocRot outMotion;
        // Used to blend to the currentMotion
        private LocRot cachedMotion;
        private float motionLength;

        public void Reset()
        {
            outMotion = cachedMotion = new LocRot(Vector3.zero, Quaternion.identity);
        }

        public void Play(ref DynamicMotion previousMotion)
        {
            cachedMotion = previousMotion.outMotion;
            playRate = Mathf.Approximately(playRate, 0f) ? 1f : playRate;
            motionLength = Mathf.Max(loc.GetLastTime(), rot.GetLastTime());
            playBack = 0f;
            blendAlpha = 0f;
        }

        private LocRot Evaluate()
        {
            return new LocRot(loc.Evaluate(playBack), Quaternion.Euler(rot.Evaluate(playBack)));
        }

        // Return currently playing motion
        public void UpdateMotion()
        {
            if (Mathf.Approximately(playBack, motionLength))
            {
                outMotion = new LocRot(Vector3.zero, Quaternion.identity);
                return;
            }
            
            playBack += Time.deltaTime * playRate;
            playBack = Mathf.Clamp(playBack, 0f, motionLength);
            var currentMotion = Evaluate();

            blendAlpha += Time.deltaTime * blendSpeed;
            blendAlpha = Mathf.Min(1f, blendAlpha);

            var result = CoreToolkitLib.Lerp(cachedMotion, currentMotion, blendAlpha);
            outMotion = result;
        }
    }

    public struct MotionPlayer
    {
        private DynamicMotion motion;

        public void Reset()
        {
            motion.Reset();
        }
        
        public void Play(DynamicMotion motionToPlay)
        {
            var cache = motion;
            motion = motionToPlay;
            motion.Reset();
            motion.Play(ref cache);
        }

        public void UpdateMotion()
        {
            motion.UpdateMotion();
        }

        public LocRot Get()
        {
            return motion.outMotion;
        }
    }

    public static class CoreToolkitLib
    {
        private const float FloatMin = 1e-10f;
        private const float SqrEpsilon = 1e-8f;
        
        public static float SpringInterp(float current, float target, ref SpringData springData)
        {
            float interpSpeed = Mathf.Min(Time.deltaTime * springData.speed, 1f);
            target = Mathf.Clamp(target, -springData.maxValue, springData.maxValue);
            
            if (!Mathf.Approximately(interpSpeed, 0f))
            {
                if (!Mathf.Approximately(springData.mass, 0f))
                {
                    float damping = 2 * Mathf.Sqrt(springData.mass * springData.stiffness) * springData.criticalDamping;
                    float error = target - current;
                    float errorDeriv = (error - springData.error);
                    springData.velocity +=
                        (error * springData.stiffness * interpSpeed + errorDeriv * damping) /
                        springData.mass;
                    springData.error = error;

                    float value = current + springData.velocity * interpSpeed;
                    return value;
                }
            
                return target;
            }

            return current;
        }

        public static Vector3 SpringInterp(Vector3 current, Vector3 target, ref VectorSpringData springData)
        {
            Vector3 final = Vector3.zero;

            final.x = SpringInterp(current.x, target.x * springData.scale.x, ref springData.x);
            final.y = SpringInterp(current.y, target.y * springData.scale.y, ref springData.y);
            final.z = SpringInterp(current.z, target.z * springData.scale.z, ref springData.z);

            return final;
        }

        public static LocRot SpringInterp(LocRot current, LocRot target,
            ref LocRotSpringData springData)
        {
            LocRot final = new LocRot(Vector3.zero, Quaternion.identity);

            final.position = SpringInterp(current.position, target.position, ref springData.loc);
            final.rotation = Quaternion.Euler(SpringInterp(current.rotation.eulerAngles, target.rotation.eulerAngles,
                ref springData.rot));
            
            return final;
        }

        // Frame-rate independent interpolation
        public static float Glerp(float a, float b, float speed)
        {
            return Mathf.Lerp(a, b, 1 - Mathf.Exp(-speed * Time.deltaTime));
        }
        
        public static float GlerpLayer(float a, float b, float speed)
        {
            return Mathf.Approximately(speed, 0f)
                ? b
                : Mathf.Lerp(a, b, 1 - Mathf.Exp(-speed * Time.deltaTime));
        }

        public static Vector3 Glerp(Vector3 a, Vector3 b, float speed)
        {
            return Vector3.Lerp(a, b, 1 - Mathf.Exp(-speed * Time.deltaTime));
        }
        
        public static Vector2 Glerp(Vector2 a, Vector2 b, float speed)
        {
            return Vector2.Lerp(a, b, 1 - Mathf.Exp(-speed * Time.deltaTime));
        }

        public static Quaternion Glerp(Quaternion a, Quaternion b, float speed)
        {
            return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-speed * Time.deltaTime));
        }

        public static LocRot Glerp(LocRot a, LocRot b, float speed)
        {
            var Rot = Quaternion.Slerp(a.rotation, b.rotation, 1 - Mathf.Exp(-speed * Time.deltaTime));
            var Loc = Vector3.Lerp(a.position, b.position, 1 - Mathf.Exp(-speed * Time.deltaTime));
            return new LocRot(Loc, Rot);
        }

        public static LocRot Lerp(LocRot a, LocRot b, float alpha)
        {
            var loc = Vector3.Lerp(a.position, b.position, alpha);
            var rot = Quaternion.Slerp(a.rotation, b.rotation, alpha);
            return new LocRot(loc, rot);
        }
        
        public static void RotateInBoneSpace(Quaternion parent, Transform bone, Quaternion rotation, float alpha)
        {
            Quaternion boneRot = bone.rotation;
            Quaternion outRot = (parent * rotation) * (Quaternion.Inverse(parent) * boneRot);
            bone.rotation = Quaternion.Slerp(boneRot, outRot, alpha);
        }
        
        public static void MoveInBoneSpace(Transform parent, Transform bone, Vector3 offset, float alpha)
        {
            var root = parent.transform;
            Vector3 finalOffset = root.TransformPoint(offset);
            finalOffset -= root.position;

            bone.position += finalOffset * alpha;
        }

        // Adapted from Two Bone IK constraint, Unity Animation Rigging package
        public static void SolveTwoBoneIK(
            Transform root,
            Transform mid,
            Transform tip,
            Transform target,
            Transform hint,
            float posWeight,
            float rotWeight,
            float hintWeight
        )
        {
            Vector3 aPosition = root.position;
            Vector3 bPosition = mid.position;
            Vector3 cPosition = tip.position;
            Vector3 tPosition = Vector3.Lerp(cPosition, target.position, posWeight);
            Quaternion tRotation = Quaternion.Lerp(tip.rotation, target.rotation, rotWeight);
            bool hasHint = hint != null && hintWeight > 0f;

            Vector3 ab = bPosition - aPosition;
            Vector3 bc = cPosition - bPosition;
            Vector3 ac = cPosition - aPosition;
            Vector3 at = tPosition - aPosition;

            float abLen = ab.magnitude;
            float bcLen = bc.magnitude;
            float acLen = ac.magnitude;
            float atLen = at.magnitude;

            float oldAbcAngle = TriangleAngle(acLen, abLen, bcLen);
            float newAbcAngle = TriangleAngle(atLen, abLen, bcLen);

            // Bend normal strategy is to take whatever has been provided in the animation
            // stream to minimize configuration changes, however if this is collinear
            // try computing a bend normal given the desired target position.
            // If this also fails, try resolving axis using hint if provided.
            Vector3 axis = Vector3.Cross(ab, bc);
            if (axis.sqrMagnitude < SqrEpsilon)
            {
                axis = hasHint ? Vector3.Cross(hint.position - aPosition, bc) : Vector3.zero;

                if (axis.sqrMagnitude < SqrEpsilon)
                    axis = Vector3.Cross(at, bc);

                if (axis.sqrMagnitude < SqrEpsilon)
                    axis = Vector3.up;
            }

            axis = Vector3.Normalize(axis);

            float a = 0.5f * (oldAbcAngle - newAbcAngle);
            float sin = Mathf.Sin(a);
            float cos = Mathf.Cos(a);
            Quaternion deltaR = new Quaternion(axis.x * sin, axis.y * sin, axis.z * sin, cos);
            mid.rotation = deltaR * mid.rotation;
            
            cPosition = tip.position;
            ac = cPosition - aPosition;
            root.rotation = FromToRotation(ac, at) * root.rotation;

            if (hasHint)
            {
                float acSqrMag = ac.sqrMagnitude;
                if (acSqrMag > 0f)
                {
                    bPosition = mid.position;
                    cPosition = tip.position;
                    ab = bPosition - aPosition;
                    ac = cPosition - aPosition;

                    Vector3 acNorm = ac / Mathf.Sqrt(acSqrMag);
                    Vector3 ah = hint.position - aPosition;
                    Vector3 abProj = ab - acNorm * Vector3.Dot(ab, acNorm);
                    Vector3 ahProj = ah - acNorm * Vector3.Dot(ah, acNorm);

                    float maxReach = abLen + bcLen;
                    if (abProj.sqrMagnitude > (maxReach * maxReach * 0.001f) && ahProj.sqrMagnitude > 0f)
                    {
                        Quaternion hintR = FromToRotation(abProj, ahProj);
                        hintR.x *= hintWeight;
                        hintR.y *= hintWeight;
                        hintR.z *= hintWeight;
                        hintR = NormalizeSafe(hintR);
                        root.rotation = hintR * root.rotation;
                    }
                }
            }

            tip.rotation = tRotation;
        }
        
        private static float TriangleAngle(float aLen, float aLen1, float aLen2)
        {
            float c = Mathf.Clamp((aLen1 * aLen1 + aLen2 * aLen2 - aLen * aLen) / (aLen1 * aLen2) / 2.0f, -1.0f, 1.0f);
            return Mathf.Acos(c);
        }

        private static Quaternion FromToRotation(Vector3 from, Vector3 to)
        {
            float theta = Vector3.Dot(from.normalized, to.normalized);
            if (theta >= 1f)
                return Quaternion.identity;

            if (theta <= -1f)
            {
                Vector3 axis = Vector3.Cross(from, Vector3.right);
                if (axis.sqrMagnitude == 0f)
                    axis = Vector3.Cross(from, Vector3.up);

                return Quaternion.AngleAxis(180f, axis);
            }

            return Quaternion.AngleAxis(Mathf.Acos(theta) * Mathf.Rad2Deg, Vector3.Cross(from, to).normalized);
        }

        private static Quaternion NormalizeSafe(Quaternion q)
        {
            float dot = Quaternion.Dot(q, q);
            if (dot > FloatMin)
            {
                float rsqrt = 1.0f / Mathf.Sqrt(dot);
                return new Quaternion(q.x * rsqrt, q.y * rsqrt, q.z * rsqrt, q.w * rsqrt);
            }

            return Quaternion.identity;
        }
    }
}