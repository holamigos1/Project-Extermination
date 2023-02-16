using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Systems.Base;
using Systems.GameCamera;
using UnityEngine;

namespace Characters.Systems
{
    public class RaycastSystem : GameSystem
    {
        public RaycastSystem(MonoBehaviour anyMonobeh, GameSystemsContainer container) : base(container)
        {
            _anyMonobeh = anyMonobeh;
        }
        private const float RAYCAST_RANGE = 5f;
        private const float RAYCAST_RATE = 100f;
        private MonoBehaviour _anyMonobeh;
        private Coroutine _coroutine;
        private Queue<Task> _physicsTasks;

        public override void Start()
        {
            _physicsTasks = new Queue<Task>();
            base.Start();

            _coroutine = _anyMonobeh.StartCoroutine(DelayRaycast(RAYCAST_RATE));
        }

        public override object OnRequest(string message, object requestObject)
        {
            switch (message)
            {
                case "Get raycast object":
                    Transform camTransform = CameraSystem.CurrentMainCamera.transform;
                    Task<GameObject> awaitTask = CastRay(camTransform.position, camTransform.forward * RAYCAST_RANGE);;
                    _physicsTasks.Enqueue(awaitTask);
                    while (awaitTask.IsCompleted == false) { }
                    return awaitTask.Result;
            }

            return null;
        }

        public override void PhysicsUpdate()
        {
            while (_physicsTasks.Count > 0)
            {
                var task = _physicsTasks.Dequeue();
                if(task.IsCompleted == false) task.RunSynchronously();
            }
        }

        private IEnumerator DelayRaycast(float seconds)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                
                Transform camTransform = CameraSystem.CurrentMainCamera.transform;
                Task<GameObject> awaitTask = CastRay(camTransform.position, camTransform.forward * RAYCAST_RANGE);
                _physicsTasks.Enqueue(awaitTask);
                while (awaitTask.IsCompleted == false) { }
                SystemsСontainer.NotifySystems("Raycast Update", awaitTask.Result);
            }
        }

        private Task<GameObject> CastRay(Vector3 posFrom, Vector3 posTo)
        {
            Ray ray = new Ray(posFrom, posTo);
            Debug.DrawRay(posFrom, posTo, Color.red);
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Vector3.Distance(posFrom, posTo)))
            {
                if (hitInfo.transform == null) return Task.FromResult<GameObject>(null);
                return Task.FromResult(hitInfo.transform.gameObject);
            }

            return Task.FromResult<GameObject>(null);
        }

        public override void Stop()
        {
            base.Stop();
            
            _anyMonobeh.StopCoroutine(_coroutine);
        }
    }
}