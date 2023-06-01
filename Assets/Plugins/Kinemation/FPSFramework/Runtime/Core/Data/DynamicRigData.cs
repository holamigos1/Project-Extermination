using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	[Serializable]
	public struct DynamicRigData
	{
		public Animator    rigAnimator;
		
		[LabelText("Тазовая кость")]
		public Transform   pelvisBone;
		
		[LabelText("Главная кость для IK")]
		public DynamicBone masterDynamicBone;
		
		[LabelText("Правая рука")]
		public DynamicBone rightHandBone;
		
		[LabelText("Левая рука")]
		public DynamicBone leftHandBone;
		
		[LabelText("Правая нога")]
		public DynamicBone rightFootBone;
		
		[LabelText("Левая нога")]
		public DynamicBone leftFootBone;

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