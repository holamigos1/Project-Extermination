using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Misc.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace UserInterface
{
	/// <summary> MonoBehaviour прикрепляемый только к игровым объектам типа Canvas. <br/>
	/// Для создания своих собственных Canvas достаточно наследовать GameCanvasBase и реализовать необходимые члены.</summary>
	[RequireComponent(typeof(Canvas))]
	public abstract class GameCanvasBase : MonoBehaviour, IComparable<GameCanvasBase>
	{
		public static EventSystem EventSystemInstance =>
			s_eventSystemObject ?
			s_eventSystemObject : 
				EventSystem.current ? 
				s_eventSystemObject = EventSystem.current :
				s_eventSystemObject = new GameObject(nameof(EventSystem))
													.AddComponent<EventSystem>()
													.AddComponent<BaseInput>()
													.AddComponent<InputSystemUIInputModule>()
													.SetParent(UIGod.UIParentInstance)
													.SetTag("GameController")
													.SetLayer(LayerMask.NameToLayer("UI"))
													.GetComponent<EventSystem>();

		private static EventSystem s_eventSystemObject;
		
		[SerializeField, HideInInspector] 
		protected Canvas BaseCanvas;

		[SerializeField, HideInInspector] 
		public GameObject CanvasGameObject;

		[SerializeField, HideInInspector] 
		private CanvasScaler CanvasScalerComponent;
		private Vector2 _referenceCanvasResolution;

		[SerializeField]
		private List<Image> _allCanvasImages = new List<Image>();
		
		[SerializeField]
		private List<TMP_Text> _allTextComponents = new List<TMP_Text>();

		private void Awake()
		{
			if (EventSystemInstance == null)
				throw new NullReferenceException($"Объект типа {nameof(EventSystem)} не обнаружен на сцене!");
			_referenceCanvasResolution = CanvasScalerComponent.referenceResolution;
		}

		private void Reset()
		{
			BaseCanvas = GetComponent<Canvas>();
			CanvasScalerComponent = GetComponent<CanvasScaler>();
			CanvasGameObject = gameObject;

			foreach (Image imageComponent in GetComponentsInChildren<Image>())
				_allCanvasImages.Add(imageComponent);
			
			foreach (TMP_Text textComponent in GetComponentsInChildren<TMP_Text>())
				_allTextComponents.Add(textComponent);
		}

		public void ApplyUITransparency(float newAlphaValue)
		{
			Color imageColor;
			newAlphaValue = Mathf.Clamp01(newAlphaValue);
			
			foreach (Image imageComponent in _allCanvasImages)
			{
				imageColor = imageComponent.color;
				imageColor.a = newAlphaValue;
				imageComponent.color = imageColor;
			}

			foreach (TMP_Text textComponent in _allTextComponents)
				textComponent.alpha = newAlphaValue;
		}

		public void ApplyUIScale(float newScale)
		{
			const float minimumResolutionClamp = 0.6f;
			const float maximumResolutionClamp = 1.4f;
			newScale = Mathf.Clamp(newScale,minimumResolutionClamp, maximumResolutionClamp);
			
			CanvasScalerComponent.referenceResolution = _referenceCanvasResolution / newScale;
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
