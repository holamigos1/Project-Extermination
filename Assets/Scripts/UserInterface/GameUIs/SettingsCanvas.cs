using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UserInterface.GameUIs
{
	public class SettingsCanvas : GameCanvasBase
	{
		public event Action Returing = delegate {  };
		public event Action<float> UIScaleValueChanging = delegate {  };
		public event Action<float> UITransparencyValueChanging = delegate {  };
		public event Action<float> FoVValueChanging = delegate {  };
		public event Action<float> SoundValueChanging = delegate {  };
		public event Action<float> MusicValueChanging = delegate {  };

		public float UIScaleValue
		{
			get => _UIScaleSlider.value;
			set => _UIScaleSlider.value = value;
		}
		public float UITransparencyValue
		{
			get => _transparencySlider.value;
			set => _transparencySlider.value = value;
		}
		public float FoVValue
		{
			get => _fovSlider.value;
			set => _fovSlider.value = value;
		}
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
		private Slider _UIScaleSlider;
		
		[SerializeField] 
		private Slider _transparencySlider;
		
		[SerializeField] 
		private Slider _fovSlider;

		[SerializeField] 
		private Slider _soundsSlider;
		
		[SerializeField] 
		private Slider _musicSlider;
		
		[SerializeField] 
		private Button _exitButton;
		
		private void OnEnable()
		{
			_UIScaleSlider.onValueChanged.AddListener(value => UIScaleValueChanging.Invoke(value));
			_transparencySlider.onValueChanged.AddListener(value => UITransparencyValueChanging.Invoke(value));
			_fovSlider.onValueChanged.AddListener(value => FoVValueChanging.Invoke(value));
			_soundsSlider.onValueChanged.AddListener(OnSoundValueChanging);
			_musicSlider.onValueChanged.AddListener(OnMusicValueChanging);
			_exitButton.onClick.AddListener(OnResume);
		}

		private void SD(float arg0)
		{
			throw new NotImplementedException();
		}

		private void OnDisable()
		{
			_UIScaleSlider.onValueChanged.RemoveListener(value => UIScaleValueChanging.Invoke(value));
			_transparencySlider.onValueChanged.RemoveListener(value => UITransparencyValueChanging.Invoke(value));
			_fovSlider.onValueChanged.RemoveListener(value => FoVValueChanging.Invoke(value));
			_soundsSlider.onValueChanged.RemoveListener(OnSoundValueChanging);
			_musicSlider.onValueChanged.RemoveListener(OnMusicValueChanging);
			_exitButton.onClick.RemoveListener(OnResume);
		}

		private void OnResume() => Returing.Invoke();
		private void OnSoundValueChanging(float value) => SoundValueChanging.Invoke(value);
		private void OnMusicValueChanging(float value) => MusicValueChanging.Invoke(value);
	}
}