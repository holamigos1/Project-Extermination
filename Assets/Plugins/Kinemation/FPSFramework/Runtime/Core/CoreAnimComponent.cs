// Designed by Kinemation, 2023

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Kinemation.FPSFramework.Runtime.Core
{
    [Serializable]
    public struct AimOffsetBone
    {
        public Transform bone;
        public Vector2 maxAngle;
    }

    // Collection of AimOffsetBones, used to rotate spine bones to look around
    [Serializable]
    public struct AimOffset
    {
        public List<AimOffsetBone> bones;
        public int indexOffset;

        [NonSerialized] public List<Vector2> angles;

        public void Init()
        {
            if (angles == null)
            {
                angles = new List<Vector2>();
            }
            else
            {
                angles.Clear();
            }

            bones ??= new List<AimOffsetBone>();

            for (int i = 0; i < bones.Count - indexOffset; i++)
            {
                var bone = bones[i];
                angles.Add(bone.maxAngle);
            }
        }

        public bool IsValid()
        {
            return bones != null && angles != null;
        }

        public bool IsChanged()
        {
            return bones.Count != angles.Count;
        }
    }

    // DynamicBone is essentially an IK bone
    [Serializable]
    public struct DynamicBone
    {
        [Tooltip("Actual bone")]
        public Transform target;
        [Tooltip("Target for elbows/knees")]
        public Transform hintTarget;
        [Tooltip("The representation of the DynamicBone in space")]
        public GameObject obj;

        public void Retarget()
        {
            if (target == null)
            {
                return;
            }

            obj.transform.position = target.position;
            obj.transform.rotation = target.rotation;
        }

        public void Rotate(Quaternion parent, Quaternion rotation, float alpha)
        {
            CoreToolkitLib.RotateInBoneSpace(parent, obj.transform, rotation, alpha);
        }

        public void Move(Transform parent, Vector3 offset, float alpha)
        {
            CoreToolkitLib.MoveInBoneSpace(parent, obj.transform, offset, alpha);
        }
    }

    // Used for detecting zero-frames
    [Serializable]
    public struct CachedBones
    {
        public (Vector3, Quaternion) pelvis;
        public (Vector3, Quaternion) rightHand;
        public (Vector3, Quaternion) leftHand;

        public List<Quaternion> lookUp;
    }

    // Essential skeleton data, used by Anim Layers
    [Serializable]
    public struct DynamicRigData
    {
        public Animator animator;
        public Transform pelvis;
        public DynamicBone masterDynamic;
        public DynamicBone rightHand;
        public DynamicBone leftHand;
        public DynamicBone rightFoot;
        public DynamicBone leftFoot;

        [Tooltip("Used for mesh space calculations")]
        public Transform rootBone;

        public CharAnimData characterData;
        public WeaponAnimData gunData;

        public void Retarget()
        {
            masterDynamic.Retarget();
            rightHand.Retarget();
            leftHand.Retarget();
            rightFoot.Retarget();
            leftFoot.Retarget();
        }
    }
    
    [Serializable]
    public abstract class AnimLayer : MonoBehaviour
    {
        [Header("Layer Blending")] 
        [SerializeField, Range(0f, 1f)] protected float layerAlpha = 1f;
        [SerializeField] protected float lerpSpeed;
        protected float smoothLayerAlpha;
        
        [Header("Misc")]
        [SerializeField] public bool runInEditor;
        protected CoreAnimComponent core;

        public void SetLayerAlpha(float weight)
        {
            layerAlpha = Mathf.Clamp01(weight);
        }
        
        public virtual void OnAnimStart()
        {
        }
        
        public void OnRetarget(CoreAnimComponent comp)
        {
            core = comp;
        }

        public virtual void OnPreAnimUpdate()
        {
            smoothLayerAlpha = CoreToolkitLib.GlerpLayer(smoothLayerAlpha, layerAlpha, lerpSpeed);
        }
        
        public virtual void OnAnimUpdate()
        {
        }

        public virtual void OnPostIK()
        {
        }

        protected virtual void Awake()
        {
        }
        
        protected WeaponAnimData GetGunData()
        {
            return core.rigData.gunData;
        }
        
        protected CharAnimData GetCharData()
        {
            return core.rigData.characterData;
        }

        protected Transform GetMasterIK()
        {
            return core.rigData.masterDynamic.obj.transform;
        }

        protected Transform GetRootBone()
        {
            return core.rigData.rootBone;
        }

        protected Transform GetPelvis()
        {
            return core.rigData.pelvis;
        }
        
        protected DynamicBone GetMasterPivot()
        {
            return core.rigData.masterDynamic;
        }

        protected DynamicBone GetRightHand()
        {
            return core.rigData.rightHand;
        }
        
        protected DynamicBone GetLeftHand()
        {
            return core.rigData.leftHand;
        }
        
        protected DynamicBone GetRightFoot()
        {
            return core.rigData.rightFoot;
        }
        
        protected DynamicBone GetLeftFoot()
        {
            return core.rigData.leftFoot;
        }

        protected Animator GetAnimator()
        {
            return core.rigData.animator;
        }

        // Offsets master pivot only, without affecting the child IK bones
        // Useful if weapon has multiple pivots
        protected void OffsetMasterPivot(LocRot offset)
        {
            LocRot rightHandTip = new LocRot(GetRightHand().obj.transform);
            LocRot leftHandTip = new LocRot(GetLeftHand().obj.transform);
            
            GetMasterPivot().Move(GetMasterIK(), offset.position, 1f);
            GetMasterPivot().Rotate(GetMasterIK().rotation, offset.rotation, 1f);

            GetRightHand().obj.transform.position = rightHandTip.position;
            GetRightHand().obj.transform.rotation = rightHandTip.rotation;
            
            GetLeftHand().obj.transform.position = leftHandTip.position;
            GetLeftHand().obj.transform.rotation = leftHandTip.rotation;
        }
    }
    
    [Serializable]
    public struct BlendTime
    {
        [Min(0f)] public float blendStart;
        [Min(0f)] public float blendEnd;

        public BlendTime(float startTime, float endTime)
        {
            blendStart = startTime;
            blendEnd = endTime;
        }

        public void Validate()
        {
            if (blendStart > blendEnd)
            {
                (blendStart, blendEnd) = (blendEnd, blendStart);
            }

            blendStart = blendStart < 0f ? 0f : blendStart;
            blendEnd = blendEnd < 0f ? 0f : blendEnd;
        }
    }

    [ExecuteAlways]
    public class CoreAnimComponent : MonoBehaviour
    {
        [Header("Essentials")]
        [Tooltip("The armature root, used for manual bone search")] 
        [SerializeField] private Transform skeleton;

        [Tooltip("Used to play animation from code")]
        public AvatarMask upperBodyMask;
        public DynamicRigData rigData;
        
        private PlayableGraph _playableGraph;
        private AnimationLayerMixerPlayable _layerMixerPlayable;
        private AnimationClipPlayable _animationPlayable;
        private BlendTime _blendOutTime;
        
        [SerializeField] [HideInInspector] private List<AnimLayer> animLayers;
        [SerializeField] private bool useIK = true;

        [Header("Misc")] 
        [SerializeField] private bool drawDebug;

        private bool _updateInEditor;
        private float _interpHands;
        private float _interpLayer;

        private Tuple<float, float> rightHandWeight = new(1f, 1f);
        private Tuple<float, float> leftHandWeight = new(1f, 1f);
        private Tuple<float, float> rightFootWeight = new(1f, 1f);
        private Tuple<float, float> leftFootWeight = new(1f, 1f);

        private void ApplyIK()
        {
            if (!useIK)
            {
                return;
            }
            
            void SolveIK(DynamicBone tipBone, Tuple<float, float> weights)
            {
                if (Mathf.Approximately(weights.Item1, 0f))
                {
                    return;
                }
                
                var lowerBone = tipBone.target.parent;
                CoreToolkitLib.SolveTwoBoneIK(lowerBone.parent, lowerBone, tipBone.target,
                    tipBone.obj.transform, tipBone.hintTarget, weights.Item1, weights.Item1, weights.Item2);
            }
            
            SolveIK(rigData.rightHand, rightHandWeight);
            SolveIK(rigData.leftHand, leftHandWeight);
            SolveIK(rigData.rightFoot, rightFootWeight);
            SolveIK(rigData.leftFoot, leftFootWeight);
        }

        private void OnEnable()
        {
            animLayers ??= new List<AnimLayer>();
        }

        private void InitPlayableGraph()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            var animator = rigData.animator;
            
            _playableGraph = animator.playableGraph;
            _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            _layerMixerPlayable = AnimationLayerMixerPlayable.Create(_playableGraph, 2);

            var output = AnimationPlayableOutput.Create(_playableGraph, "FPSAnimator", animator);
            output.SetSourcePlayable(_layerMixerPlayable);

            var controllerPlayable = AnimatorControllerPlayable.Create(_playableGraph, animator.runtimeAnimatorController);
            _playableGraph.Connect(controllerPlayable, 0, _layerMixerPlayable, 0);
            _layerMixerPlayable.SetInputWeight(0, 1f);

            _layerMixerPlayable.SetLayerMaskFromAvatarMask(1, upperBodyMask);

            _playableGraph.Play();
        }

        private void BlendOutAnimation()
        {
            if (!Application.isPlaying || !_animationPlayable.IsValid())
            {
                return;
            }

            var time = (float) _animationPlayable.GetTime();
            var clipLength = _animationPlayable.GetAnimationClip().length;

            time = Mathf.Max(time, 0.001f);
            clipLength = Mathf.Max(clipLength, 0.001f);

            float timeRatio = Mathf.Min(time / clipLength, _blendOutTime.blendEnd);
            if (timeRatio >= _blendOutTime.blendStart)
            {
                float progress = (timeRatio - _blendOutTime.blendStart) /
                                 Mathf.Max(_blendOutTime.blendEnd - _blendOutTime.blendStart, 0.001f);
                float weight = Mathf.Lerp(1f, 0f, Mathf.Sin((progress * Mathf.PI) / 2));
                _layerMixerPlayable.SetInputWeight(1, weight);

                // When reaching the end of the blending, disconnect the playable
                if (Mathf.Approximately(_blendOutTime.blendEnd, timeRatio))
                {
                    _layerMixerPlayable.DisconnectInput(1);
                }
            }
        }

        private void Start()
        {
            foreach (var layer in animLayers)
            {
                layer.OnAnimStart();
            }
            
            InitPlayableGraph();
        }

        private void Update()
        {
            if (!Application.isPlaying && _updateInEditor)
            {
                if (rigData.animator == null)
                {
                    return;
                }

                rigData.animator.Update(Time.deltaTime);
            }
            
            BlendOutAnimation();
        }
        
        private void LateUpdate()
        {
            if (!Application.isPlaying && !_updateInEditor)
            {
                return;
            }
            
            Retarget();
            PreUpdateLayers();
            UpdateLayers();
            ApplyIK();
            PostUpdateLayers();
        }
        
        private void OnDestroy()
        {
            if (!_playableGraph.IsValid())
            {
                return;
            }
            
            _playableGraph.Stop();
            _playableGraph.Destroy();
        }
        
        private void Retarget()
        {
            foreach (var layer in animLayers)
            {
                if (!Application.isPlaying && !layer.runInEditor)
                {
                    continue;
                }
                
                layer.OnRetarget(this);
            }
            
            rigData.Retarget();
        }

        // Called right after retargeting
        private void PreUpdateLayers()
        {
            foreach (var layer in animLayers)
            {
                if (!Application.isPlaying && !layer.runInEditor)
                {
                    continue;
                }
                
                layer.OnPreAnimUpdate();
            }
        }

        private void UpdateLayers()
        {
            foreach (var layer in animLayers)
            {
                if (!Application.isPlaying && !layer.runInEditor)
                {
                    continue;
                }
                
                layer.OnAnimUpdate();
            }
        }

        // Called after IK update
        private void PostUpdateLayers()
        {
            foreach (var layer in animLayers)
            {
                if (!Application.isPlaying && !layer.runInEditor)
                {
                    continue;
                }
                
                layer.OnPostIK();
            }
        }

        public void EnableEditorPreview()
        {
            if (rigData.animator == null)
            {
                rigData.animator = GetComponent<Animator>();
            }

            _updateInEditor = true;
        }

        public void DisableEditorPreview()
        {
            _updateInEditor = false;

            if (rigData.animator == null)
            {
                return;
            }

            rigData.animator.Rebind();
            rigData.animator.Update(0f);
        }
        
        public Transform GetRootBone()
        {
            return rigData.rootBone;
        }
        
        public void PlayAnimation(AnimationClip clip, BlendTime blendOut, float playRate = 1f)
        {
            if (clip == null)
            {
                Debug.Log("PlayAnimation: AnimationClip is null");
                return;
            }
        
            if (_animationPlayable.IsValid())
            {
                _layerMixerPlayable.DisconnectInput(1);
            }

            var newAnimationPlayable = AnimationClipPlayable.Create(_playableGraph, clip);
            _playableGraph.Connect(newAnimationPlayable, 0, _layerMixerPlayable, 1);
            _layerMixerPlayable.SetInputWeight(1, 1f);

            newAnimationPlayable.SetTime(0);
            newAnimationPlayable.SetSpeed(playRate);

            _animationPlayable = newAnimationPlayable;
            _blendOutTime = blendOut;
            _blendOutTime.Validate();
        }

        public void OnGunEquipped(WeaponAnimData gunAimData)
        {
            rigData.gunData = gunAimData;
            rigData.masterDynamic.target = rigData.gunData.gunAimData.pivotPoint;
        }

        public void OnSightChanged(Transform newSight)
        {
            rigData.gunData.gunAimData.aimPoint = newSight;
        }
        
        public void SetCharData(CharAnimData data)
        {
            rigData.characterData = data;
        }

        public void SetRightHandIKWeight(float effector, float hint)
        {
            rightHandWeight = Tuple.Create(effector, hint);
        }
        
        public void SetLeftHandIKWeight(float effector, float hint)
        {
            leftHandWeight = Tuple.Create(effector, hint);
        }

        public void SetRightFootIKWeight(float effector, float hint)
        {
            rightFootWeight = Tuple.Create(effector, hint);
        }
        
        public void SetLeftFootIKWeight(float effector, float hint)
        {
            leftFootWeight = Tuple.Create(effector, hint);
        }

        // Editor utils
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (drawDebug)
            {
                Gizmos.color = Color.green;

                void DrawDynamicBone(ref DynamicBone bone, string boneName)
                {
                    if (bone.obj != null)
                    {
                        var loc = bone.obj.transform.position;
                        Gizmos.DrawWireSphere(loc, 0.06f);
                        Handles.Label(loc, boneName);
                    }
                }

                DrawDynamicBone(ref rigData.rightHand, "RightHandIK");
                DrawDynamicBone(ref rigData.leftHand, "LeftHandIK");
                DrawDynamicBone(ref rigData.rightFoot, "RightFootIK");
                DrawDynamicBone(ref rigData.leftFoot, "LeftFootIK");

                Gizmos.color = Color.blue;
                if (rigData.rootBone != null)
                {
                    var mainBone = rigData.rootBone.position;
                    Gizmos.DrawWireCube(mainBone, new Vector3(0.1f, 0.1f, 0.1f));
                    Handles.Label(mainBone, "rootBone");
                }
            }

            if (!Application.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
                SceneView.RepaintAll();
            }
        }
        
        public void SetupBones()
        {
            if (rigData.animator == null)
            {
                rigData.animator = GetComponent<Animator>();
            }
            
            if (rigData.rootBone == null)
            {
                var root = transform.Find("rootBone");

                if (root != null)
                {
                    rigData.rootBone = root.transform;
                }
                else
                {
                    var bone = new GameObject("rootBone");
                    bone.transform.parent = transform;
                    rigData.rootBone = bone.transform;
                    rigData.rootBone.localPosition = Vector3.zero;
                }
            }

            if (rigData.rightFoot.obj == null)
            {
                var bone = transform.Find("RightFootIK");

                if (bone != null)
                {
                    rigData.rightFoot.obj = bone.gameObject;
                }
                else
                {
                    rigData.rightFoot.obj = new GameObject("RightFootIK");
                    rigData.rightFoot.obj.transform.parent = transform;
                    rigData.rightFoot.obj.transform.localPosition = Vector3.zero;
                }
            }

            if (rigData.leftFoot.obj == null)
            {
                var bone = transform.Find("LeftFootIK");

                if (bone != null)
                {
                    rigData.leftFoot.obj = bone.gameObject;
                }
                else
                {
                    rigData.leftFoot.obj = new GameObject("LeftFootIK");
                    rigData.leftFoot.obj.transform.parent = transform;
                    rigData.leftFoot.obj.transform.localPosition = Vector3.zero;
                }
            }

            if (rigData.animator.isHuman)
            {
                rigData.pelvis = rigData.animator.GetBoneTransform(HumanBodyBones.Hips);
                rigData.rightHand.target = rigData.animator.GetBoneTransform(HumanBodyBones.RightHand);
                rigData.leftHand.target = rigData.animator.GetBoneTransform(HumanBodyBones.LeftHand);
                rigData.rightFoot.target = rigData.animator.GetBoneTransform(HumanBodyBones.RightFoot);
                rigData.leftFoot.target = rigData.animator.GetBoneTransform(HumanBodyBones.LeftFoot);

                Transform head = rigData.animator.GetBoneTransform(HumanBodyBones.Head);
                SetupIKBones(head);
                return;
            }

            if (skeleton == null)
            {
                Debug.Log("Error: skeleton ref is null!");
                return;
            }

            var children = skeleton.GetComponentsInChildren<Transform>(true);

            bool foundRightHand = false;
            bool foundLeftHand = false;
            bool foundRightFoot = false;
            bool foundLeftFoot = false;
            bool foundHead = false;
            bool foundPelvis = false;

            foreach (var bone in children)
            {
                if (bone.name.ToLower().Contains("ik"))
                {
                    continue;
                }

                bool bMatches = bone.name.ToLower().Contains("hips") || bone.name.ToLower().Contains("pelvis");
                if (!foundPelvis && bMatches)
                {
                    rigData.pelvis = bone;
                    foundPelvis = true;
                    continue;
                }

                bMatches = bone.name.ToLower().Contains("lefthand") || bone.name.ToLower().Contains("hand_l")
                                                                    || bone.name.ToLower().Contains("hand l")
                                                                    || bone.name.ToLower().Contains("l hand")
                                                                    || bone.name.ToLower().Contains("l.hand")
                                                                    || bone.name.ToLower().Contains("hand.l");
                if (!foundLeftHand && bMatches)
                {
                    rigData.leftHand.target = bone;

                    if (rigData.leftHand.hintTarget == null)
                    {
                        rigData.leftHand.hintTarget = bone.parent;
                    }

                    foundLeftHand = true;
                    continue;
                }

                bMatches = bone.name.ToLower().Contains("righthand") || bone.name.ToLower().Contains("hand_r")
                                                                     || bone.name.ToLower().Contains("hand r")
                                                                     || bone.name.ToLower().Contains("r hand")
                                                                     || bone.name.ToLower().Contains("r.hand")
                                                                     || bone.name.ToLower().Contains("hand.r");
                if (!foundRightHand && bMatches)
                {
                    rigData.rightHand.target = bone;

                    if (rigData.rightHand.hintTarget == null)
                    {
                        rigData.rightHand.hintTarget = bone.parent;
                    }

                    foundRightHand = true;
                }

                bMatches = bone.name.ToLower().Contains("rightfoot") || bone.name.ToLower().Contains("foot_r")
                                                                     || bone.name.ToLower().Contains("foot r")
                                                                     || bone.name.ToLower().Contains("r foot")
                                                                     || bone.name.ToLower().Contains("r.foot")
                                                                     || bone.name.ToLower().Contains("foot.r");
                if (!foundRightFoot && bMatches)
                {
                    rigData.rightFoot.target = bone;
                    rigData.rightFoot.hintTarget = bone.parent;

                    foundRightFoot = true;
                }

                bMatches = bone.name.ToLower().Contains("leftfoot") || bone.name.ToLower().Contains("foot_l")
                                                                    || bone.name.ToLower().Contains("foot l")
                                                                    || bone.name.ToLower().Contains("l foot")
                                                                    || bone.name.ToLower().Contains("l.foot")
                                                                    || bone.name.ToLower().Contains("foot.l");
                if (!foundLeftFoot && bMatches)
                {
                    rigData.leftFoot.target = bone;
                    rigData.leftFoot.hintTarget = bone.parent;

                    foundLeftFoot = true;
                }

                if (!foundHead && bone.name.ToLower().Contains("head"))
                {
                    SetupIKBones(bone);
                    foundHead = true;
                }
            }

            bool bFound = foundRightHand && foundLeftHand && foundRightFoot && foundLeftFoot && foundHead &&
                          foundPelvis;

            Debug.Log(bFound ? "All bones are found!" : "Some bones are missing!");
        }
        
        private void SetupIKBones(Transform head)
        {
            if (rigData.masterDynamic.obj == null)
            {
                var boneObject = head.transform.Find("MasterIK");

                if (boneObject != null)
                {
                    rigData.masterDynamic.obj = boneObject.gameObject;
                }
                else
                {
                    rigData.masterDynamic.obj = new GameObject("MasterIK");
                    rigData.masterDynamic.obj.transform.parent = head;
                    rigData.masterDynamic.obj.transform.localPosition = Vector3.zero;
                }
            }

            if (rigData.rightHand.obj == null)
            {
                var boneObject = head.transform.Find("RightHandIK");

                if (boneObject != null)
                {
                    rigData.rightHand.obj = boneObject.gameObject;
                }
                else
                {
                    rigData.rightHand.obj = new GameObject("RightHandIK");
                }

                rigData.rightHand.obj.transform.parent = rigData.masterDynamic.obj.transform;
                rigData.rightHand.obj.transform.localPosition = Vector3.zero;
            }

            if (rigData.leftHand.obj == null)
            {
                var boneObject = head.transform.Find("LeftHandIK");

                if (boneObject != null)
                {
                    rigData.leftHand.obj = boneObject.gameObject;
                }
                else
                {
                    rigData.leftHand.obj = new GameObject("LeftHandIK");
                }

                rigData.leftHand.obj.transform.parent = rigData.masterDynamic.obj.transform;
                rigData.leftHand.obj.transform.localPosition = Vector3.zero;
            }
        }

        public void AddLayer(AnimLayer newLayer)
        {
            animLayers.Add(newLayer);
        }

        public void RemoveLayer(int index)
        {
            if (index < 0 || index > animLayers.Count - 1)
            {
                return;
            }

            var toRemove = animLayers[index];
            animLayers.RemoveAt(index);
            DestroyImmediate(toRemove, true);
        }

        public bool IsLayerUnique(Type layer)
        {
            bool isUnique = true;
            foreach (var item in animLayers)
            {
                if (item.GetType() == layer)
                {
                    isUnique = false;
                    break;
                }
            }

            return isUnique;
        }

        public AnimLayer GetLayer(int index)
        {
            if (index < 0 || index > animLayers.Count - 1)
            {
                return null;
            }

            return animLayers[index];
        }
        
        public bool HasA(AnimLayer item)
        {
            return animLayers.Contains(item);
        }
#endif
    }
}