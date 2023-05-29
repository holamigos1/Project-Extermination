// Designed by Kinemation, 2023

using System.Collections.Generic;
using Plugins.Kinemation.FPSFramework.Runtime.Core;
using Plugins.Kinemation.FPSFramework.Runtime.Core.Data;
using UnityEngine;

namespace Plugins.Kinemation.FPSFramework.Runtime.Layers
{
	public class LookLayer : AnimLayer
	{
		[SerializeField, Range(0f, 1f)] protected float handsLayerAlpha;
		[SerializeField]                protected float handsLerpSpeed;

		[SerializeField, Range(0f, 1f)] protected float pelvisLayerAlpha = 1f;
		[SerializeField]                protected float pelvisLerpSpeed;

		[Header("Offsets")]
		[SerializeField] protected Vector3 pelvisOffset;

		[Header("Aim Offset")]
		[SerializeField] protected AimOffset lookUpOffset;
		[SerializeField]                   protected AimOffset lookRightOffset;
		[SerializeField]                   protected bool      enableAutoDistribution;
		[SerializeField]                   protected bool      enableManualSpineControl;
		[SerializeField, Range(-90f, 90f)] protected float     aimUp;
		[SerializeField, Range(-90f, 90f)] protected float     aimRight;
		[SerializeField]                   protected float     smoothAim;
		
		[Header("Leaning")]
		[SerializeField] [Range(-1, 1)] protected int leanDirection;
		[SerializeField] protected float leanAmount = 45f;
		[SerializeField] protected float leanSpeed;

		[Header("Misc")]
        
		[SerializeField] protected bool detectZeroFrames = true;
		[SerializeField] protected bool checkZeroFootIK = true;
		[SerializeField] protected bool useRightOffset  = true;
		// Used to detect zero key-frames
		[SerializeField, HideInInspector]  private CachedBones cachedBones;
		[SerializeField, HideInInspector]  private CachedBones cacheRef;

		protected float InterpHands;
		protected float InterpPelvis;

		protected float   leanInput;
		protected Vector2 lerpedAim;

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

			if (!lookUpOffset.IsValid || lookUpOffset.IsChanged)
			{
				lookUpOffset.Init();

				cachedBones.lookUp.Clear();
				cacheRef.lookUp.Clear();

				for (var i = 0; i < lookUpOffset.bones.Count; i++)
				{
					cachedBones.lookUp.Add(Quaternion.identity);
					cacheRef.lookUp.Add(Quaternion.identity);
				}
			}

			if (!lookRightOffset.IsValid || lookRightOffset.IsChanged)
				lookRightOffset.Init();

			void Distribute(ref AimOffset aimOffset)
			{
				if (enableAutoDistribution)
				{
					var enable = false;
					var divider = 1;
					var sum = 0f;

					for (var i = 0; i < aimOffset.bones.Count - aimOffset.indexOffset; i++)
					{
						if (enable)
						{
							AimOffsetBone bone = aimOffset.bones[i];
							bone.maxAngle.x = (90f - sum) / divider;
							aimOffset.bones[i] = bone;
							continue;
						}

						if (!Mathf.Approximately(aimOffset.bones[i].maxAngle.x, aimOffset.Angles[i].x))
						{
							divider = aimOffset.bones.Count - aimOffset.indexOffset - (i + 1);
							enable = true;
						}

						sum += aimOffset.bones[i].maxAngle.x;
					}
				}

				if (enableAutoDistribution)
				{
					var enable = false;
					var divider = 1;
					var sum = 0f;

					for (var i = 0; i < aimOffset.bones.Count - aimOffset.indexOffset; i++)
					{
						if (enable)
						{
							AimOffsetBone bone = aimOffset.bones[i];
							bone.maxAngle.y = (90f - sum) / divider;
							aimOffset.bones[i] = bone;
							continue;
						}

						if (!Mathf.Approximately(aimOffset.bones[i].maxAngle.y, aimOffset.Angles[i].y))
						{
							divider = aimOffset.bones.Count - aimOffset.indexOffset - (i + 1);
							enable = true;
						}

						sum += aimOffset.bones[i].maxAngle.y;
					}
				}

				for (var i = 0; i < aimOffset.bones.Count - aimOffset.indexOffset; i++)
					aimOffset.Angles[i] = aimOffset.bones[i].maxAngle;
			}

