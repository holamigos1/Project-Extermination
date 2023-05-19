using System;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.GameUIs
{
	public class PauseCanvas : GameCanvasBase
	{
		public event Action Resume = delegate {  };
		public event Action ToSettings = delegate {  };
		public event Action Exit = delegate {  };

		[SerializeField]
		private Button _resumeButton;
		
		[SerializeField]
		private Button _settingsButton;
		
		[SerializeField]
		private Button _exitButton;

		private void OnEnable()
		{
			_resumeButton.onClick.AddListener(OnResume);
			_settingsButton.onClick.AddListener(OnSettings);
			_exitButton.onClick.AddListener(OnExit);
		}

		private void OnDisable()
		{
			_resumeButton.onClick.RemoveListener(OnResume);
			_settingsButton.onClick.RemoveListener(OnSettings);
			_exitButton.onClick.RemoveListener(OnExit);
		}

		private void OnResume() => Resume.Invoke();
		private void OnSettings() => ToSettings.Invoke();
		private void OnExit() => Exit.Invoke();
	}
}