using UnityEngine;

namespace UserInterface
{
	/// <summary>
	/// Ебаный тупой безпослезный моеобех по верх которого нада свои типовые полотна хучяить
	/// </summary>
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
