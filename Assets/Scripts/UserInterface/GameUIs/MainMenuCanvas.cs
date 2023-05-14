using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.GameUIs
{
	public class MainMenuCanvas : GameCanvasBase
	{
		[SerializeField] 
		private Button _playButton;
		
		[SerializeField] 
		private Button _settingsButton;		
		
		[SerializeField] 
		private Button _exitButton;
	}
}