using DG.Tweening;
using Movement.SourseMovment.Movement;
using UnityEngine;

namespace Movement.SourseMovment
{
    [RequireComponent(typeof(CharacterMovemnet), typeof(Rigidbody))]
    public class MagnitatonBehaviour : MonoBehaviour
    {
        public bool IsMagnetized => _isMagnetized;
        
        [SerializeField] private float _connectionTime = 1f;
        [SerializeField] private bool _isMagnetized;
        [SerializeField] private bool _isInProccesMagnetized;
        private GameObject _cameraHolder;
        private MouseLook _lockingScript;
        private Rigidbody _thisRb;

        private void OnEnable()
        {
            gameObject.GetComponent<CharacterMovemnet>().enabled = false;

            _lockingScript = _lockingScript != null ? _lockingScript : GetComponentInChildren<MouseLook>();
            _lockingScript.enabled = true;
            _cameraHolder = _lockingScript.gameObject;

            _thisRb = _thisRb != null ? _thisRb : GetComponent<Rigidbody>();
            _thisRb.useGravity = false;
            _thisRb.isKinematic = true; 
        }

        public void Magnetize_ToPointByBack(Transform magnetizePoint)
        {
            if (_isInProccesMagnetized) return;
            
            _lockingScript.enabled = false;
            _isInProccesMagnetized = true;
            
            transform.DOMove(magnetizePoint.position + magnetizePoint.forward * 0.5f
                ,_connectionTime);
            
            _cameraHolder.transform.DOLocalRotate(Vector3.zero, _connectionTime);
            
            transform.DORotateQuaternion(magnetizePoint.localRotation, _connectionTime)
                .OnComplete(() =>
            {
                _lockingScript.enabled = true;
                _isInProccesMagnetized = false;
                _isMagnetized = true;
            });
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void UnMagnetize()
        {
            if (!_isMagnetized) return;

            _thisRb.isKinematic = false;
            _thisRb.useGravity = true;
            _lockingScript.enabled = false;

            var rotateToGravity = new Vector3(0/*мне похуй*/,
                                            _thisRb.rotation.eulerAngles.y,
                                            _thisRb.rotation.eulerAngles.z);

            _cameraHolder.transform.DOLocalRotate(Vector3.zero, _connectionTime);
            
            _thisRb.DORotate(rotateToGravity, _connectionTime)
                .OnComplete(() =>
                {
                    _isMagnetized = false;
                    EnableSourceMovement();
                });
        }

        private void EnableSourceMovement()
        {
            _thisRb.isKinematic = true;
            _thisRb.useGravity = false;
            GetComponent<CharacterMovemnet>().enabled = true;
        }
    }
}