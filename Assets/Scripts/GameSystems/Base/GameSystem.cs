using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GameSystems.Base
{
    [Serializable]
    public abstract class GameSystem : IObserver, IGameSystem, IDisposable
    {
        public event Action<Type> SystemStopped;

        [ToggleLeft]
        [ShowInInspector]
        [PropertyOrder(1)]
        [GUIColor("@GameExtensions.Extensions.GetEnableToggleColor(_isEnabled)")] 
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == true)
                {
                    _isEnabled = true;
                    if (Application.isPlaying) Start();
                }
                else
                {
                    if (Application.isPlaying) Stop();
                    _isEnabled = false;
                }
            }
        }
        
        [SerializeField][HideInInspector] 
        protected GameSystemsContainer SystemsСontainer;
        
        [SerializeField][HideInInspector] 
        private bool _isEnabled = true;

        public void DefineContainer(GameSystemsContainer container) =>
            SystemsСontainer = container;
        
        public virtual void OnNotify(string message, System.Object data) { }
        public virtual object OnRequest(string message, object requestObject) => null;
        public virtual Task<object> OnAsyncRequest(string message, object requestObject) => null;

        public virtual void Start()
        {
            if(_isEnabled == false) return;
            
            Debug.Log($"{this.GetType().Name} запущен и готов к бою");
            SystemsСontainer.Notify += OnNotify;
        }

        public virtual void Update() { }
        public virtual void PhysicsUpdate() { }

        public virtual void Stop()
        {
            if(_isEnabled == false) return;
            
            SystemsСontainer.Notify -= OnNotify;
            
            _isEnabled = false;
            
            Debug.Log($"{this.GetType().Name} вырубился");
            SystemStopped?.Invoke(this.GetType());
        }

        public void Dispose()
        {
            Stop();
            SystemsСontainer.RemoveSystem(this);
        }
    }
}