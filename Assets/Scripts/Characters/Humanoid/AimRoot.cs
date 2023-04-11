﻿using Misc;
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

        public void SyncHorizontalPosition(Transform point) //магнитит рут к башке
        {
            Vector3 syncWorldPos = point.position;
            syncWorldPos += point.forward * _startLocalPosition.z;
            syncWorldPos.y = _startLocalPosition.y;
            
            Transform.position = syncWorldPos;
        }

        public bool Yaw(float angle) //влево вправо (y ось)
        {
            Vector3 localAngles = Transform.localRotation.eulerAngles;
            localAngles.y += angle;
            localAngles.y = localAngles.y.ClampAngle(_yAxisClamp, out bool isClamped).IfNegativeAngle();

            if (isClamped == false) Transform.localRotation = Quaternion.Euler(localAngles);
            
            return isClamped;
        }
        
        public void Pitch(float angle) //вверх вниз (x ось)
        {
            Vector3 localAngles = Transform.localRotation.eulerAngles;
            localAngles.x += angle;
            localAngles.x = localAngles.x.ClampAngle(_xAxisClamp, out bool isClamped).IfNegativeAngle();
            
            if (isClamped == false) Transform.localRotation = Quaternion.Euler(localAngles);
        }
    }
}