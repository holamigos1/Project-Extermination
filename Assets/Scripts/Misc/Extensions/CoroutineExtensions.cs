using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace Misc.Extensions
{
    /// <summary> Сверхобёртка CustomYieldInstruction унаследованного напрямую от IEnumerator. <br/><br/>
    /// Для создания любых YieldInstruction достаточно наследовать Instruction и реализовать необходимые члены. </summary>
    /// <seealso cref="CustomYieldInstruction"/> 
    /// <seealso cref="IEnumerator"/>
    public abstract class ExtendedYieldInstruction : IEnumerator, IInstruction
    {
        protected ExtendedYieldInstruction()
        {
            AsIEnumerator = this;
        }
        
        protected ExtendedYieldInstruction(MonoBehaviour monoParent)
        {
            MonoParent = monoParent;
            AsIEnumerator = this;
        }

        private MonoBehaviour MonoParent { get; set; }
        public bool IsExecuting { get; private set; }
        public bool IsPaused { get; private set; }
        private IEnumerator AsIEnumerator { get; set; }

        object IEnumerator.Current => _current;
        
        public event Action<ExtendedYieldInstruction> Started;
        public event Action<ExtendedYieldInstruction> Paused;
        public event Action<ExtendedYieldInstruction> Cancelled;
        public event Action<ExtendedYieldInstruction> Done;
        
        private bool IsStopped { get; set; }
        
        private ExtendedYieldInstruction _current;
        private object _routine;

        private static MonoBehaviour CoroutineParent => s_coroutineParent ?
                                                        s_coroutineParent :
                                                        s_coroutineParent = new GameObject("Coroutine Container") 
                                                                                .SetTag("GameController")
                                                                                .AddComponent<MonoBehaviourContainer>();

        private static MonoBehaviour s_coroutineParent;

        void IEnumerator.Reset()
        {
            IsPaused = false;
            IsStopped = false;
            _routine = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IEnumerator.MoveNext()     //TODO Проверить производительность по сравнению с обычной корутиной в похожих задачах
        {
            if (IsStopped)
            {
                AsIEnumerator.Reset();
                return false;
            }

            if (IsExecuting == false)
            {
                IsExecuting = true;
                _routine = new object();

                OnStarted();
                Started?.Invoke(this);
            }

            if (_current != null)
                return true;

            if (IsPaused)
                return true;

            if (Update() == false)
            {
                OnDone();
                Done?.Invoke(this);

                IsStopped = true;
                return false;
            }

            return true;
        }

        public void Pause()
        {
            if (IsExecuting == false || IsPaused)
                return;
            
            IsPaused = true;

            OnPaused();
            Paused?.Invoke(this);
        }

        public void Resume()
        {
            IsPaused = false;
            OnResumed();
        }

        public void Cancel()
        {
            if (Stop() == false)
                return;
            
            OnCancelled();
            Cancelled?.Invoke(this);
        }

        private bool Stop()
        {
            if (IsExecuting == false)
                return false;
            
            if (MonoParent == false)
                MonoParent = CoroutineParent;
            
            if (_routine is Coroutine coroutine)
                MonoParent.StopCoroutine(coroutine);

            AsIEnumerator.Reset();

            return IsStopped = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ExtendedYieldInstruction Execute()
        {
            if (_current != null)
            {
                Debug.LogWarning($"Instruction { GetType().Name} is currently waiting for another one and can't be stared right now.");
                return this;
            }

            if (MonoParent == false)
                MonoParent = CoroutineParent;
            
            if (IsExecuting == false)
            {
                IsExecuting = true;
                _routine = MonoParent.StartCoroutine(this);
                return this;
            }

            Debug.LogWarning($"Instruction { GetType().Name} is already executing.");
            return this;
        }

        public ExtendedYieldInstruction Execute(MonoBehaviour parent)
        {
            MonoParent = parent;
            return Execute();
        }

        public void Reset()
        {
            Cancel();

            Started = null;
            Paused = null;
            Cancelled = null;
            Done = null;
        }

        protected virtual void OnStarted() { }
        protected virtual void OnPaused() { }
        protected virtual void OnResumed() { }
        protected virtual void OnCancelled() { }
        protected virtual void OnDone() { }
        protected abstract bool Update();
    }
    
    public interface IInstruction
    {
        bool IsExecuting { get; }
        bool IsPaused { get; }

        ExtendedYieldInstruction Execute();
        void Pause();
        void Resume();
        void Cancel();

        event Action<ExtendedYieldInstruction> Started;
        event Action<ExtendedYieldInstruction> Paused;
        event Action<ExtendedYieldInstruction> Cancelled;
        event Action<ExtendedYieldInstruction> Done;
    }
}