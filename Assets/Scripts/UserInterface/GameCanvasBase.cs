using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UserInterface
{
	/// <summary>
	/// Ебаный тупой безпослезный моеобех по верх которого нада свои типовые полотна хучяить
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	public abstract class GameCanvasBase : MonoBehaviour, IComparable<GameCanvasBase>
	{
		[SerializeField] 
		protected Canvas BaseCanvas;

		[SerializeField] 
		private EventSystem _eventSystem;

		private void Awake()
		{
			if (EventSystem.current != _eventSystem)
				_eventSystem.enabled = false;
		}

		private void Reset()
		{
			_eventSystem = GetComponentInChildren<EventSystem>();
			BaseCanvas = GetComponent<Canvas>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CompareTo(GameCanvasBase other)
		{
			if (other != null)
				return GetInstanceID().CompareTo(other.GetInstanceID());
			
			Debug.LogWarning($"А чё в списке пустой {nameof(GameCanvasBase)} висит?");
			return 1;
		}
	}
}
