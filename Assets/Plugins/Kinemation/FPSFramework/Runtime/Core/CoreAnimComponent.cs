// Designed by Kinemation, 2023

using System;
using System.Collections.Generic;
using Plugins.Kinemation.FPSFramework.Runtime.Core.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core
{
	// Collection of AimOffsetBones, used to rotate spine bones to look around

	// DynamicBone is essentially an IK bone

	// Used for detecting zero-frames

	// Essential skeleton data, used by Anim Layers

	[ExecuteAlways]
	public class CoreAnimComponent : MonoBehaviour
	{
		[Header("Essentials"), Tooltip("The armature root, used for manual bone search"), SerializeField] 
        
		private Transform skeleton;

		[Tooltip("Used to play animation from code")]
		public AvatarMask upperBodyMask;

		public DynamicRigData rigData;

		[SerializeField, HideInInspector]  private List<AnimLayer> animLayers;
		[SerializeField]                   private bool            useIK = true;

		[Header("Misc"), SerializeField] 
        
		private bool drawDebug;

		private AnimationClipPlayable       _animationPlayable;
		private BlendTime                   _blendOutTime;
		private float                       _interpHands;
		private float                       _interpLayer;
		private AnimationLayerMixerPlayable _layerMixerPlayable;

		private PlayableGraph _playableGraph;

		private bool                _updateInEditor;
		private Tuple<float, float> leftFootWeight  = new(1f, 1f);
		private Tuple<float, float> leftHandWeight  = new(1f, 1f);
		private Tuple<float, float> rightFootWeight = new(1f, 1f);

		private Tuple<float, float> rightHandWeight = new(1f, 1f);

		private void Start()
		{
			foreach (AnimLayer layer in animLayers)
				layer.OnAnimStart();

			InitPlayableGraph();
		}

		private void Update()
		{
			if (!Application.isPlaying && _updateInEditor)
			{
				if (rigData.rigAnimator == null)
					return;

				rigData.rigAnimator.Update(Time.deltaTime);
			}

			BlendOutAnimation();
		}

		private void LateUpdate()
		{
			if (!Application.isPlaying && !_updateInEditor)
				return;

			Retarget();
			PreUpdateLayers();
			UpdateLayers();
			ApplyIK();
			PostUpdateLayers();
		}

		private void OnEnable() =>
			animLayers ??= new List<AnimLayer>();

		private void OnDestroy()
		{
			if (!_playableGraph.IsValid())
				return;

			_playableGraph.Stop();
			_playableGraph.Destroy();
		}

		private void ApplyIK()
		{
			if (!useIK)
				return;

			void SolveIK(DynamicBone tipBone, Tuple<float, float> weights)
			{
				if (Mathf.Approximately(weights.Item1, 0f))
					return;

				Transform lowerBone = tipBone.target.parent;
				CoreToolkitLib.SolveTwoBoneIK(lowerBone.parent, lowerBone, tipBone.target,
											  tipBone.obj.transform, tipBone.hintTarget, weights.Item1, weights.Item1,
											  weights.Item2);
			}

			SolveIK(rigData.rightHandBone, rightHandWeight);
			SolveIK(rigData.leftHandBone, leftHandWeight);
			SolveIK(rigData.rightFootBone, rightFootWeight);
			SolveIK(rigData.leftFootBone, leftFootWeight);
		}

		private void InitPlayableGraph()
		{
			if (!Application.isPlaying)
				return;

			Animator animator = rigData.rigAnimator;

			_playableGraph = animator.playableGraph;
			_playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

			_layerMixerPlayable = AnimationLayerMixerPlayable.Create(_playableGraph, 2);

			var output = AnimationPlayableOutput.Create(_playableGraph, "FPSAnimator", animator);
			output.SetSourcePlayable(_layerMixerPlayable);

			var controllerPlayable =
				AnimatorControllerPlayable.Create(_playableGraph, animator.runtimeAnimatorController);
			_playableGraph.Connect(controllerPlayable, 0, _layerMixerPlayable, 0);
			_layerMixerPlayable.SetInputWeight(0, 1f);

			_layerMixerPlayable.SetLayerMaskFromAvatarMask(1, upperBodyMask);

			_playableGraph.Play();
		}

		private void BlendOutAnimation()
		{
			if (!Application.isPlaying || !_animationPlayable.IsValid())
				return;

			var time = (float)_animationPlayable.GetTime();
			float clipLength = _animationPlayable.GetAnimationClip().length;

			time = Mathf.Max(time, 0.001f);
			clipLength = Mathf.Max(clipLength, 0.001f);

			float timeRatio = Mathf.Min(time / clipLength, _blendOutTime.blendEnd);
			if (timeRatio >= _blendOutTime.blendStart)
			{
				float progress = (timeRatio - _blendOutTime.blendStart)
								 / Mathf.Max(_blendOutTime.blendEnd - _blendOutTime.blendStart, 0.001f);
				float weight = Mathf.Lerp(1f, 0f, Mathf.Sin(progress * Mathf.PI / 2));
				_layerMixerPlayable.SetInputWeight(1, weight);

				// When reaching the end of the blending, disconnect the playable
				if (Mathf.Approximately(_blendOutTime.blendEnd, timeRatio))
					_layerMixerPlayable.DisconnectInput(1);
			}
		}

		private void Retarget()
		{
			foreach (AnimLayer layer in animLayers)
			{
				if (!Application.isPlaying && !layer.runInEditor)
					continue;

				layer.OnRetarget(this);
			}

			rigData.Retarget();
		}

		// Called right after retargeting
		private void PreUpdateLayers()
		{
			foreach (AnimLayer layer in animLayers)
			{
				if (!Application.isPlaying && !layer.runInEditor)
					continue;

				layer.OnPreAnimUpdate();
			}
		}

		private void UpdateLayers()
		{
			foreach (AnimLayer layer in animLayers)
			{
				if (!Application.isPlaying && !layer.runInEditor)
					continue;

				layer.OnAnimUpdate();
			}
		}

		// Called after IK update
		private void PostUpdateLayers()
		{
			foreach (AnimLayer layer in animLayers)
			{
				if (!Application.isPlaying && !layer.runInEditor)
					continue;

				layer.OnPostIK();
			}
		}

		public void EnableEditorPreview()
		{
			if (rigData.rigAnimator == null)
				rigData.rigAnimator = GetComponent<Animator>();

			_updateInEditor = true;
		}

		public void DisableEditorPreview()
		{
			_updateInEditor = false;

			if (rigData.rigAnimator == null)
				return;

			rigData.rigAnimator.Rebind();
			rigData.rigAnimator.Update(0f);
		}

		public Transform GetRootBone() =>
			rigData.rootBone;

		public void PlayAnimation(AnimationClip clip, BlendTime blendOut, float playRate = 1f)
		{
			if (clip == null)
			{
				Debug.Log("PlayAnimation: AnimationClip is null");
				return;
			}

			if (_animationPlayable.IsValid())
				_layerMixerPlayable.DisconnectInput(1);

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
			rigData.masterDynamicBone.target = rigData.gunData.gunAimData.pivotPoint;
		}

		public void OnSightChanged(Transform newSight) =>
			rigData.gunData.gunAimData.aimPoint = newSight;

		public void SetCharData(CharAnimData data) =>
			rigData.characterData = data;

		public void SetRightHandIKWeight(float effector, float hint) =>
			rightHandWeight = Tuple.Create(effector, hint);

		public void SetLeftHandIKWeight(float effector, float hint) =>
			leftHandWeight = Tuple.Create(effector, hint);

		public void SetRightFootIKWeight(float effector, float hint) =>
			rightFootWeight = Tuple.Create(effector, hint);

		public void SetLeftFootIKWeight(float effector, float hint) =>
			leftFootWeight = Tuple.Create(effector, hint);

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
						Vector3 loc = bone.obj.transform.position;
						Gizmos.DrawWireSphere(loc, 0.06f);
						Handles.Label(loc, boneName);
					}
				}

				DrawDynamicBone(ref rigData.rightHandBone, "RightHandIK");
				DrawDynamicBone(ref rigData.leftHandBone, "LeftHandIK");
				DrawDynamicBone(ref rigData.rightFootBone, "RightFootIK");
				DrawDynamicBone(ref rigData.leftFootBone, "LeftFootIK");

				Gizmos.color = Color.blue;
				if (rigData.rootBone != null)
				{
					Vector3 mainBone = rigData.rootBone.position;
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
			if (rigData.rigAnimator == null)
				rigData.rigAnimator = GetComponent<Animator>();

			if (rigData.rootBone == null)
			{
				Transform root = transform.Find("rootBone"); //TODO Сделать базу имён костей с разных программ

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

			if (rigData.rightFootBone.obj == null)
			{
				Transform bone = transform.Find("RightFootIK"); //TODO Сделать базу имён костей с разных программ

				if (bone != null)
				{
					rigData.rightFootBone.obj = bone.gameObject;
				}
				else
				{
					rigData.rightFootBone.obj = new GameObject("RightFootIK");
					rigData.rightFootBone.obj.transform.parent = transform;
					rigData.rightFootBone.obj.transform.localPosition = Vector3.zero;
				}
			}

			if (rigData.leftFootBone.obj == null)
			{
				Transform bone = transform.Find("LeftFootIK"); //TODO Сделать базу имён костей с разных программ

				if (bone != null)
				{
					rigData.leftFootBone.obj = bone.gameObject;
				}
				else
				{
					rigData.leftFootBone.obj = new GameObject("LeftFootIK");
					rigData.leftFootBone.obj.transform.parent = transform;
					rigData.leftFootBone.obj.transform.localPosition = Vector3.zero;
				}
			}

			if (rigData.rigAnimator.isHuman)
			{
				rigData.pelvisBone = rigData.rigAnimator.GetBoneTransform(HumanBodyBones.Hips);
				rigData.rightHandBone.target = rigData.rigAnimator.GetBoneTransform(HumanBodyBones.RightHand);
				rigData.leftHandBone.target = rigData.rigAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
				rigData.rightFootBone.target = rigData.rigAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
				rigData.leftFootBone.target = rigData.rigAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);

				Transform head = rigData.rigAnimator.GetBoneTransform(HumanBodyBones.Head);
				SetupIKBones(head);
				return;
			}

			if (skeleton == null)
			{
				Debug.Log("Error: skeleton ref is null!");
				return;
			}

			var children = skeleton.GetComponentsInChildren<Transform>(true);

			var foundRightHand = false;
			var foundLeftHand = false;
			var foundRightFoot = false;
			var foundLeftFoot = false;
			var foundHead = false;
			var foundPelvis = false;

			foreach (Transform bone in children)
			{
				if (bone.name.ToLower().Contains("ik"))
					continue;

				bool bMatches = bone.name.ToLower().Contains("hips") || bone.name.ToLower().Contains("pelvis");
				if (!foundPelvis && bMatches)
				{
					rigData.pelvisBone = bone;
					foundPelvis = true;
					continue;
				}

				bMatches = bone.name.ToLower().Contains("lefthand")
						   || bone.name.ToLower().Contains("hand_l") //TODO Сделать базу имён костей с разных программ
						   || bone.name.ToLower().Contains("hand l")
						   || bone.name.ToLower().Contains("l hand")
						   || bone.name.ToLower().Contains("l.hand")
						   || bone.name.ToLower().Contains("hand.l");
				if (!foundLeftHand && bMatches)
				{
					rigData.leftHandBone.target = bone;

					if (rigData.leftHandBone.hintTarget == null)
						rigData.leftHandBone.hintTarget = bone.parent;

					foundLeftHand = true;
					continue;
				}

				bMatches = bone.name.ToLower().Contains("righthand")
						   || bone.name.ToLower().Contains("hand_r") //TODO Сделать базу имён костей с разных программ
						   || bone.name.ToLower().Contains("hand r")
						   || bone.name.ToLower().Contains("r hand")
						   || bone.name.ToLower().Contains("r.hand")
						   || bone.name.ToLower().Contains("hand.r");
				if (!foundRightHand && bMatches)
				{
					rigData.rightHandBone.target = bone;

					if (rigData.rightHandBone.hintTarget == null)
						rigData.rightHandBone.hintTarget = bone.parent;

					foundRightHand = true;
				}

				bMatches = bone.name.ToLower().Contains("rightfoot")
						   || bone.name.ToLower().Contains("foot_r") //TODO Сделать базу имён костей с разных программ
						   || bone.name.ToLower().Contains("foot r")
						   || bone.name.ToLower().Contains("r foot")
						   || bone.name.ToLower().Contains("r.foot")
						   || bone.name.ToLower().Contains("foot.r");
				if (!foundRightFoot && bMatches)
				{
					rigData.rightFootBone.target = bone;
					rigData.rightFootBone.hintTarget = bone.parent;

					foundRightFoot = true;
				}

				bMatches = bone.name.ToLower().Contains("leftfoot")
						   || bone.name.ToLower().Contains("foot_l") //TODO Сделать базу имён костей с разных программ
						   || bone.name.ToLower().Contains("foot l")
						   || bone.name.ToLower().Contains("l foot")
						   || bone.name.ToLower().Contains("l.foot")
						   || bone.name.ToLower().Contains("foot.l");
				if (!foundLeftFoot && bMatches)
				{
					rigData.leftFootBone.target = bone;
					rigData.leftFootBone.hintTarget = bone.parent;

					foundLeftFoot = true;
				}

				if (!foundHead && bone.name.ToLower().Contains("head"))
				{
					SetupIKBones(bone);
					foundHead = true;
				}
			}

			bool bFound = foundRightHand
						  && foundLeftHand
						  && foundRightFoot
						  && foundLeftFoot
						  && foundHead
						  && foundPelvis;

			Debug.Log(bFound ?
						  "All bones are found!" :
						  "Some bones are missing!");
		}

		private void SetupIKBones(Transform head)
		{
			if (rigData.masterDynamicBone.obj == null)
			{
				Transform boneObject = head.transform.Find("MasterIK");

				if (boneObject != null)
				{
					rigData.masterDynamicBone.obj = boneObject.gameObject;
				}
				else
				{
					rigData.masterDynamicBone.obj = new GameObject("MasterIK");
					rigData.masterDynamicBone.obj.transform.parent = head;
					rigData.masterDynamicBone.obj.transform.localPosition = Vector3.zero;
				}
			}

			if (rigData.rightHandBone.obj == null)
			{
				Transform boneObject =
					head.transform.Find("RightHandIK"); //TODO Сделать базу имён костей с разных программ

				if (boneObject != null)
					rigData.rightHandBone.obj = boneObject.gameObject;
				else
					rigData.rightHandBone.obj = new GameObject("RightHandIK");

				rigData.rightHandBone.obj.transform.parent = rigData.masterDynamicBone.obj.transform;
				rigData.rightHandBone.obj.transform.localPosition = Vector3.zero;
			}

			if (rigData.leftHandBone.obj == null)
			{
				Transform boneObject = head.transform.Find("LeftHandIK");

				if (boneObject != null)
					rigData.leftHandBone.obj = boneObject.gameObject;
				else
					rigData.leftHandBone.obj = new GameObject("LeftHandIK");

				rigData.leftHandBone.obj.transform.parent = rigData.masterDynamicBone.obj.transform;
				rigData.leftHandBone.obj.transform.localPosition = Vector3.zero;
			}
		}

		public void AddLayer(AnimLayer newLayer) =>
			animLayers.Add(newLayer);

		public void RemoveLayer(int index)
		{
			if (index < 0 || index > animLayers.Count - 1)
				return;

			AnimLayer toRemove = animLayers[index];
			animLayers.RemoveAt(index);
			DestroyImmediate(toRemove, true);
		}

		public bool IsLayerUnique(Type layer)
		{
			var isUnique = true;
			foreach (AnimLayer item in animLayers)
				if (item.GetType() == layer)
				{
					isUnique = false;
					break;
				}

			return isUnique;
		}

		public AnimLayer GetLayer(int index)
		{
			if (index < 0 || index > animLayers.Count - 1)
				return null;

			return animLayers[index];
		}

		public bool HasA(AnimLayer item) =>
			animLayers.Contains(item);
	#endif
	}
}