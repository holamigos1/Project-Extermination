using Misc;
using Misc.Extensions;
using UnityEngine;

namespace Characters.Humanoid
{
    public class AimRoot : MonoBehaviour
    {
        public Transform Transform { get; private set; }
        public Transform AimTarget => _aimTarget;

        [SerializeField] private Transform _aimTarget;
        
        private MinMaxDiapason _xAxisClamp; //Pitch
        private MinMaxDiapason _yAxisClamp; //Yaw
        private Vector3 _startLocalPosition;
        private Vector3 spoint;
        private Vector3 _planeNormal;
        
        public void Awake()
        {
            Transform = transform;
            _startLocalPosition = Transform.localPosition;
        }
        
        public void SetClamping(MinMaxDiapason xAxisClamp, MinMaxDiapason yAxisClamp)
        {
            _xAxisClamp = xAxisClamp;
            _yAxisClamp = yAxisClamp;
        }

        public void SyncWithHeadBone(Transform headBone) //магнитит рут к башке
        {
            //Todo а если голова больше? 
            //todo а если персонаж присел? или лежит? прицеливается?
            
            Vector3 tipOfNose = new Vector3(0,0.1f,0.15f); 
            Vector3 syncWorldPos = headBone.position;

            syncWorldPos += headBone.forward * tipOfNose.z;
            syncWorldPos += headBone.up * tipOfNose.y;
                                                                                            //Mathf.Cos компенсирует тярску при беге, смотря вперёд
            syncWorldPos.y = Mathf.Lerp(Transform.position.y, syncWorldPos.y, Mathf.Abs(Mathf.Cos(Transform.localRotation.eulerAngles.x)));
            Transform.position = syncWorldPos;
        }

        public bool Yaw(float angle) //влево вправо (y ось)
        {
            Vector3 localAngles = Transform.localRotation.eulerAngles;
            
            localAngles.y += angle;
            localAngles.y = localAngles.y.ClampAngle(_yAxisClamp, out bool isClamped).IfNegativeAngle();

            if (isClamped == false) 
                Transform.localRotation = Quaternion.Euler(localAngles);
            
            return isClamped;
        }
        
        public void Pitch(float angle) //вверх вниз (x ось)
        {
            Vector3 localAngles = Transform.localRotation.eulerAngles;
            
            localAngles.x += angle;
            localAngles.x = localAngles.x.ClampAngle(_xAxisClamp, out bool isClamped).IfNegativeAngle();
            
            if (isClamped == false) 
                Transform.localRotation = Quaternion.Euler(localAngles);
        }
        
        /// <summary>От AimRoot до его AimTarget</summary>
        public GameObject GetRayBlockObject(LayerMask mask)
        { 
            Collider rayBlockCollider = Transform.GetRaycastBlockingObject(AimTarget.position, mask).collider;
            
            return rayBlockCollider != null ?
                rayBlockCollider.gameObject :
                null;   
        }
    }
}