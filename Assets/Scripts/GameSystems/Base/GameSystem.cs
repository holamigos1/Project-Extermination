using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameSystems.Base
{
    [Serializable]
    public abstract class GameSystem : IObserver, IGameSystem, IDisposable
    {
        public GameSystem() { }
        
        
        [ToggleLeft] [ShowInInspector] [PropertyOrder(-1)] [LabelText("Включена")]
        [GUIColor("@GameExtensions.Extensions.GetEnableToggleColor(_isEnabled)")] 
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled) return;
                if (value == true && Application.isPlaying) Start();
                if (value == false && Application.isPlaying) Stop();
                _isEnabled = value;
            }
        }
        
        protected GameSystemsContainer SystemsСontainer;
        
        [SerializeField]
        [HideInInspector] 
        private bool _isEnabled = true;

        public void DefineContainer(GameSystemsContainer container) => SystemsСontainer = container;
        public virtual void OnNotify(string message, System.Object data) { }
        public virtual object OnRequest(string message, object requestObject) => null;
        public virtual Task<object> OnAsyncRequest(string message, object requestObject) => null;
        public virtual void Reset() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void PhysicsUpdate() { }
        public virtual void Stop() { }

        public void Dispose()
        {
            Stop();
            SystemsСontainer.RemoveSystem(this);
        }

        public override string ToString() =>
            this.GetType().Name +" к вашим услугам";
    }
}