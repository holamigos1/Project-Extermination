using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Mathematics;

namespace Movement.SourseMovment
{
    public class MouseLook : MonoBehaviour
    {
        public enum RotationAxes
        {
            MouseXAndY = 0,
            MouseX = 1,
            MouseY = 2
        }

        [SerializeField] 
        private bool _isAbleToRotatePlayer = true;
        
        [ShowIf("_isAbleToRotatePlayer")] [SerializeField] 
        private Transform _playerTransform;
        
        [EnumPaging] [SerializeField] 
        private RotationAxes _axes = RotationAxes.MouseXAndY;
        
        [SerializeField] 
        private float2 _sensitivity = 15F;
        
        [MinMaxSlider(-180,180,true)] [SerializeField] 
        private Vector2Int _xAngleLimits;
        
        private float2 _rotation;

        private void Start()
        {
            // Make the rigid body not change rotation
            if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().freezeRotation = true;
        }

        private void Update()
        {
            switch (_axes)
            {
                case RotationAxes.MouseXAndY when _isAbleToRotatePlayer:
                    _rotation.y += Input.GetAxis("Mouse X") * _sensitivity.x;
                    _rotation.x += Input.GetAxis("Mouse Y") * _sensitivity.y;
                    _rotation.x = math.clamp(_rotation.x, _xAngleLimits.x, _xAngleLimits.y);
                    _playerTransform.localEulerAngles = new Vector3(0, _rotation.y, 0);
                    transform.localEulerAngles = new Vector3(-_rotation.x, 0 , 0);
                    break;
                
                case RotationAxes.MouseXAndY:
                    _rotation.x += Input.GetAxis("Mouse X") * _sensitivity.x;
                    _rotation.y += Input.GetAxis("Mouse Y") * _sensitivity.y;
                    transform.localEulerAngles = new Vector3(-_rotation.y, _rotation.x, 0);
                    break;
                
                case RotationAxes.MouseX:
                    transform.Rotate(0, Input.GetAxis("Mouse X") * _sensitivity.x, 0);
                    break;
                
                case RotationAxes.MouseY:
                    _rotation.y += Input.GetAxis("Mouse Y") * _sensitivity.y;
                    transform.localEulerAngles = new Vector3(-_rotation.y, transform.localEulerAngles.y, 0);
                    break;
            }
        }

        private void OnEnable()
        {
            _rotation.y = transform.localRotation.y;
            _rotation.x = transform.localRotation.x;
        }
    }
}