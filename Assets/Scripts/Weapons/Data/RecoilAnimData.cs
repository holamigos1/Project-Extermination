// Designed by Kinemation, 2023

using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons.Data
{
    [CreateAssetMenu(fileName = "NewRecoilAnimData", menuName = "RecoilAnimData")]
    public class RecoilAnimData : ScriptableObject
    {
        [Header("Rotation Targets")]
        public Vector2 pitch;
        public Vector4 roll;
        public Vector4 yaw;

        [Header("Translation Targets")] 
        public Vector2 kickback;
        public Vector2 kickUp;
        public Vector2 kickRight;

        [ FormerlySerializedAs("aimRot")]
        [ Header("Aiming Multipliers")]
        public Vector3 aimRotation;
        [FormerlySerializedAs("aimLoc")] 
        public Vector3 aimLocation;

        [ FormerlySerializedAs("smoothRot")]
        [ Header("Auto/Burst Settings")]
        public Vector3 smoothRotation;
        [FormerlySerializedAs("smoothLoc")] 
        public Vector3 smoothLocation;
        
        [FormerlySerializedAs("extraRot")] public Vector3 extraRotation;
        [FormerlySerializedAs("extraLoc")] public Vector3 extraLocation;
    
        [Header("Noise Layer")]
        public Vector2 noiseX;
        public Vector2 noiseY;

        public Vector2 noiseAccel;
        public Vector2 noiseDamp;
    
        public float noiseScalar = 1f;
    
        [Header("Pushback Layer")]
        public float pushAmount = 0f;
        public float pushAccel;
        public float pushDamp;

        [Header("Misc")]
        public bool smoothRoll;
        public float playRate;
    
        [Header("Recoil Curves")]
        public RecoilCurves recoilCurves;
    }
}
