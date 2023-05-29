using System;
using Plugins.Kinemation.FPSFramework.Runtime.Core.Data;
using UnityEngine;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core
{
	[Serializable]
	public abstract class AnimLayer : MonoBehaviour
	{
		[Header("Layer Blending")]
		[Range(0f, 1f)]
		[SerializeField] protected float layerAlpha = 1f;
		[SerializeField] protected float lerpSpeed;

		[Header("Misc")]
		[SerializeField] public bool runInEditor;

		protected CoreAnimComponent CoreAnim;
		protected float             SmoothLayerAlpha;

		protected virtual void Awake()
		{ }

		public void SetLayerAlpha(float weight) =>
			layerAlpha = Mathf.Clamp01(weight);

		public virtual void OnAnimStart()
		{ }

		public void OnRetarget(CoreAnimComponent comp) =>
			CoreAnim = comp;

		public virtual void OnPreAnimUpdate() =>
			SmoothLayerAlpha = CoreToolkitLib.GlerpLayer(SmoothLayerAlpha, layerAlpha, lerpSpeed);

		public virtual void OnAnimUpdate()
		{ }

		public virtual void OnPostIK()
		{ }

		protected WeaponAnimData GunData =>
			CoreAnim.rigData.gunData;

		protected CharAnimData CharData =>
			CoreAnim.rigData.characterData;

		protected Transform MasterIK =>
			CoreAnim.rigData.masterDynamicBone.obj.transform;

		protected Transform RootBone =>
			CoreAnim.rigData.rootBone;

		protected Transform Pelvis_bone =>
			CoreAnim.rigData.pelvisBone;

		protected DynamicBone MasterPivot =>
			CoreAnim.rigData.masterDynamicBone;

		protected DynamicBone Right_hand_bone =>
			CoreAnim.rigData.rightHandBone;

		protected DynamicBone Left_hand_bone =>
			CoreAnim.rigData.leftHandBone;

		protected DynamicBone Right_foot_bone =>
			CoreAnim.rigData.rightFootBone;

		protected DynamicBone Left_foot_bone =>
			CoreAnim.rigData.leftFootBone;

		protected Animator Rig_animator =>
			CoreAnim.rigData.rigAnimator;

		// Offsets master pivot only, without affecting the child IK bones
		// Useful if weapon has multiple pivots
		protected void OffsetMasterPivot(LocationAndRotation offset)
		{
			var rightHandTip = new LocationAndRotation(Right_hand_bone.obj.transform);
			var leftHandTip = new LocationAndRotation(Left_hand_bone.obj.transform);

			MasterPivot.Move(MasterIK, offset.position, 1f);
			MasterPivot.Rotate(MasterIK.rotation, offset.rotation, 1f);

			Right_hand_bone.obj.transform.position = rightHandTip.position;
			Right_hand_bone.obj.transform.rotation = rightHandTip.rotation;

			Left_hand_bone.obj.transform.position = leftHandTip.position;
			Left_hand_bone.obj.transform.rotation = leftHandTip.rotation;
		}
	}
}