			if (lookUpOffset.bones.Count > 0)
				Distribute(ref lookUpOffset);

			if (lookRightOffset.bones.Count > 0)
				Distribute(ref lookRightOffset);
		}

		public void SetPelvisWeight(float weight) =>
			pelvisLayerAlpha = Mathf.Clamp01(weight);

		public void SetHandsWeight(float weight) =>
			handsLayerAlpha = Mathf.Clamp01(weight);

		public override void OnPreAnimUpdate()
		{
			base.OnPreAnimUpdate();
			if (detectZeroFrames)
				CheckZeroFrames();
		}

		public override void OnAnimUpdate() =>
			ApplySpineLayer();

		public override void OnPostIK()
		{
			if (detectZeroFrames)
				CacheBones();
		}

		private void CheckZeroFrames()
		{
			if (cachedBones.pelvis.Item1 == CoreAnim.rigData.pelvisBone.localPosition)
				CoreAnim.rigData.pelvisBone.localPosition = cacheRef.pelvis.Item1;

			if (cachedBones.pelvis.Item2 == CoreAnim.rigData.pelvisBone.localRotation)
			{
				CoreAnim.rigData.pelvisBone.localRotation = cacheRef.pelvis.Item2;

				if (checkZeroFootIK)
				{
					CoreAnim.rigData.rightFootBone.Retarget();
					CoreAnim.rigData.leftFootBone.Retarget();
				}
			}

			cacheRef.pelvis.Item2 = CoreAnim.rigData.pelvisBone.localRotation;

			var bZeroSpine = false;
			for (var i = 0; i < cachedBones.lookUp.Count; i++)
			{
				Transform bone = lookUpOffset.bones[i].boneTransform;
				if (bone == null || bone == CoreAnim.rigData.pelvisBone)
					continue;

				if (cachedBones.lookUp[i] == bone.localRotation)
				{
					bZeroSpine = true;
					bone.localRotation = cacheRef.lookUp[i];
				}
			}

			if (bZeroSpine)
			{
				CoreAnim.rigData.masterDynamicBone.Retarget();
				CoreAnim.rigData.rightHandBone.Retarget();
				CoreAnim.rigData.leftHandBone.Retarget();
			}

			cacheRef.pelvis.Item1 = CoreAnim.rigData.pelvisBone.localPosition;

			for (var i = 0; i < lookUpOffset.bones.Count; i++)
			{
				Transform bone = lookUpOffset.bones[i].boneTransform;
				if (bone == null)
					continue;

				cacheRef.lookUp[i] = bone.localRotation;
			}
		}

		private void CacheBones()
		{
			cachedBones.pelvis.Item1 = CoreAnim.rigData.pelvisBone.localPosition;
			cachedBones.pelvis.Item2 = CoreAnim.rigData.pelvisBone.localRotation;

			for (var i = 0; i < lookUpOffset.bones.Count; i++)
			{
				Transform bone = lookUpOffset.bones[i].boneTransform;
				if (bone == null || bone == CoreAnim.rigData.pelvisBone)
					continue;

				cachedBones.lookUp[i] = bone.localRotation;
			}
		}

		private bool BlendLayers() =>
			Mathf.Approximately(SmoothLayerAlpha, 0f);

