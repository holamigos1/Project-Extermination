using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Systems.Base
{
    public class GameSystemsContainer
    {
        //TODO Прописать красивые метотды отображения систем в GameSystemsContainer с возможностью добавления и изменения их в сис
        public event Action<string, System.Object> SystemsNotify = delegate(string s, object o) {  };
        public IEnumerable<GameSystem> gameSystems => _gameSystems;
        
        public GameSystemsContainer()
        {
            _gameSystems = new List<GameSystem>();
        }
        
        
        private readonly List<GameSystem> _gameSystems;

        public void AddSystem(GameSystem gameSystemInst)
        {
            if(_gameSystems.Contains(gameSystemInst)) return; //а пусть будет только одна система без дублей
            
            _gameSystems.Add(gameSystemInst);
            gameSystemInst.SystemStopped += StopSystem;
            gameSystemInst.Start();
        }
        
        public void NotifySystems(string message, System.Object data)
        {
            SystemsNotify.Invoke(message, data);
        }

        [CanBeNull]
        public List<object> MakeRequest(string message, object requestObject)
        {
            List<object> responseList = new List<object>();
            
            _gameSystems.ForEach(system =>
            {
                var response = system.OnRequest(message, requestObject);
                if(response != null) responseList.Add(response);
            });
            
            if (responseList.Count > 0) return responseList;
            else return null;
        }
        
        [CanBeNull]
        public List<object> MakeRequest(string message)
        {
            List<object> responseList = new List<object>();
            
            _gameSystems.ForEach(system =>
            {
                var response = system.OnRequest(message, null);
                if(response != null) responseList.Add(response);
            });
            
            if (responseList.Count > 0) return responseList;
            else return null;
        }

        public void UpdateSystems()
        {
            foreach (GameSystem system in _gameSystems)
            {
                system.Update();
            }
        }
        
        public void UpdatePhysicsSystems()
        {
            foreach (GameSystem system in _gameSystems)
            {
                system.PhysicsUpdate();
            }
        }

        public GameSystem FindSystem<T>(T systemInst) where T: GameSystem
        {
            if (_gameSystems.Contains(systemInst))
            {
                return _gameSystems.Find(system => system.GetType() == typeof(T));
            }

            return null;
        }

        public GameSystem FindSystem(Type systemType)
        {
            return _gameSystems.Find(system => system.GetType() == systemType);
        }
        
        public void StopSystem<T>(T systemType) where T : GameSystem
        {
            GameSystem genericSystem = _gameSystems.Find(systemInst => systemInst.GetType() == typeof(T));
            genericSystem.SystemStopped -= StopSystem;
            genericSystem.Stop();
        }

        public void StopSystem(Type systemType)
        {
            GameSystem genericSystem = _gameSystems.Find(systemInst => systemInst.GetType() == systemType);
            genericSystem.SystemStopped -= StopSystem;
        }

        public void ShutDownSystems()
        {
            foreach (GameSystem system in _gameSystems)
            {
                system.Stop();
            }
        }
    }
}