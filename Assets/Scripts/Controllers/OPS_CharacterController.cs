using DG.Tweening;
using Fragsurf.Movement;
using UnityEngine;

namespace Controllers
{
    public class OPS_CharacterController : MonoBehaviour
    {
        [SerializeField] private float connectionTime = 1f;
        [SerializeField] private float magnetizePointForwardMultiplier = 1;
        public bool isMagnetized;
        public bool isInProccesMagnetized;
        private GameObject _cameraHolder;
        private float _colliderRadius;
        private MouseLook _lockingScript;
        private Vector3 _startRotation;
        private Rigidbody _thisRb;

        private void OnEnable()
        {
            _colliderRadius = gameObject.GetComponent<SurfCharacter>().colliderSize.x;

            gameObject.GetComponent<SurfCharacter>().enabled = false;
            gameObject.GetComponentInChildren<PlayerAiming>().enabled = false;

            _lockingScript = gameObject.GetComponentInChildren<MouseLook>();
            _lockingScript.enabled = true;
            _cameraHolder = _lockingScript.gameObject;

            _thisRb = GetComponent<Rigidbody>();
            _thisRb.useGravity = false;
            _thisRb.isKinematic = true;
            _startRotation = _thisRb.rotation.eulerAngles;
        }

        public void Magnetize_ToPointByBack(Transform magnetizePoint)
        {
            if (isInProccesMagnetized) return;
            
            _lockingScript.enabled = false;
            isInProccesMagnetized = true;
            
            transform.DOMove(magnetizePoint.position + magnetizePoint.forward * 0.5f
                , connectionTime);
            
            _cameraHolder.transform.DOLocalRotate(Vector3.zero, connectionTime);
            
            transform.DORotateQuaternion(magnetizePoint.localRotation, connectionTime)
                .OnComplete(() =>
            {
                _lockingScript.enabled = true;
                isInProccesMagnetized = false;
                isMagnetized = true;
            });
        }

        public void UnMagnetize()
        {
            if (!isMagnetized) return;

            _thisRb.isKinematic = false;
            _thisRb.useGravity = true;
            _lockingScript.enabled = false;

            var rotateToGravity = new Vector3(_startRotation.x,
                _thisRb.rotation.eulerAngles.y,
                _thisRb.rotation.eulerAngles.z);

            _cameraHolder.transform.DOLocalRotate(Vector3.zero, connectionTime / 2);
            
            _thisRb.DORotate(rotateToGravity, connectionTime / 2).OnComplete(() =>
            {
                isMagnetized = false;
                EnableSourceMovement();
            });
        }

        private void EnableSourceMovement()
        {
            _thisRb.isKinematic = true;
            _thisRb.useGravity = false;
            GetComponent<SurfCharacter>().enabled = true;
            GetComponentInChildren<PlayerAiming>().enabled = true;
        }
    }
}