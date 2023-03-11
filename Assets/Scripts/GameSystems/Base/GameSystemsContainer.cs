using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = System.Object;

namespace GameSystems.Base
{
    #nullable enable
    [Serializable]
    public class GameSystemsContainer
    {
        public event Action? Update;
        public event Action? PhysUpdate;
        public event Action<string, object>? Notify;
        public IEnumerable<GameSystem> GameSystems => _gameSystems;

        [SerializeReference] 
        private List<GameSystem> _gameSystems = new List<GameSystem>();

        public void AddSystem(GameSystem gameSystemInst)
        {
            gameSystemInst.DefineContainer(this);
            _gameSystems.Add(gameSystemInst);
        }

        public void InitSystems() =>
            _gameSystems.ForEach(system => system.DefineContainer(this));
        
        public void StartAllSystems() =>
            _gameSystems.ForEach(system => system.Start());
        
        public void StartSystem(GameSystem gameSystemInst) => 
            gameSystemInst.Start();

        public void NotifySystems(string message, System.Object data) =>
            Notify?.Invoke(message, data);
        
        public List<object>? MakeRequest(string message) => 
            MakeRequest(message, null);
        public List<object>? MakeRequest(string message, object requestObject)
        {
            List<object> responseList = new List<object>();

            foreach (GameSystem? system in _gameSystems)
            {
                Object response = system.OnRequest(message, requestObject);
                if(response != null) responseList.Add(response);
            }

            if (responseList.Count > 0) return responseList;
            else return null;
        }
        
        public async Task<List<object>?> MakeAsyncRequest(string message) => 
            await AsyncRequest(message, null);
        public async Task<List<object>?> MakeAsyncRequest(string message, object requestObject) =>
            await AsyncRequest(message, requestObject);
        private async Task<List<object>?> AsyncRequest(string message, object? requestObject)
        {
            List<Task<object>> tasks = new List<Task<object>>();
            List<object> responseList = new List<object>();

            foreach (var system in _gameSystems)
            {
                if(system.IsEnabled == false) continue;
                Task<object> task = system.OnAsyncRequest(message, requestObject);
                if(task != null) tasks.Add(task);
            }
            
            if (tasks.Count == 0) return null;
            
            await Task.WhenAll(tasks);

            foreach (var task in tasks)
                if(task.Result != null)
                    responseList.Add(task.Result);
            
            return responseList;
        }
        
        public void UpdateSystems() =>
            Update?.Invoke();
        
        public void UpdatePhysicsSystems() =>
            PhysUpdate?.Invoke();

        public bool TryGetSystem<T>(out GameSystem systemInst) where T : GameSystem
        {
            GameSystem? desiredSystem = GetSystem<T>();
            systemInst = null;
            
            if (desiredSystem != null) 
                systemInst = desiredSystem;

            return desiredSystem != null;
        }
        
        public GameSystem? GetSystem<T>() where T: GameSystem => 
            _gameSystems.Find(system => system.GetType() == typeof(T));
        public GameSystem? GetSystem(Type systemType) =>
            _gameSystems.Find(system => system.GetType() == systemType);
        
        public void ShutDownSystems() =>
            _gameSystems.ForEach(system => system.Stop());

        public void RemoveSystem<T>() where T: GameSystem
        {
            GameSystem? genericSystem = _gameSystems.Find(systemInst => systemInst.GetType() == typeof(T)); 
            if(genericSystem.IsEnabled) genericSystem.Stop();
            _gameSystems.Remove(genericSystem);
        }
        public void RemoveSystem<T>(T systemType) where T : GameSystem
        {
            GameSystem? genericSystem = _gameSystems.Find(systemInst => systemInst == systemType); //TODO Мне кажется эта хуета работает не правильно
            if(genericSystem.IsEnabled) genericSystem.Stop();
            _gameSystems.Remove(genericSystem);
        }
    }
}