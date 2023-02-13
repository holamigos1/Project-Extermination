using UnityEngine;

public class AntiGravitiBehaviour : MonoBehaviour
{
    [SerializeField] private float МножительИмпульсаВыстрела;
    [SerializeField] private float МножительГравитации;
    private bool _isShooted;
    private const float MIN_SPEED = 1;
    private Vector3 _prevPosition;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        _prevPosition = transform.position;
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _rb.AddForce(transform.forward * МножительИмпульсаВыстрела, ForceMode.Impulse);
            _isShooted = true;
        }

        if (_isShooted) _rb.AddForce(Vector3.up * 9.81f * МножительГравитации, ForceMode.Acceleration);
    }
}