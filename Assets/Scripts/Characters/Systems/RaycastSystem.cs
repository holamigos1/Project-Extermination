using UnityEngine;

namespace Characters.Systems
{
    public class RaycastSystem
    {
        private Camera _currentMainCamera;
        
        public RaycastSystem(Camera mainCamera)
        {
            _currentMainCamera = mainCamera;
        }

        public Object CastRayToCameraCenter()
        {
            Object rayBlockingObject = new Object();
            
            Ray ray = new Ray(_currentMainCamera.transform.position, _currentMainCamera.transform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                rayBlockingObject = hitInfo.transform.gameObject;
            }
            
            return rayBlockingObject;
        }
    }
}