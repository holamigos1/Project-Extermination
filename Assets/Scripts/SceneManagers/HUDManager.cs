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
        
        [SerializeField]
        private UnityEvent<float> _onFOV_Changed;
    
        private HUDCanvas      _hudCanvas;
        private SettingsCanvas _settingsCanvas;
        private PauseCanvas    _pauseCanvas;

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
                    if(!_settingsCanvas) return;
                    
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
                    _pauseCanvas.ApplyUITransparency(PlayerPrefsVars.UITransparencyValue);
                    _pauseCanvas.ApplyUIScale(PlayerPrefsVars.UIScaleValue);
                }
                else
                {
                    if(!_pauseCanvas) return;
                    
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
            _hudCanvas.ApplyUITransparency(PlayerPrefsVars.UITransparencyValue);
            _hudCanvas.ApplyUIScale(PlayerPrefsVars.UIScaleValue);
            OnFoVValueChanging(PlayerPrefsVars.FPSFoVValue);
            ResumePause();
        }

        private void OnDisable()
        {
            ResumePause();
            Destroy(_hudCanvas);
            _hudCanvas = null;
            SettingsCanvas = null;
            PauseCanvas = null;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) //TODO Сделать через NewInputSystem
                if (!_pauseCanvas) Pause();
                else ResumePause();
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
            Debug.Log(UIGod.GetCanvasInstance<SettingsCanvas>());
            SettingsCanvas = UIGod.GetCanvasInstance<SettingsCanvas>();
        }
    
        private void GoToMainMenu()
        {
            SceneManager.LoadScene(_mainMenuSceneAsset.name);
        }

        private void ResumePause()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            if(_pauseCanvas == null)
                return;
            
            Time.timeScale = 1;
        
            if (_settingsCanvas != null)
                OnSettingsReturn();
        
            _onGameRunning.Invoke(true);
            
            PauseCanvas = null;
        }
    
        private void OnSettingsReturn()
        {
            SettingsCanvas = null;
        }
    
        private void OnFoVValueChanging(float newValue)
        {
            PlayerPrefsVars.FPSFoVValue = newValue;
            _onFOV_Changed.Invoke(newValue);
        }

        private void OnUITransparencyValueChanging(float newValue)
        {
            PlayerPrefsVars.UITransparencyValue = newValue;
            _pauseCanvas.ApplyUITransparency(newValue);
            _hudCanvas.ApplyUITransparency(newValue);
            _settingsCanvas.ApplyUITransparency(newValue);
        }
        
        private void OnUIScaleValueChanging(float newValue)
        {
            PlayerPrefsVars.UIScaleValue = newValue;
            _pauseCanvas.ApplyUIScale(newValue);
            _hudCanvas.ApplyUIScale(newValue);
            _settingsCanvas.ApplyUIScale(newValue);
        }
        
        private void OnMusicValueChanging(float newValue) =>
            PlayerPrefsVars.GlobalMusicValue = newValue;
        
        private void OnSoundValueChanging(float newValue) =>
            PlayerPrefsVars.GlobalSoundsValue = newValue;
    }
}
