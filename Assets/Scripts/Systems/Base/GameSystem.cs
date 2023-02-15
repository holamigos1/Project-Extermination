using System;
using System.Collections.Generic;
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

        public List<GameSystem> GameSystems { get; }
        
        public virtual void OnNotify(string message, System.Object data)
        {
            
        }

        void IObserver.NotifyOtherObservers(string message, System.Object data)
        {
            
        }

        public void Start()
        {
            Debug.Log($"{typeof(GameSystem)} заработал" );
        }

        public virtual void Update()
        {
            
        }

        public void Stop()
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