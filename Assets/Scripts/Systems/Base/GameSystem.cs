using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Systems.Base
{
    public abstract class GameSystem : IObserver, IGameSystem, IDisposable
    {
        public event Action<Type> SystemStopped;
        public GameSystemsContainer SystemsСontainer => _container;
        
        protected GameSystem(GameSystemsContainer container)
        {
            _container = container;
            _container.SystemsNotify += OnNotify;
        }
        
        private GameSystemsContainer _container;

        public virtual void OnNotify(string message, System.Object data)
        {
            
        }

        public virtual object OnRequest(string message, object requestObject)
        {
            return null;
        }
        
        public virtual async Task<object> OnAsyncRequest(string message, object requestObject)
        {
            return null;
        }
        
        void IObserver.NotifyOtherObservers(string message, System.Object data)
        {
            
        }

        public virtual void PhysicsUpdate()
        {
            
        }
        
        
        public virtual void Start()
        {
            Debug.Log($"{this.GetType().Name} заработал");
        }

        public virtual void Update()
        {
            
        }

        public virtual void Stop()
        {
            _container.SystemsNotify -= OnNotify;
            SystemStopped.Invoke(this.GetType());
        }

        public void Dispose()
        {
            Stop();
        }
    }
}