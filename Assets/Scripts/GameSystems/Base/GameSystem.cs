using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameSystems.Base
{
    [Serializable]
    public abstract class GameSystem : IObserver, IGameSystem, IDisposable
    {
        public event Action<Type> SystemStopped;
        public bool IsEnabled => _isEnabled;
        
        [SerializeField][HideInInspector] 
        protected GameSystemsContainer SystemsСontainer;
        
        [SerializeField][ToggleLeft][GUIColor("@GameExtensions.Extensions.GetEnableToggleColor(_isEnabled)")] 
        private bool _isEnabled = true;

        public void DefineContainer(GameSystemsContainer container) =>
            SystemsСontainer = container;
        
        public virtual void OnNotify(string message, System.Object data) { }
        public virtual object OnRequest(string message, object requestObject) => null;
        public virtual Task<object> OnAsyncRequest(string message, object requestObject) => null;

        public virtual void Start()
        {
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