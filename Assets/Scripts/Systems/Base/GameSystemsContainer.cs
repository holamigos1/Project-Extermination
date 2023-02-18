using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Systems.Base
{
    public class GameSystemsContainer
    {
        //TODO Прописать красивые методы отображения систем в GameSystemsContainer с возможностью добавления и изменения любых систем в проекте через Inspector
        public event Action<string, System.Object> SystemsNotify = delegate(string s, object o) {  };
        public IEnumerable<GameSystem> gameSystems => _gameSystems;
        
        public GameSystemsContainer()
        {
            _gameSystems = new List<GameSystem>();
        }
        
        
        private readonly List<GameSystem> _gameSystems;

        public void AddSystem(GameSystem gameSystemInst)
        {
            if(_gameSystems.Contains(gameSystemInst)) return;
            
            _gameSystems.Add(gameSystemInst);
            
            StartSystem(gameSystemInst);
        }

        public void StartSystem(GameSystem gameSystemInst) => gameSystemInst.Start();
        
        
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
        public async Task<List<object>> MakeAsyncRequest(string message, object requestObject)
        {
            List<Task<object>> tasks = new List<Task<object>>();
            List<object> responseList = new List<object>();
            
            _gameSystems.ForEach(system => 
                tasks.Add(system.OnAsyncRequest(message, requestObject)));
            
            await Task.WhenAll(tasks);

            if (tasks.Count <= 0) return null;
            
            foreach (var task in tasks)
            {
                if(task.Result != null) responseList.Add(task.Result);
            }
            
            return responseList;
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

        public void ShutDownSystems()
        {
            foreach (GameSystem system in _gameSystems)
            {
                system.Stop();
            }
        }

        public void RemoveSystem<T>(T systemInst) where T: GameSystem
        {
            GameSystem genericSystem = _gameSystems.Find(systemInst => systemInst.GetType() == typeof(T)); ;
            _gameSystems.Remove(genericSystem);
        }
    }
}