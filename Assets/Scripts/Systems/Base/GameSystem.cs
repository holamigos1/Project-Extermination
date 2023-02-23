using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Systems.Base
{
    public abstract class GameSystem : IObserver, IGameSystem, IDisposable
    {
        public event Action<Type> SystemStopped = delegate(Type type) {  };
        public GameSystemsContainer SystemsСontainer => _container;
        public bool IsEnabled => _isEnabled;

        private GameSystemsContainer _container;
        private bool _isEnabled;
        
        public virtual void OnNotify(string message, System.Object data)
        {
            if(_isEnabled == false) return;
        }

        public virtual object OnRequest(string message, object requestObject)
        {
            if(_isEnabled == false) return null;
            return null;
        }
        
        public virtual async Task<object> OnAsyncRequest(string message, object requestObject)
        {
            if(_isEnabled == false) return null;
            
            return null;
        }

        public virtual void PhysicsUpdate()
        {
            if(_isEnabled == false) return;
        }
        
        
        public virtual void Start()
        {
            Debug.Log($"{this.GetType().Name} заработал");
            _container.SystemsNotify += OnNotify;
        }

        public virtual void Update()
        {
            if(_isEnabled == false) return;
        }

        public virtual void Stop()
        {
            if(_isEnabled == false) return;
            
            _container.SystemsNotify -= OnNotify;
            
            _isEnabled = false;
            SystemStopped.Invoke(this.GetType());
        }

        public void DefineContainer(GameSystemsContainer container)
        {
            _container = container;
        }

        public void Dispose()
        {
            Stop();
            _container.RemoveSystem(this);
        }
    }
}