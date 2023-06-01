// Designed by Kinemation, 2023

using System;
using System.Collections.Generic;
using Plugins.Kinemation.FPSFramework.Runtime.Core.Data;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;
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
		[ Header("Essentials")]
		[ Tooltip("The armature root, used for manual bone search")]
		[ SerializeField]
		private Transform skeleton;

		[Tooltip("Used to play animation from code")]
		public AvatarMask upperBodyMask;

		[FormerlySerializedAs("rigData")] 
		[LabelText("Данные о костях")]
		public DynamicRigData _rigData;

		[SerializeField] 
		[HideInInspector]
		private List<AnimLayer> animLayers;
		
		[SerializeField]                     
		private bool useIK = true;

		[Header("Misc")]
		
		[SerializeField] 
		private bool drawDebug;
		
		private AnimationClipPlayable       _animationPlayable;
		private BlendTime                   _blendOutTime;
		private float                       _interpHands;
		private float                       _interpLayer;
		private AnimationLayerMixerPlayable _layerMixerPlayable;

		private PlayableGraph _playableGraph;

		private bool _updateInEditor;
		
		private Tuple<float, float> _leftFootWeight  = new(1f, 1f);
		private Tuple<float, float> _leftHandWeight  = new(1f, 1f);
		private Tuple<float, float> _rightFootWeight = new(1f, 1f);
		private Tuple<float, float> _rightHandWeight = new(1f, 1f);

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
				if (_rigData.rigAnimator == null)
					return;

				_rigData.rigAnimator.Update(Time.deltaTime);
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

				Transform lowerBone = tipBone._bone.parent;
				CoreToolkitLib.SolveTwoBoneIK(lowerBone.parent, lowerBone, tipBone._bone,
											  tipBone._boneObject.transform, tipBone._boneHint, weights.Item1, weights.Item1,
											  weights.Item2);
			}

			SolveIK(_rigData.rightHandBone, _rightHandWeight);
			SolveIK(_rigData.leftHandBone, _leftHandWeight);
			SolveIK(_rigData.rightFootBone, _rightFootWeight);
			SolveIK(_rigData.leftFootBone, _leftFootWeight);
		}

		private void InitPlayableGraph()
		{
			if (!Application.isPlaying)
				return;

			Animator animator = _rigData.rigAnimator;

			_playableGraph = animator.playableGraph;
			_playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

			_layerMixerPlayable = AnimationLayerMixerPlayable.Create(_playableGraph, 2);

			AnimationPlayableOutput output = AnimationPlayableOutput.Create(_playableGraph, "FPSAnimator", animator);
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

			_rigData.Retarget();
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
			if (_rigData.rigAnimator == null)
				_rigData.rigAnimator = GetComponent<Animator>();

			_updateInEditor = true;
		}

		public void DisableEditorPreview()
		{
			_updateInEditor = false;

			if (_rigData.rigAnimator == null)
				return;

			_rigData.rigAnimator.Rebind();
			_rigData.rigAnimator.Update(0f);
		}

		public Transform GetRootBone() =>
			_rigData.rootBone;

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
			_rigData.gunData = gunAimData;
			_rigData.masterDynamicBone._bone = _rigData.gunData.gunAimData.pivotPoint;
		}

		public void OnSightChanged(Transform newSight) =>
			_rigData.gunData.gunAimData.aimPoint = newSight;

		public void SetCharData(CharAnimData data) =>
			_rigData.characterData = data;

		public void SetRightHandIKWeight(float effector, float hint) =>
			_rightHandWeight = Tuple.Create(effector, hint);

		public void SetLeftHandIKWeight(float effector, float hint) =>
			_leftHandWeight = Tuple.Create(effector, hint);

		public void SetRightFootIKWeight(float effector, float hint) =>
			_rightFootWeight = Tuple.Create(effector, hint);

		public void SetLeftFootIKWeight(float effector, float hint) =>
			_leftFootWeight = Tuple.Create(effector, hint);

		// Editor utils
	#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (drawDebug)
			{
				Gizmos.color = Color.green;

				void DrawDynamicBone(ref DynamicBone bone, string boneName)
				{
					if (bone._boneObject != null)
					{
						Vector3 loc = bone._boneObject.transform.position;
						Gizmos.DrawWireSphere(loc, 0.06f);
						Handles.Label(loc, boneName);
					}
				}

				DrawDynamicBone(ref _rigData.rightHandBone, "RightHandIK");
				DrawDynamicBone(ref _rigData.leftHandBone, "LeftHandIK");
				DrawDynamicBone(ref _rigData.rightFootBone, "RightFootIK");
				DrawDynamicBone(ref _rigData.leftFootBone, "LeftFootIK");

				Gizmos.color = Color.blue;
				if (_rigData.rootBone != null)
				{
					Vector3 mainBone = _rigData.rootBone.position;
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
			if (_rigData.rigAnimator == null)
				_rigData.rigAnimator = GetComponent<Animator>();

			if (_rigData.rootBone == null)
			{
				Transform root = transform.Find("rootBone"); //TODO Сделать базу имён костей с разных программ

				if (root != null)
				{
					_rigData.rootBone = root.transform;
				}
				else
				{
					var bone = new GameObject("rootBone");
					bone.transform.parent = transform;
					_rigData.rootBone = bone.transform;
					_rigData.rootBone.localPosition = Vector3.zero;
				}
			}

			if (_rigData.rightFootBone._boneObject == null)
			{
				Transform bone = transform.Find("RightFootIK"); //TODO Сделать базу имён костей с разных программ

				if (bone != null)
				{
					_rigData.rightFootBone._boneObject = bone.gameObject;
				}
				else
				{
					_rigData.rightFootBone._boneObject = new GameObject("RightFootIK");
					_rigData.rightFootBone._boneObject.transform.parent = transform;
					_rigData.rightFootBone._boneObject.transform.localPosition = Vector3.zero;
				}
			}

			if (_rigData.leftFootBone._boneObject == null)
			{
				Transform bone = transform.Find("LeftFootIK"); //TODO Сделать базу имён костей с разных программ

				if (bone != null)
				{
					_rigData.leftFootBone._boneObject = bone.gameObject;
				}
				else
				{
					_rigData.leftFootBone._boneObject = new GameObject("LeftFootIK");
					_rigData.leftFootBone._boneObject.transform.parent = transform;
					_rigData.leftFootBone._boneObject.transform.localPosition = Vector3.zero;
				}
			}

			if (_rigData.rigAnimator.isHuman)
			{
				_rigData.pelvisBone = _rigData.rigAnimator.GetBoneTransform(HumanBodyBones.Hips);
				_rigData.rightHandBone._bone = _rigData.rigAnimator.GetBoneTransform(HumanBodyBones.RightHand);
				_rigData.leftHandBone._bone = _rigData.rigAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
				_rigData.rightFootBone._bone = _rigData.rigAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
				_rigData.leftFootBone._bone = _rigData.rigAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);

				Transform head = _rigData.rigAnimator.GetBoneTransform(HumanBodyBones.Head);
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
					_rigData.pelvisBone = bone;
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
					_rigData.leftHandBone._bone = bone;

					if (_rigData.leftHandBone._boneHint == null)
						_rigData.leftHandBone._boneHint = bone.parent;

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
					_rigData.rightHandBone._bone = bone;

					if (_rigData.rightHandBone._boneHint == null)
						_rigData.rightHandBone._boneHint = bone.parent;

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
					_rigData.rightFootBone._bone = bone;
					_rigData.rightFootBone._boneHint = bone.parent;

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
					_rigData.leftFootBone._bone = bone;
					_rigData.leftFootBone._boneHint = bone.parent;

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
			if (_rigData.masterDynamicBone._boneObject == null)
			{
				Transform boneObject = head.transform.Find("MasterIK");

				if (boneObject != null)
				{
					_rigData.masterDynamicBone._boneObject = boneObject.gameObject;
				}
				else
				{
					_rigData.masterDynamicBone._boneObject = new GameObject("MasterIK");
					_rigData.masterDynamicBone._boneObject.transform.parent = head;
					_rigData.masterDynamicBone._boneObject.transform.localPosition = Vector3.zero;
				}
			}

			if (_rigData.rightHandBone._boneObject == null)
			{
				Transform boneObject =
					head.transform.Find("RightHandIK"); //TODO Сделать базу имён костей с разных программ

				if (boneObject != null)
					_rigData.rightHandBone._boneObject = boneObject.gameObject;
				else
					_rigData.rightHandBone._boneObject = new GameObject("RightHandIK");

				_rigData.rightHandBone._boneObject.transform.parent = _rigData.masterDynamicBone._boneObject.transform;
				_rigData.rightHandBone._boneObject.transform.localPosition = Vector3.zero;
			}

			if (_rigData.leftHandBone._boneObject == null)
			{
				Transform boneObject = head.transform.Find("LeftHandIK");

				if (boneObject != null)
					_rigData.leftHandBone._boneObject = boneObject.gameObject;
				else
					_rigData.leftHandBone._boneObject = new GameObject("LeftHandIK");

				_rigData.leftHandBone._boneObject.transform.parent = _rigData.masterDynamicBone._boneObject.transform;
				_rigData.leftHandBone._boneObject.transform.localPosition = Vector3.zero;
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