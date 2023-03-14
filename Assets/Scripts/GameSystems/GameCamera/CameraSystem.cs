using System;
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

                throw new Exception("АА иди нахуе юбляя камеры ненет");
            }
            set => _currentMainCamera = value;
        }

        private static Camera _currentMainCamera;
    }
}