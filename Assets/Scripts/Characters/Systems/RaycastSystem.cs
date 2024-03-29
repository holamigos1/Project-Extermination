﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Systems.Base;
using Systems.GameCamera;
using UnityEngine;

namespace Characters.Systems
{
    public class RaycastSystem : GameSystem
    {
        /// <summary>
        /// Система каста лучей 
        /// </summary>
        public RaycastSystem(RaycastSystemData raycastData)
        {
            _raycastData = raycastData;
        }
        
        private const float RAYCAST_RANGE = 5f; //TODO Убери в конфиг
        private const float RAYCAST_RATE = 0.1f;
        
        private RaycastSystemData _raycastData;
        private Coroutine _coroutine;
        private Queue<Task> _physicsTasks;
        private static Transform _mainCameraTransform;

        public override void Start()
        {
            _physicsTasks = new Queue<Task>();
            base.Start();

            _coroutine = _raycastData.AnyMonobeh.StartCoroutine(DelayRaycast(RAYCAST_RATE));
            _mainCameraTransform = CameraSystem.CurrentMainCamera.transform;
        }

        public override void PhysicsUpdate()
        {
            while (_physicsTasks.Count > 0)
            {
                var task = _physicsTasks.Dequeue();
                if(task.IsCompleted == false) task.RunSynchronously(); //Выполнит задание в Fixed Update
            }
        }
        
        public override async Task<object> OnAsyncRequest(string message, object requestObject)
        {
            switch (message)
            {
                case "Get raycast object":
                   var gameObject = await GetRaycastBlockingObjAsync(_mainCameraTransform.position, _mainCameraTransform.forward * RAYCAST_RANGE);
                   return gameObject;
            }

            return null;
        }
        
        private IEnumerator DelayRaycast(float seconds)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                var task = GetRaycastBlockingObjAsync(_mainCameraTransform.position, _mainCameraTransform.forward * RAYCAST_RANGE);
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
            base.Stop();
            _raycastData.AnyMonobeh.StopCoroutine(_coroutine);
        }
    }
    
    public class RaycastSystemData
    {
        public LayerMask RayblockLayers => _rayblockLayers;
        public MonoBehaviour AnyMonobeh => _anyMonobeh;
        
        public RaycastSystemData(MonoBehaviour anyMonobeh, LayerMask rayblockLayers)
        {
            _anyMonobeh = anyMonobeh;
            _rayblockLayers = rayblockLayers;
        }
        
        private LayerMask _rayblockLayers;
        private MonoBehaviour _anyMonobeh;
    }
}