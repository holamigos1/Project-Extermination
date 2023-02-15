using System;
using System.Collections.Generic;

namespace Systems.Base
{
    public class GameSystemsContainer
    {
        //TODO Прописать красивые метотды отображения систем в GameSystemsContainer с возможностью добавления и изменения их в сис
        public event Action<string, System.Object> SystemsNotify;
        public IEnumerable<GameSystem> Systems => _systems;
        
        public GameSystemsContainer()
        {
            _systems = new List<GameSystem>();
        }
        
        private List<GameSystem> _systems;

        public void AddSystem(GameSystem gameSystemInst)
        {
            if(_systems.Contains(gameSystemInst)) return; //а пусть будет только одна система без дублей
            
            _systems.Add(gameSystemInst);
            gameSystemInst.SystemStopped += StopSystem;
            gameSystemInst.Start();
        }

        public void StopSystem<T>(T systemType) where T : GameSystem
        {
            GameSystem genericSystem = _systems.Find(systemInst => systemInst.GetType() == typeof(T));
            genericSystem.SystemStopped -= StopSystem;
            genericSystem.Stop();
        }

        public void StopSystem(Type systemType)
        {
            GameSystem genericSystem = _systems.Find(systemInst => systemInst.GetType() == systemType);
            genericSystem.SystemStopped -= StopSystem;
        }

        public void UpdateSystems()
        {
            foreach (GameSystem system in _systems)
            {
                system.Update();
            }
        }

        public GameSystem FindSystem<T>(T systemInst) where T: GameSystem
        {
            if (_systems.Contains(systemInst))
            {
                return _systems.Find(system => system.GetType() == typeof(T));
            }

            return null;
        }

        public GameSystem FindSystem(Type systemType)
        {
            return _systems.Find(system => system.GetType() == systemType);
        }

        public void ShutDownSystems()
        {
            foreach (GameSystem system in _systems)
            {
                system.Stop();
            }
        }
        
        public void NotifySystems(string message, System.Object data)
        {
            SystemsNotify.Invoke(message, data);
        }
    }
}