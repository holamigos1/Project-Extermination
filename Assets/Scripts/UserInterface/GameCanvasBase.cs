using UnityEngine;

namespace UserInterface
{
	[RequireComponent(typeof(Canvas))]
	public abstract class GameCanvasBase : MonoBehaviour
	{
		protected Canvas BaseCanvas;

		public virtual void Awake()
		{
			BaseCanvas = GetComponent<Canvas>();
		}
	}
}
