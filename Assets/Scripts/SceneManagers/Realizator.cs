using System;
using System.Collections.Generic;
using GameItems.Base;
using Misc.Extensions;
using Misc.InterfaceReference;
using Misc.InterfaceReference.Attributes;
using UnityEngine;
using Weapons;
using Weapons.Melle;
using Range = System.Range;

namespace SceneManagers
{
	public class Realizator : MonoBehaviour
	{
		[SerializeField] 
		[TypeFilter(typeof(Weapon))] 
		[Expose]
		public InterfaceReference<IPickup> _interfaceReference;

		private List<string> list;

		private AnimationCurve curve;

		private void Start()
		{
			Debug.Log(curve.IsClear());
		}
	}
}