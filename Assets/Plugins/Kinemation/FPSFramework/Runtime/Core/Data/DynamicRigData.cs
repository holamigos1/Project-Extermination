using System;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	[Serializable]
	public struct DynamicRigData
	{
		[FormerlySerializedAs("animator")]      public Animator    rigAnimator;
		[FormerlySerializedAs("pelvis")]        public Transform   pelvisBone;
		[FormerlySerializedAs("masterDynamic")] public DynamicBone masterDynamicBone;
		[FormerlySerializedAs("rightHand")]     public DynamicBone rightHandBone;
		[FormerlySerializedAs("leftHand")]      public DynamicBone leftHandBone;
		[FormerlySerializedAs("rightFoot")]     public DynamicBone rightFootBone;
		[FormerlySerializedAs("leftFoot")]      public DynamicBone leftFootBone;

		[Tooltip("Used for mesh space calculations")]
		public Transform rootBone;

		public WeaponAnimData gunData;

		public CharAnimData characterData;

		public void Retarget()
		{
			masterDynamicBone.Retarget();
			rightHandBone.Retarget();
			leftHandBone.Retarget();
			rightFootBone.Retarget();
			leftFootBone.Retarget();
		}
	}
}