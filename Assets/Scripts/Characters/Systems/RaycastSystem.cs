using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameSystems.Base;
using GameSystems.GameCamera;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Systems
{
    /// <summary>
    /// Система каста лучей 
    /// </summary>
    [Serializable]
    public class RaycastSystem : GameSystem
    {
        [Title("Система каста лучей.", 
            "Кастит лучи и возвращает объекты в которые лучи попали.")] 
        [ShowInInspector] [HideLabel] [DisplayAsString][PropertySpace(SpaceBefore = -5,SpaceAfter = -20)]
        #pragma warning disable CS0219
        private string _info = "";

        public RaycastSystem() { }

        public RaycastSystem(RaycastSystemData raycastData) =>
            _raycastData = raycastData;
        
        private const float RAYCAST_RANGE = 5f; //TODO Убери в конфиг
        private const float RAYCAST_RATE = 0.1f;

        [SerializeField] [BoxGroup("Данные для каста лучей")] [HideLabel]
        private RaycastSystemData _raycastData;
        private Transform _mainCameraTransform;
        private Coroutine _coroutine;
        private Queue<Task> _physicsTasks;

        public override void Start()
        {
            _mainCameraTransform = CameraSystem.CurrentMainCamera.transform;

            _physicsTasks = new Queue<Task>();
            SystemsСontainer.PhysUpdate += PhysicsUpdate;

            _coroutine = _raycastData.AnyMonobeh.StartCoroutine(DelayRaycast(RAYCAST_RATE));
        }
        

        public override void PhysicsUpdate()
        {
            while (_physicsTasks.Count > 0)
            {
                var task = _physicsTasks.Dequeue();
                if(task.IsCompleted == false) task.RunSynchronously(); //Выполнит задание в Fixed Update
            }
        }
        
        public override async Task<object> OnAsyncRequest(string message, object data)
        {
            switch (message)
            {
                case "Get raycast object":
                    var gameObject = await GetRaycastBlockingObjAsync(
                       _mainCameraTransform.position, 
                       _mainCameraTransform.forward * RAYCAST_RANGE);
                   return gameObject;
            }

            return null;
        }
        
        private IEnumerator DelayRaycast(float seconds)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                var task = GetRaycastBlockingObjAsync(_mainCameraTransform.position, 
                    _mainCameraTransform.forward * RAYCAST_RANGE);
                yield return new WaitUntil(() => task.IsCompleted);
            }
        }


        private async Task<GameObject> GetRaycastBlockingObjAsync(Vector3 rayStartPos, Vector3 rayDirectionPos)
        {
            Task<GameObject> awaitTask = 
                new Task<GameObject>(() => GetRaycastBlockingObj(rayStartPos, rayDirectionPos));
            
            _physicsTasks.Enqueue(awaitTask);
            
            GameObject result = await awaitTask;
            
            return result;
        }
        

        private GameObject GetRaycastBlockingObj(Vector3 rayStartPos, Vector3 rayDirectionPos)
        {
            if(_raycastData.RayblockLayers.value == 0) Debug.LogWarning($"RayblockLayers для системы {this.GetType().Name} не установленны.");
            Debug.DrawRay(rayStartPos, rayDirectionPos, Color.red);//TODO Удали как наиграешься
            
            Ray ray = new Ray(rayStartPos, rayDirectionPos);
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 
                                Vector3.Distance(rayStartPos, rayDirectionPos), 
                                _raycastData.RayblockLayers) == false) return null;
            
            if (hitInfo.transform == null) return null;
            
            return hitInfo.transform.gameObject;
        }

        public override void Stop()
        {
            _raycastData.AnyMonobeh.StopCoroutine(_coroutine);
            SystemsСontainer.Update -= PhysicsUpdate;
        }
    }
    
    [Serializable]
    public class RaycastSystemData
    {
        public LayerMask RayblockLayers => _rayblockLayers;
        public MonoBehaviour AnyMonobeh => _anyMonobeh;
        
        public RaycastSystemData(MonoBehaviour anyMonobeh, LayerMask rayblockLayers)
        {
            _anyMonobeh = anyMonobeh;
            _rayblockLayers = rayblockLayers;
        }
        
        [SerializeField] 
        [LabelText("Слои блокирующие луч")]
        private LayerMask _rayblockLayers;
        
        [SerializeField] 
        [LabelText("Любой скрипт MonoBehaviour")]
        private MonoBehaviour _anyMonobeh;
    }
}