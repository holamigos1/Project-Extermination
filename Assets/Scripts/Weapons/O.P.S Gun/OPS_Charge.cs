using Scripts.GameEnums;
using Scripts.TagHolders;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Weapons.OPS
{
    public class OPS_Charge : MonoBehaviour
    {
        [SerializeField] private Material Red;
        [SerializeField] private Material Green;
        [SerializeField] private Material Magenta;
        [SerializeField] private float SpeedMultiplier;
        [SerializeField] private float AntiGravitationForceMultiplier;
        public GameEnums.OPS_Charge ChargeType;
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

        private void Start()
        {
            if (ChargeType == GameEnums.OPS_Charge.Antigravity)
            {
                gameObject.GetComponent<Renderer>().material = Magenta;
                _thisRigidbody.useGravity = false;
            }

            if (ChargeType == GameEnums.OPS_Charge.Gravitation)
            {
                gameObject.GetComponent<Renderer>().material = Green;
                _thisRigidbody.useGravity = true;
                _thisRigidbody.velocity = new Vector3(0, 0, 0); //гасим ипульс от выстрела пушки
            }

            if (ChargeType == GameEnums.OPS_Charge.Horizontal)
            {
                gameObject.transform.localEulerAngles = new Vector3(
                    0, gameObject.transform.localEulerAngles.y, gameObject.transform.localEulerAngles.z);
                _thisRigidbody.freezeRotation = true;
                gameObject.GetComponent<Renderer>().material = Red;
                _thisRigidbody.useGravity = false;
            }
        }

        private void FixedUpdate()
        {
            if (_isPlanted) return;

            if (ChargeType == GameEnums.OPS_Charge.Horizontal)
            {
                if (_isColliding)
                    if (Mathf.Abs(_thisRigidbody.velocity.z) < 0.001f || Mathf.Abs(_thisRigidbody.velocity.x) < 0.001f)
                        PlaceCharge();
                _thisRigidbody.velocity = new Vector3(_thisRigidbody.velocity.x, 0, _thisRigidbody.velocity.z);
                _thisRigidbody.AddForce(transform.forward * SpeedMultiplier, ForceMode.Acceleration);
            }

            if (ChargeType == GameEnums.OPS_Charge.Antigravity)
            {
                if (_isColliding)
                    if (Mathf.Abs(_thisRigidbody.velocity.y) < 0.001f)
                        PlaceCharge();
                _thisRigidbody.AddForce(Vector3.up * (9.81f * AntiGravitationForceMultiplier), ForceMode.Acceleration);
            }

            if (ChargeType == GameEnums.OPS_Charge.Gravitation) //Логика зелёного заряда
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
            if (otherCollision.transform.tag == UnityTags.CLEAR_TAG) _isColliding = true;
            // otherCollision
            /*gameObject.layer = LayerMask.NameToLayer(UnityLayers.OPS_CHARGES_LAYER);
                isPlanted = true;
                StayPosition = transform.position;
                Debug.Log("Planted");*/
            if (otherCollision.transform.tag == UnityTags.WET_TAG)
            {
                /*isPlanted = true;
                Debug.Log("Planted");*/
            }

            if (otherCollision.transform.tag == UnityTags.DUSTY_TAG)
            {
                /*isPlanted = true;
                Debug.Log("Planted");*/
            }

            if (otherCollision.transform.tag == UnityTags.BOUND_TAG)
            {
                Debug.Log("Charge is out of bounds");
                Destroy(this);
            }

            if (otherCollision.transform.tag == UnityTags.BROKEN_TAG)
            {
                /*isPlanted = true;
                Debug.Log("Planted");*/
            }
        }

        private void PlaceCharge()
        {
            //transform.rotation = Quaternion.FromToRotation(collidedObj.contacts[0].point, collidedObj.contacts[0].normal);
            transform.rotation = Quaternion.LookRotation(_collidedObj.contacts[0].normal);
            _isPlanted = true;
            _stayPosition = transform.position;
            gameObject.layer = LayerMask.NameToLayer(UnityLayers.OPS_CHARGES_LAYER);
            _thisRigidbody.velocity = Vector3.zero;
            _thisRigidbody.useGravity = false;
            _thisRigidbody.isKinematic = true;
            transform.localPosition = _stayPosition;

            //логика растекания сферы..
        }
    }
}