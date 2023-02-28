using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameSystems.Base
{
    public abstract class GameSystem : IObserver, IGameSystem, IDisposable
    {
        public event Action<Type> SystemStopped;
        public bool IsEnabled => _isEnabled;
        protected GameSystemsContainer SystemsСontainer;

        private bool _isEnabled;

        public void DefineContainer(GameSystemsContainer container) => SystemsСontainer = container;
        public virtual void OnNotify(string message, System.Object data) { }
        public virtual object OnRequest(string message, object requestObject) => null;
        public virtual Task<object> OnAsyncRequest(string message, object requestObject) => null;

        public virtual void Start()
        {
            Debug.Log($"{this.GetType().Name} заработал");
            SystemsСontainer.SystemsNotify += OnNotify;
        }

        public virtual void Update() { }
        public virtual void PhysicsUpdate() { }

        public virtual void Stop()
        {
            if(_isEnabled == false) return;
            
            SystemsСontainer.SystemsNotify -= OnNotify;
            
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