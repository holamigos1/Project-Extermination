using System.Collections.Generic;
using GameAnimation.Sheets;
using Misc;
using UnityEngine;

namespace DefaultNamespace
{
	public class asetdbtest : MonoBehaviour
	{
		[SerializeReference] public List<realezatsia> List;

		private void Reset()
		{
			Debug.Log(List.First());
			/*
			var d = AssetDataBaseExtensions.
				LoadAssetAtFilter<HumanAnimatorSheet>($"t:{nameof(HumanAnimatorSheet)}");//TODO Магическая t:
				*/
		}
	}

	public interface realezatsia
	{
		
	}
}