using Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UserInterface;
using UserInterface.GameUIs;

namespace SceneManagers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField]
        private SceneAsset _gameScene;
    
        private MainMenuCanvas _mainMenuCanvas;
        private SettingsCanvas _settingsCanvas;

        public MainMenuCanvas MainMenuCanvas
        {
            get => _mainMenuCanvas;
            set
            {
                if (value)
                {
                    _mainMenuCanvas = value;
                    _mainMenuCanvas.Play += OnPlayAction;
                    _mainMenuCanvas.Exit += OnExit;
                    _mainMenuCanvas.ToSettings += OnSettingsAction;
                    _mainMenuCanvas.ApplyUITransparency(PlayerPrefsVars.UITransparencyValue);
                    _mainMenuCanvas.ApplyUIScale(PlayerPrefsVars.UIScaleValue);
                }
                else
                {
                    _mainMenuCanvas.Play -= OnPlayAction;
                    _mainMenuCanvas.Exit -= OnExit;
                    _mainMenuCanvas.ToSettings -= OnSettingsAction;
                    Destroy(_mainMenuCanvas.gameObject);
                    _mainMenuCanvas = null;
                }
            }
        }
        
        public SettingsCanvas SettingsCanvas //TODO Этот же код дублируется в других файлах, убери логику в классы обработчики 
        {
            set
            {
                if (value)
                {
                    _settingsCanvas = value;
                    _settingsCanvas.Returing += OnSettingsReturn;
                    _settingsCanvas.MusicValueChanging += OnMusicValueChanging;
                    _settingsCanvas.SoundValueChanging += OnSoundValueChanging;
                    _settingsCanvas.FoVValueChanging += OnFoVValueChanging;
                    _settingsCanvas.UITransparencyValueChanging += OnUITransparencyValueChanging;
                    _settingsCanvas.UIScaleValueChanging += OnUIScaleValueChanging;
                    
                    _settingsCanvas.UIScaleValue = PlayerPrefsVars.UIScaleValue;
                    _settingsCanvas.FoVValue = PlayerPrefsVars.FPSFoVValue;
                    _settingsCanvas.UITransparencyValue = PlayerPrefsVars.UITransparencyValue;
                    _settingsCanvas.SoundValue = PlayerPrefsVars.GlobalSoundsValue;
                    _settingsCanvas.MusicValue = PlayerPrefsVars.GlobalMusicValue;
                    _settingsCanvas.ApplyUITransparency(PlayerPrefsVars.UITransparencyValue);
                    _settingsCanvas.ApplyUIScale(PlayerPrefsVars.UIScaleValue);
                }
                else
                {
                    _settingsCanvas.Returing -= OnSettingsReturn;
                    _settingsCanvas.MusicValueChanging -= OnMusicValueChanging;
                    _settingsCanvas.SoundValueChanging -= OnSoundValueChanging;
                    _settingsCanvas.FoVValueChanging -= OnFoVValueChanging;
                    _settingsCanvas.UITransparencyValueChanging -= OnUITransparencyValueChanging;
                    _settingsCanvas.UIScaleValueChanging -= OnUIScaleValueChanging;
                    
                    Destroy(_settingsCanvas.gameObject);
                    _settingsCanvas = null;
                }
            }
        }
    
        private void OnEnable()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            MainMenuCanvas = UIGod.GetCanvasInstance<MainMenuCanvas>();
        }
        
        private void OnDisable()
        {
            if (_mainMenuCanvas == null)
                return;
        
            Cursor.visible = false;

            MainMenuCanvas = null;
        }

        private void OnPlayAction()
        {
            SceneManager.LoadScene(_gameScene.name);
        }
        private void OnSettingsAction()
        {
            SettingsCanvas = UIGod.GetCanvasInstance<SettingsCanvas>();
        }

        private void OnSettingsReturn()
        {
            SettingsCanvas = null;
        }
        
        private void OnFoVValueChanging(float newValue)
        {
            PlayerPrefsVars.FPSFoVValue = newValue;
        }

        private void OnUITransparencyValueChanging(float newValue)
        {
            PlayerPrefsVars.UITransparencyValue = newValue;
            _mainMenuCanvas.ApplyUITransparency(newValue);
            _settingsCanvas.ApplyUITransparency(newValue);
        }
        
        private void OnUIScaleValueChanging(float newValue)
        {
            PlayerPrefsVars.UIScaleValue = newValue;
            _mainMenuCanvas.ApplyUIScale(newValue);
            _settingsCanvas.ApplyUIScale(newValue);
        }
        
        private void OnMusicValueChanging(float newValue) =>
            PlayerPrefsVars.GlobalMusicValue = newValue;
        
        private void OnSoundValueChanging(float newValue) =>
            PlayerPrefsVars.GlobalSoundsValue = newValue;

        private void OnExit()
        {
            Debug.Log("Application.Quit()");
            Application.Quit();
        }
    }
}
