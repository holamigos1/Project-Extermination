using UnityEngine;

namespace Weapons.O.P.S_Gun
{
    public class OPS_Charge : MonoBehaviour
    {
        [SerializeField] private Material _red;
        [SerializeField] private Material _green;
        [SerializeField] private Material _magenta;
        [SerializeField] private float _speedMultiplier;
        [SerializeField] private float _antiGravitationForceMultiplier;
        [SerializeField] private Scripts.GameEnums.OPS_Charge _chargeType;
        private Collision _collidedObj;
        private bool _isColliding;
        private bool _isPlanted;
        private Vector3 _lastPos;
        private Vector3 _stayPosition;
        private Collider _thisCollider;
        private Rigidbody _thisRigidbody;

        private void Awake()
        {
            _lastPos = transform.position;
            _thisRigidbody = GetComponent<Rigidbody>();
            _thisCollider = GetComponent<Collider>();
        }

        public void Setup(Scripts.GameEnums.OPS_Charge chargeType)
        {
            _chargeType = chargeType;
        }
        
        private void Start()
        {
            if (_chargeType == Scripts.GameEnums.OPS_Charge.Antigravity)
            {
                gameObject.GetComponent<Renderer>().material = _magenta;
                _thisRigidbody.useGravity = false;
            }

            if (_chargeType == Scripts.GameEnums.OPS_Charge.Gravitation)
            {
                gameObject.GetComponent<Renderer>().material = _green;
                _thisRigidbody.useGravity = true;
                _thisRigidbody.velocity = new Vector3(0, 0, 0); //гасим ипульс от выстрела пушки
            }

            if (_chargeType == Scripts.GameEnums.OPS_Charge.Horizontal)
            {
                gameObject.transform.localEulerAngles = new Vector3(
                    0, gameObject.transform.localEulerAngles.y, gameObject.transform.localEulerAngles.z);
                _thisRigidbody.freezeRotation = true;
                gameObject.GetComponent<Renderer>().material = _red;
                _thisRigidbody.useGravity = false;
            }
        }

        private void FixedUpdate()
        {
            if (_isPlanted) return;

            if (_chargeType == Scripts.GameEnums.OPS_Charge.Horizontal)
            {
                if (_isColliding)
                    if (Mathf.Abs(_thisRigidbody.velocity.z) < 0.001f || Mathf.Abs(_thisRigidbody.velocity.x) < 0.001f)
                        PlaceCharge();
                
                _thisRigidbody.velocity = new Vector3(_thisRigidbody.velocity.x, 0, _thisRigidbody.velocity.z);
                _thisRigidbody.AddForce(transform.forward * _speedMultiplier, ForceMode.Acceleration);
            }

            if (_chargeType == Scripts.GameEnums.OPS_Charge.Antigravity)
            {
                if (_isColliding)
                    if (Mathf.Abs(_thisRigidbody.velocity.y) < 0.001f)
                        PlaceCharge();
                
                _thisRigidbody.AddForce(Vector3.up * (9.81f * _antiGravitationForceMultiplier), ForceMode.Acceleration);
            }

            if (_chargeType == Scripts.GameEnums.OPS_Charge.Gravitation) //Логика зелёного заряда
            {
                if (_isColliding)
                    if (Mathf.Abs(_thisRigidbody.velocity.y) < 0.001f)
                        PlaceCharge();
                
                return;
            }

            _lastPos = transform.position;
        }


        
        private void OnCollisionEnter(Collision otherCollision)
        {
            _collidedObj = otherCollision;
            
            if (otherCollision.transform.CompareTag(Data.Tags.GameTags.CLEAR_TAG)) _isColliding = true;
            if (otherCollision.transform.CompareTag(Data.Tags.GameTags.BOUND_TAG)) Destroy(this);
        }

        private void PlaceCharge()
        {
            //transform.rotation = Quaternion.FromToRotation(collidedObj.contacts[0].point, collidedObj.contacts[0].normal);
            transform.rotation = Quaternion.LookRotation(_collidedObj.contacts[0].normal);
            _isPlanted = true;
            _stayPosition = transform.position;
            gameObject.layer = LayerMask.NameToLayer(Data.Layers.GameLayers.OPS_CHARGES_LAYER);
            _thisRigidbody.velocity = Vector3.zero;
            _thisRigidbody.useGravity = false;
            _thisRigidbody.isKinematic = true;
            transform.localPosition = _stayPosition;

            //логика растекания сферы..
        }
    }
}