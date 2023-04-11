using UnityEngine;

namespace Characters.Humanoid
{
    public class AimRoot : MonoBehaviour
    {
        public Transform Transform { get; private set; }
        
        private Range _xAxisClamp; //Pitch
        private Range _yAxisClamp; //Yaw
        private Vector3 _startLocalPosition;
        
        public void Awake()
        {
            Transform = transform;
            _startLocalPosition = Transform.localPosition;
        }
        
        public void SetClamping(Range xAxisClamp, Range yAxisClamp)
        {
            _xAxisClamp = xAxisClamp;
            _yAxisClamp = yAxisClamp;
        }

        public void SyncHorizontalPosition(Transform point)
        {
            Vector3 syncWorldPos = point.position;
            Vector3 rootWorldPos = Transform.position;
            syncWorldPos.y = rootWorldPos.y;
            syncWorldPos.z += _startLocalPosition.z;
            
            Transform.position = syncWorldPos;
        }

        public bool Yaw(float angle) //влево вправо (y ось)
        {
            Vector3 localAngles = Transform.localRotation.eulerAngles;
            localAngles.y += angle;
            localAngles.y = localAngles.y.ClampAngle(_yAxisClamp, out bool isClamped).IfNegativeAngle();
            Transform.eulerAngles = localAngles;
            
            return isClamped;
        }
        
        public void Pitch(float angle) //вверх вниз (x ось)
        {
            Vector3 localRotation = Transform.localRotation.eulerAngles;
            localRotation.x += angle;
            localRotation.x = localRotation.x.ClampAngle(_xAxisClamp).IfNegativeAngle();
            Transform.eulerAngles = localRotation;
        }
    }
}