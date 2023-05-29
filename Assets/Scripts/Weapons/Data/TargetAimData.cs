// Designed by Kinemation, 2023

using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons.Data
{
    [CreateAssetMenu(fileName = "NewAimData", menuName = "TargetAimData")]
    public class TargetAimData : ScriptableObject
    {
        [FormerlySerializedAs("aimLoc")] public Vector3       aimLocation;
        [FormerlySerializedAs("aimRot")] public Quaternion    aimRotation;
        public                                  AnimationClip staticPose;
        public                                  string        stateName;
    }
}