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
        
        public SettingsCanvas SettingsCanvas
        {
            get => _settingsCanvas;
            set
            {
                if (value)
                {
                    _settingsCanvas = value;
                    _settingsCanvas.Returing += OnSettingsReturn;
                    _settingsCanvas.MusicValueChanging += OnMusicValueChanging;
                    _settingsCanvas.SoundValueChanging += OnSoundValueChanging;
                    _settingsCanvas.SoundValue = PlayerPrefsVars.GlobalSoundsValue;
                    _settingsCanvas.MusicValue = PlayerPrefsVars.GlobalMusicValue;
                }
                else
                {
                    _settingsCanvas.Returing -= OnSettingsReturn;
                    _settingsCanvas.MusicValueChanging -= OnMusicValueChanging;
                    _settingsCanvas.SoundValueChanging -= OnSoundValueChanging;
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
