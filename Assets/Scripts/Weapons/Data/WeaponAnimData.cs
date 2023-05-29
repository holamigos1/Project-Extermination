using System;
using UnityEngine;

namespace Weapons.Data
{
	[Serializable]
	public struct WeaponAnimData
	{
		[Header("LeftHandIK")]
		public Transform leftHandTarget;
        
		[Header("AdsLayer")]
		public GunAimData gunAimData;
		public Vector3 handsOffset;
		
		[Header("SwayLayer")]
		public LocationRotationSpringData springData;
		public FreeAimData  freeAimData;
		public MoveSwayData moveSwayData;
		
		[Header("WeaponCollision")] 
		public GunBlockData blockData;
	}
}