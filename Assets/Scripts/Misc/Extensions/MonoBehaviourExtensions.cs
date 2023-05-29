using System.Runtime.CompilerServices;
using UnityEngine;

namespace Misc.Extensions
{
	public static class MonoBehaviourExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MonoBehaviour AddComponent<TMonoBeh>(this MonoBehaviour monoBehaviour)
			where TMonoBeh : MonoBehaviour
		{
			monoBehaviour.gameObject.AddComponent<TMonoBeh>();
			return monoBehaviour;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MonoBehaviour SetTag(this MonoBehaviour monoBehaviour, string tag)
		{
			monoBehaviour.tag = tag;
			return monoBehaviour;
		}
        
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MonoBehaviour SetLayer(this MonoBehaviour monoBehaviour, LayerMask layer)
		{
			monoBehaviour.gameObject.layer = layer;
			return monoBehaviour;
		}
        
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MonoBehaviour SetParent(this MonoBehaviour monoBehaviour, Transform parent)
		{
			monoBehaviour.transform.parent = parent;
			return monoBehaviour;
		}
	}
}