		private void ApplySpineLayer()
		{
			if (BlendLayers())
				return;

			if (!enableManualSpineControl)
			{
				aimUp = CharData.totalAimInput.y;
				aimRight = CharData.totalAimInput.x;

				if (lookRightOffset.bones.Count == 0 || !useRightOffset)
					aimRight = 0f;

				leanInput = CoreToolkitLib.Glerp(leanInput, leanAmount * CharData.leanDirection,
												 leanSpeed);
			}
			else
			{
				leanInput = CoreToolkitLib.Glerp(leanInput, leanAmount * leanDirection, leanSpeed);
			}

			InterpPelvis = CoreToolkitLib.Glerp(InterpPelvis, pelvisLayerAlpha * SmoothLayerAlpha,
												pelvisLerpSpeed);

			Vector3 pelvisFinal = Vector3.Lerp(Vector3.zero, pelvisOffset, InterpPelvis);
			CoreToolkitLib.MoveInBoneSpace(RootBone, CoreAnim.rigData.pelvisBone, pelvisFinal, 1f);

			lerpedAim.y = CoreToolkitLib.GlerpLayer(lerpedAim.y, aimUp, smoothAim);
			lerpedAim.x = CoreToolkitLib.GlerpLayer(lerpedAim.x, aimRight, smoothAim);

			foreach (AimOffsetBone bone in lookRightOffset.bones)
			{
				if (!Application.isPlaying && bone.boneTransform == null)
					continue;

				float angleFraction = lerpedAim.x >= 0f ?
					bone.maxAngle.y :
					bone.maxAngle.x;
				CoreToolkitLib.RotateInBoneSpace(RootBone.rotation, bone.boneTransform,
												 Quaternion.Euler(
													 0f, lerpedAim.x * SmoothLayerAlpha / (90f / angleFraction), 0f),
												 1f);
			}

			foreach (AimOffsetBone bone in lookRightOffset.bones)
			{
				if (!Application.isPlaying && bone.boneTransform == null)
					continue;

				float angleFraction = bone.maxAngle.x;
				CoreToolkitLib.RotateInBoneSpace(
					RootBone.rotation * Quaternion.Euler(0f, lerpedAim.x, 0f), bone.boneTransform,
					Quaternion.Euler(0f, 0f, leanInput * SmoothLayerAlpha / (90f / angleFraction)), 1f);
			}

			Vector3 rightHandLoc = CoreAnim.rigData.rightHandBone.obj.transform.position;
			Quaternion rightHandRot = CoreAnim.rigData.rightHandBone.obj.transform.rotation;

			Vector3 leftHandLoc = CoreAnim.rigData.leftHandBone.obj.transform.position;
			Quaternion leftHandRot = CoreAnim.rigData.leftHandBone.obj.transform.rotation;

			foreach (AimOffsetBone bone in lookUpOffset.bones)
			{
				if (!Application.isPlaying && bone.boneTransform == null)
					continue;

				float angleFraction = lerpedAim.y >= 0f ?
					bone.maxAngle.y :
					bone.maxAngle.x;

				CoreToolkitLib.RotateInBoneSpace(RootBone.rotation * Quaternion.Euler(0f, lerpedAim.x, 0f),
												 bone.boneTransform,
												 Quaternion.Euler(
													 lerpedAim.y * SmoothLayerAlpha / (90f / angleFraction), 0f, 0f),
												 1f);
			}

			InterpHands = CoreToolkitLib.GlerpLayer(InterpHands, handsLayerAlpha, handsLerpSpeed);

			CoreAnim.rigData.rightHandBone.obj.transform.position = Vector3.Lerp(rightHandLoc,
																		 CoreAnim.rigData.rightHandBone.obj.transform.position,
																		 InterpHands);
			CoreAnim.rigData.rightHandBone.obj.transform.rotation = Quaternion.Slerp(rightHandRot,
																			 CoreAnim.rigData.rightHandBone.obj.transform
																				 .rotation,
																			 InterpHands);

			CoreAnim.rigData.leftHandBone.obj.transform.position = Vector3.Lerp(
				leftHandLoc, CoreAnim.rigData.leftHandBone.obj.transform.position,
				InterpHands);
			CoreAnim.rigData.leftHandBone.obj.transform.rotation = Quaternion.Slerp(leftHandRot,
																			CoreAnim.rigData.leftHandBone.obj.transform
																				.rotation,
																			InterpHands);
		}
	}
}