// Designed by Kinemation, 2023

using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class LookLayer : AnimLayer
    {
        [SerializeField, Range(0f, 1f)] protected float handsLayerAlpha;
        [SerializeField] protected float handsLerpSpeed;

        [SerializeField, Range(0f, 1f)] protected float pelvisLayerAlpha = 1f;
        [SerializeField] protected float pelvisLerpSpeed;
        protected float interpPelvis;

        [Header("Offsets")] 
        [SerializeField] protected Vector3 pelvisOffset;

        [Header("Aim Offset")] [SerializeField]
        protected AimOffset lookUpOffset;

        [SerializeField] protected AimOffset lookRightOffset;

        [SerializeField] protected bool enableAutoDistribution;
        [SerializeField] protected bool enableManualSpineControl;

        [SerializeField, Range(-90f, 90f)] protected float aimUp;
        [SerializeField, Range(-90f, 90f)] protected float aimRight;

        // Aim rotation lerp speed. If 0, no lag will be applied.
        [SerializeField] protected float smoothAim;

        [Header("Leaning")]
        [SerializeField] [Range(-1, 1)] protected int leanDirection;
        [SerializeField] protected float leanAmount = 45f;
        [SerializeField] protected float leanSpeed;
        
        [Header("Misc")]
        [SerializeField] protected bool detectZeroFrames = true;
        [SerializeField] protected bool checkZeroFootIK = true;
        [SerializeField] protected bool useRightOffset = true;
        
        protected float leanInput;
        
        protected float interpHands;
        protected Vector2 lerpedAim;

        // Used to detect zero key-frames
        [SerializeField] [HideInInspector] private CachedBones cachedBones;
        [SerializeField] [HideInInspector] private CachedBones cacheRef;

        public void SetPelvisWeight(float weight)
        {
            pelvisLayerAlpha = Mathf.Clamp01(weight);
        }

        public void SetHandsWeight(float weight)
        {
            handsLayerAlpha = Mathf.Clamp01(weight);
        }
        
        public override void OnPreAnimUpdate()
        {
            base.OnPreAnimUpdate();
            if (detectZeroFrames)
            {
                CheckZeroFrames();
            }
        }

        public override void OnAnimUpdate()
        {
            ApplySpineLayer();
        }

        public override void OnPostIK()
        {
            if (detectZeroFrames)
            {
                CacheBones();
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            lookUpOffset.Init();
            lookRightOffset.Init();
        }

        private void OnValidate()
        {
            if (cachedBones.lookUp == null)
            {
                cachedBones.lookUp ??= new List<Quaternion>();
                cacheRef.lookUp ??= new List<Quaternion>();
            }

            if (!lookUpOffset.IsValid() || lookUpOffset.IsChanged())
            {
                lookUpOffset.Init();

                cachedBones.lookUp.Clear();
                cacheRef.lookUp.Clear();

                for (int i = 0; i < lookUpOffset.bones.Count; i++)
                {
                    cachedBones.lookUp.Add(Quaternion.identity);
                    cacheRef.lookUp.Add(Quaternion.identity);
                }
            }

            if (!lookRightOffset.IsValid() || lookRightOffset.IsChanged())
            {
                lookRightOffset.Init();
            }

            void Distribute(ref AimOffset aimOffset)
            {
                if (enableAutoDistribution)
                {
                    bool enable = false;
                    int divider = 1;
                    float sum = 0f;

                    for (int i = 0; i < aimOffset.bones.Count - aimOffset.indexOffset; i++)
                    {
                        if (enable)
                        {
                            var bone = aimOffset.bones[i];
                            bone.maxAngle.x = (90f - sum) / divider;
                            aimOffset.bones[i] = bone;
                            continue;
                        }

                        if (!Mathf.Approximately(aimOffset.bones[i].maxAngle.x, aimOffset.angles[i].x))
                        {
                            divider = aimOffset.bones.Count - aimOffset.indexOffset - (i + 1);
                            enable = true;
                        }

                        sum += aimOffset.bones[i].maxAngle.x;
                    }
                }

                if (enableAutoDistribution)
                {
                    bool enable = false;
                    int divider = 1;
                    float sum = 0f;

                    for (int i = 0; i < aimOffset.bones.Count - aimOffset.indexOffset; i++)
                    {
                        if (enable)
                        {
                            var bone = aimOffset.bones[i];
                            bone.maxAngle.y = (90f - sum) / divider;
                            aimOffset.bones[i] = bone;
                            continue;
                        }

                        if (!Mathf.Approximately(aimOffset.bones[i].maxAngle.y, aimOffset.angles[i].y))
                        {
                            divider = aimOffset.bones.Count - aimOffset.indexOffset - (i + 1);
                            enable = true;
                        }

                        sum += aimOffset.bones[i].maxAngle.y;
                    }
                }

                for (int i = 0; i < aimOffset.bones.Count - aimOffset.indexOffset; i++)
                {
                    aimOffset.angles[i] = aimOffset.bones[i].maxAngle;
                }
            }

            if (lookUpOffset.bones.Count > 0)
            {
                Distribute(ref lookUpOffset);
            }

            if (lookRightOffset.bones.Count > 0)
            {
                Distribute(ref lookRightOffset);
            }
        }
        
        private void CheckZeroFrames()
        {
            if (cachedBones.pelvis.Item1 == core.rigData.pelvis.localPosition)
            {
                core.rigData.pelvis.localPosition = cacheRef.pelvis.Item1;
            }
            
            if (cachedBones.pelvis.Item2 == core.rigData.pelvis.localRotation)
            {
                core.rigData.pelvis.localRotation = cacheRef.pelvis.Item2;
                
                if (checkZeroFootIK)
                {
                    core.rigData.rightFoot.Retarget();
                    core.rigData.leftFoot.Retarget();
                }
            }

            cacheRef.pelvis.Item2 = core.rigData.pelvis.localRotation;

            bool bZeroSpine = false;
            for (int i = 0; i < cachedBones.lookUp.Count; i++)
            {
                var bone = lookUpOffset.bones[i].bone;
                if (bone == null || bone == core.rigData.pelvis)
                {
                    continue;
                }

                if (cachedBones.lookUp[i] == bone.localRotation)
                {
                    bZeroSpine = true;
                    bone.localRotation = cacheRef.lookUp[i];
                }
            }
            
            if (bZeroSpine)
            {
                core.rigData.masterDynamic.Retarget();
                core.rigData.rightHand.Retarget();
                core.rigData.leftHand.Retarget();
            }
            
            cacheRef.pelvis.Item1 = core.rigData.pelvis.localPosition;

            for (int i = 0; i < lookUpOffset.bones.Count; i++)
            {
                var bone = lookUpOffset.bones[i].bone;
                if (bone == null)
                {
                    continue;
                }
                
                cacheRef.lookUp[i] = bone.localRotation;
            }
        }
        
        private void CacheBones()
        {
            cachedBones.pelvis.Item1 = core.rigData.pelvis.localPosition;
            cachedBones.pelvis.Item2 = core.rigData.pelvis.localRotation;
            
            for (int i = 0; i < lookUpOffset.bones.Count; i++)
            {
                var bone = lookUpOffset.bones[i].bone;
                if (bone == null || bone == core.rigData.pelvis)
                {
                    continue;
                }

                cachedBones.lookUp[i] = bone.localRotation;
            }
        }

        private bool BlendLayers()
        {
            return Mathf.Approximately(smoothLayerAlpha, 0f);
        }

        private void ApplySpineLayer()
        {
            if (BlendLayers())
            {
                return;
            }
            
            if (!enableManualSpineControl)
            {
                aimUp = GetCharData().totalAimInput.y;
                aimRight = GetCharData().totalAimInput.x;
                
                if (lookRightOffset.bones.Count == 0 || !useRightOffset)
                {
                    aimRight = 0f;
                }
                
                leanInput = CoreToolkitLib.Glerp(leanInput, leanAmount * GetCharData().leanDirection, 
                    leanSpeed);
            }
            else
            {
                leanInput = CoreToolkitLib.Glerp(leanInput, leanAmount * leanDirection, leanSpeed);
            }
            
            interpPelvis = CoreToolkitLib.Glerp(interpPelvis, pelvisLayerAlpha * smoothLayerAlpha, 
                pelvisLerpSpeed);
            
            Vector3 pelvisFinal = Vector3.Lerp(Vector3.zero, pelvisOffset, interpPelvis);
            CoreToolkitLib.MoveInBoneSpace(GetRootBone(), core.rigData.pelvis, pelvisFinal, 1f);

            lerpedAim.y = CoreToolkitLib.GlerpLayer(lerpedAim.y, aimUp, smoothAim);
            lerpedAim.x = CoreToolkitLib.GlerpLayer(lerpedAim.x, aimRight, smoothAim);

            foreach (var bone in lookRightOffset.bones)
            {
                if (!Application.isPlaying && bone.bone == null)
                {
                    continue;
                }

                float angleFraction = lerpedAim.x >= 0f ? bone.maxAngle.y : bone.maxAngle.x;
                CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, bone.bone,
                    Quaternion.Euler(0f, lerpedAim.x * smoothLayerAlpha / (90f / angleFraction),0f), 1f);
            }
            
            foreach (var bone in lookRightOffset.bones)
            {
                if (!Application.isPlaying && bone.bone == null)
                {
                    continue;
                }

                float angleFraction = bone.maxAngle.x;
                CoreToolkitLib.RotateInBoneSpace(
                    GetRootBone().rotation * Quaternion.Euler(0f, lerpedAim.x, 0f), bone.bone,
                    Quaternion.Euler(0f, 0f, leanInput * smoothLayerAlpha / (90f / angleFraction)), 1f);
            }

            Vector3 rightHandLoc = core.rigData.rightHand.obj.transform.position;
            Quaternion rightHandRot = core.rigData.rightHand.obj.transform.rotation;

            Vector3 leftHandLoc = core.rigData.leftHand.obj.transform.position;
            Quaternion leftHandRot = core.rigData.leftHand.obj.transform.rotation;

            foreach (var bone in lookUpOffset.bones)
            {
                if (!Application.isPlaying && bone.bone == null)
                {
                    continue;
                }

                float angleFraction = lerpedAim.y >= 0f ? bone.maxAngle.y : bone.maxAngle.x;

                CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation * Quaternion.Euler(0f, lerpedAim.x, 0f),
                    bone.bone,
                    Quaternion.Euler(lerpedAim.y * smoothLayerAlpha / (90f / angleFraction), 0f, 0f), 1f);
            }

            interpHands = CoreToolkitLib.GlerpLayer(interpHands, handsLayerAlpha, handsLerpSpeed);

            core.rigData.rightHand.obj.transform.position = Vector3.Lerp(rightHandLoc,
                core.rigData.rightHand.obj.transform.position,
                interpHands);
            core.rigData.rightHand.obj.transform.rotation = Quaternion.Slerp(rightHandRot,
                core.rigData.rightHand.obj.transform.rotation,
                interpHands);

            core.rigData.leftHand.obj.transform.position = Vector3.Lerp(leftHandLoc, core.rigData.leftHand.obj.transform.position,
                interpHands);
            core.rigData.leftHand.obj.transform.rotation = Quaternion.Slerp(leftHandRot,
                core.rigData.leftHand.obj.transform.rotation,
                interpHands);
        }
    }
}