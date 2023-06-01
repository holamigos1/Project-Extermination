using System;
using Plugins.Kinemation.FPSFramework.Runtime.Core.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core
{
	[Serializable]
	public abstract class AnimLayer : MonoBehaviour
	{
		[Header("Смешиваемость слоя")]
		
		[Range(0f, 1f)]
		[LabelText("Вес этого слоя")]
		[SuffixLabel("Коэффициент")]
		[Tooltip("На сколько этот слой важен относительно других анимационных слоёв")]
		[SerializeField] protected float layerAlpha = 1f;
		
		[LabelText("Скорость смешивания слоёв")]
		[SuffixLabel("Секунды")]
		[SerializeField] protected float lerpSpeed;

		[Header("Редактор")]
		
		[LabelText("Проигрывается из редактора")]
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
			CoreAnim._rigData.gunData;

		protected CharAnimData CharData =>
			CoreAnim._rigData.characterData;

		protected Transform MasterIK =>
			CoreAnim._rigData.masterDynamicBone._boneObject.transform;

		protected Transform RootBone =>
			CoreAnim._rigData.rootBone;

		protected Transform Pelvis_bone =>
			CoreAnim._rigData.pelvisBone;

		protected DynamicBone MasterPivot =>
			CoreAnim._rigData.masterDynamicBone;

		protected DynamicBone Right_hand_bone =>
			CoreAnim._rigData.rightHandBone;

		protected DynamicBone Left_hand_bone =>
			CoreAnim._rigData.leftHandBone;

		protected DynamicBone Right_foot_bone =>
			CoreAnim._rigData.rightFootBone;

		protected DynamicBone Left_foot_bone =>
			CoreAnim._rigData.leftFootBone;

		protected Animator Rig_animator =>
			CoreAnim._rigData.rigAnimator;

		// Offsets master pivot only, without affecting the child IK bones
		// Useful if weapon has multiple pivots
		protected void OffsetMasterPivot(LocationAndRotation offset)
		{
			var rightHandTip = new LocationAndRotation(Right_hand_bone._boneObject.transform);
			var leftHandTip = new LocationAndRotation(Left_hand_bone._boneObject.transform);

			MasterPivot.Move(MasterIK, offset.position, 1f);
			MasterPivot.Rotate(MasterIK.rotation, offset.rotation, 1f);

			Right_hand_bone._boneObject.transform.position = rightHandTip.position;
			Right_hand_bone._boneObject.transform.rotation = rightHandTip.rotation;

			Left_hand_bone._boneObject.transform.position = leftHandTip.position;
			Left_hand_bone._boneObject.transform.rotation = leftHandTip.rotation;
		}
	}
}