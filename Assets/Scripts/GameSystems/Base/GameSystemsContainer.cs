using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameSystems.Base
{
    #nullable enable
    public class GameSystemsContainer
    {
        //TODO Прописать красивые методы отображения систем в GameSystemsContainer с возможностью добавления и изменения любых систем в проекте через Inspector
        public event Action<string, object>? SystemsNotify;
        public IEnumerable<GameSystem> gameSystems => _gameSystems;
        
        public GameSystemsContainer()
        {
            _gameSystems = new List<GameSystem>();
        }
        
        private readonly List<GameSystem> _gameSystems;

        public void AddSystem(GameSystem gameSystemInst)
        {
            if(_gameSystems.Contains(gameSystemInst)) return;
            
            gameSystemInst.DefineContainer(this);
            _gameSystems.Add(gameSystemInst);
            StartSystem(gameSystemInst);
        }

        public void StartSystem(GameSystem gameSystemInst) => 
            gameSystemInst.Start();
        
        public void NotifySystems(string message, System.Object data) =>
            SystemsNotify?.Invoke(message, data);
        
        public void NotifySystems(string message) =>
            SystemsNotify?.Invoke(message, null);
        

        public List<object>? MakeRequest(string message, object requestObject)
        {
            List<object> responseList = new List<object>();

            foreach (GameSystem system in _gameSystems)
            {
                Object response = system.OnRequest(message, requestObject);
                if(response != null) responseList.Add(response);
            }

            if (responseList.Count > 0) return responseList;
            else return null;
        }

        public async Task<List<object>?>? MakeAsyncRequest(string message, object requestObject) =>
            await AsyncRequest(message, requestObject)!;

        public async Task<List<object>?>? MakeAsyncRequest(string message) => 
            await AsyncRequest(message, null);

        private async Task<List<object>?>? AsyncRequest(string message, object requestObject)
        {
            List<Task<object>> tasks = new List<Task<object>>();
            List<object> responseList = new List<object>();

            foreach (var system in _gameSystems)
            {
                Task<object> result;
                
                if (requestObject == null) result = system.OnAsyncRequest(message);
                else result = system.OnAsyncRequest(message, requestObject);
                
                if(result != null) tasks.Add(result);
            }
            
            if (tasks.Count == 0) return null;
            
            await Task.WhenAll(tasks);

            foreach (var task in tasks)
                if(task.Result != null) responseList.Add(task.Result);
            
            return responseList;
        }
        
        public List<object>? MakeRequest(string message)
        {
            List<object> responseList = new List<object>();
            
            foreach (GameSystem system in _gameSystems)
            {
                var response = system.OnRequest(message, null);
                if(response != null) responseList.Add(response);
            }
            
            if (responseList.Count > 0) return responseList;
            else return null;
        }

        public void UpdateSystems() =>
            _gameSystems.ForEach(system => system.Update());
        
        public void UpdatePhysicsSystems() =>
            _gameSystems.ForEach(system => system.PhysicsUpdate()); 
        
        public GameSystem GetSystem<T>(T systemInst) where T: GameSystem => 
            _gameSystems.Find(system => system.GetType() == typeof(T));
        
        public GameSystem GetSystem(Type systemType) =>
            _gameSystems.Find(system => system.GetType() == systemType);
        
        public void ShutDownSystems() =>
            _gameSystems.ForEach(system => system.Stop());

        public void RemoveSystem<T>() where T: GameSystem
        {
            GameSystem genericSystem = _gameSystems.Find(systemInst => systemInst.GetType() == typeof(T)); 
            if(genericSystem.IsEnabled) genericSystem.Stop();
            _gameSystems.Remove(genericSystem);
        }
        
        public void RemoveSystem<T>(T systemType) where T : GameSystem
        {
            GameSystem genericSystem = _gameSystems.Find(systemInst => systemInst == systemType); //TODO Мне кажется эта хуета работает не правильно
            if(genericSystem.IsEnabled) genericSystem.Stop();
            _gameSystems.Remove(genericSystem);
        }
    }
}