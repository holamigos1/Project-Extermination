using Systems.Base;
using Systems.GameCamera;
using UnityEngine;

namespace Characters.Systems
{
    public class RaycastSystem : GameSystem
    {
        private GameObject _currentRaycastBlockingObj;
        
        public RaycastSystem(GameSystemsContainer container) : base(container)
        {
            
        }

        public override void Update()
        {
            base.Update();

            CastRayToCameraCenter();
        }

        public GameObject CastRayToCameraCenter()
        {
            Ray ray = new Ray(CameraSystem.CurrentMainCamera.transform.position, CameraSystem.CurrentMainCamera.transform.forward);
            
            Debug.DrawRay(CameraSystem.CurrentMainCamera.transform.position, CameraSystem.CurrentMainCamera.transform.forward*1000, Color.red);
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.transform == null) return null;
                
                if (_currentRaycastBlockingObj != hitInfo.transform.gameObject)
                {
                    _currentRaycastBlockingObj = hitInfo.transform.gameObject;
                    SystemsСontainer.NotifySystems("Raycast Update", _currentRaycastBlockingObj);
                    return _currentRaycastBlockingObj;
                }
            }
            
            return null;
        }
        

    }
}