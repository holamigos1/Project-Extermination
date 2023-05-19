using System;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.GameUIs
{
	public class SettingsCanvas : GameCanvasBase
	{
		public event Action Returing = delegate {  };
		public event Action<float> SoundValueChanging = delegate {  };
		public event Action<float> MusicValueChanging = delegate {  };

		public float SoundValue
		{
			get => _soundsSlider.value;
			set => _soundsSlider.value = value;
		}
		
		public float MusicValue
		{
			get => _musicSlider.value;
			set => _musicSlider.value = value;
		}

		[SerializeField] 
		private Button _exitButton;
		
		[SerializeField] 
		private Slider _soundsSlider;
		
		[SerializeField] 
		private Slider _musicSlider;
		
		private void OnEnable()
		{
			_soundsSlider.onValueChanged.AddListener(OnSoundValueChanging);
			_musicSlider.onValueChanged.AddListener(OnMusicValueChanging);
			_exitButton.onClick.AddListener(OnResume);
		}

		private void OnDisable()
		{
			_soundsSlider.onValueChanged.RemoveListener(OnSoundValueChanging);
			_musicSlider.onValueChanged.RemoveListener(OnMusicValueChanging);
			_exitButton.onClick.RemoveListener(OnResume);
		}

		private void OnResume() => Returing.Invoke();
		private void OnSoundValueChanging(float value) => SoundValueChanging.Invoke(value);
		private void OnMusicValueChanging(float value) => MusicValueChanging.Invoke(value);
	}
}