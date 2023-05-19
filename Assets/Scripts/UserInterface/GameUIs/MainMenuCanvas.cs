using System;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.GameUIs
{
	public class MainMenuCanvas : GameCanvasBase
	{
		public event Action Play = delegate {  };
		public event Action ToSettings = delegate {  };
		public event Action Exit = delegate {  };

		
		[SerializeField] 
		private Button _playButton;
		
		[SerializeField] 
		private Button _settingsButton;		
		
		[SerializeField] 
		private Button _exitButton;
		
		private void OnEnable()
		{
			_playButton.onClick.AddListener(OnPlay);
			_settingsButton.onClick.AddListener(OnSettings);
			_exitButton.onClick.AddListener(OnExit);
		}

		private void OnDisable()
		{
			_playButton.onClick.RemoveListener(OnPlay);
			_settingsButton.onClick.RemoveListener(OnSettings);
			_exitButton.onClick.RemoveListener(OnExit);
		}

		private void OnPlay() => Play.Invoke();
		private void OnSettings() => ToSettings.Invoke();
		private void OnExit() => Exit.Invoke();
	}
}