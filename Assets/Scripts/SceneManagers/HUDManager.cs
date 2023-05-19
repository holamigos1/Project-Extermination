using Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UserInterface;
using UserInterface.GameUIs;

namespace SceneManagers
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] 
        private SceneAsset _mainMenuSceneAsset;

        [SerializeField]
        private UnityEvent<bool> _onGameRunning;
    
        private HUDCanvas      _hudCanvas;
        private SettingsCanvas _settingsCanvas;
        private PauseCanvas    _pauseCanvas;

        public SettingsCanvas SettingsCanvas
        {
            get => _settingsCanvas;
            set
            {
                if (value)
                {
                    _settingsCanvas = value;
                    _settingsCanvas.Returing += ReturnFromSettings;
                    _settingsCanvas.MusicValueChanging += OnMusicValueChanging;
                    _settingsCanvas.SoundValueChanging += OnSoundValueChanging;
                    _settingsCanvas.SoundValue = PlayerPrefsVars.GlobalSoundsValue;
                    _settingsCanvas.MusicValue = PlayerPrefsVars.GlobalMusicValue;
                }
                else
                {
                    _settingsCanvas.Returing -= ReturnFromSettings;
                    _settingsCanvas.MusicValueChanging -= OnMusicValueChanging;
                    _settingsCanvas.SoundValueChanging -= OnSoundValueChanging;
                    Destroy(_settingsCanvas.gameObject);
                    _settingsCanvas = null;
                }
            }
        }
        
        public PauseCanvas PauseCanvas
        {
            get => _pauseCanvas;
            private set
            {
                if (value)
                {
                    _pauseCanvas = value;
                    _pauseCanvas.Exit += GoToMainMenu;
                    _pauseCanvas.Resume += ResumePause;
                    _pauseCanvas.ToSettings += GoToSettings;
                }
                else
                {
                    _pauseCanvas.Exit -= GoToMainMenu;
                    _pauseCanvas.Resume -= ResumePause;
                    _pauseCanvas.ToSettings -= GoToSettings;
                    Destroy(_pauseCanvas.gameObject);
                    _pauseCanvas = null;
                }
            }
        }

        // Start is called before the first frame update
        private void OnEnable()
        {
            _hudCanvas = UIGod.GetCanvasInstance<HUDCanvas>();
        }

        private void OnDisable()
        {
            ResumePause();
            Destroy(_hudCanvas);
            _hudCanvas = null;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) //TODO Сделать через NewInputSystem
                if (_pauseCanvas == null)
                    Pause();
                else
                    ResumePause();
            
        }

        private void Pause()
        {
            Time.timeScale = 0;
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            PauseCanvas = UIGod.GetCanvasInstance<PauseCanvas>();

            _onGameRunning.Invoke(false);
        }

        private void GoToSettings()
        {
            SettingsCanvas = UIGod.GetCanvasInstance<SettingsCanvas>();
        }
    
        private void GoToMainMenu()
        {
            SceneManager.LoadScene(_mainMenuSceneAsset.name);
        }

        private void ResumePause()
        {
            Time.timeScale = 1;
            
            if(_pauseCanvas == null)
                return;
        
            if (_settingsCanvas != null)
                ReturnFromSettings();
        
            _onGameRunning.Invoke(true);
        
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            PauseCanvas = null;
        }
    
        private void ReturnFromSettings()
        {
            SettingsCanvas = null;
        }
    
        private void OnMusicValueChanging(float newValue) =>
            PlayerPrefsVars.GlobalMusicValue = newValue;

        private void OnSoundValueChanging(float newValue) =>
            PlayerPrefsVars.GlobalSoundsValue = newValue;
    }
}
