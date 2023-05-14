using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.GameUIs
{
	public class PauseCanvas : GameCanvasBase
	{
		[SerializeField] 
		private Button _resumeButton;
		
		[SerializeField] 
		private Button _settingsButton;
		
		[SerializeField] 
		private Button _exitButton;
	}
}