using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.GameUIs
{
	public class SettingsCanvas : GameCanvasBase
	{
		[SerializeField] 
		private Button _exitButton;
		
		[SerializeField] 
		private Slider _soundsSlider;
		
		[SerializeField] 
		private Slider _musicSlider;
	}
}