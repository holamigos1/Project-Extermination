using UnityEngine;

namespace GameSystems.GameCamera
{
    public static class CameraSystem
    {
        //Эту чтучку нужно будет превратить во что-то стоящие
        
        public static Camera CurrentMainCamera
        {
            get
            {
                if (_currentMainCamera != null) return _currentMainCamera;
                
                _currentMainCamera = Camera.main;
                
                if (_currentMainCamera != null) return _currentMainCamera;
                
                GameObject inst = new GameObject("Camera");
                return inst.AddComponent<Camera>();

            }
            set => _currentMainCamera = value;
        }

        private static Camera _currentMainCamera;
    }
}