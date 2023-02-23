using UnityEngine;

public class AntiGravitiScript : MonoBehaviour
{
    [SerializeField] private float МножительИмпульсаВыстрела;
    [SerializeField] private float МножительГравитации;
    private bool isShooted;
    private readonly float minSpeed = 1;
    private Vector3 prevPosition;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.magnitude - prevPosition.magnitude) < minSpeed)
        {
        }

        prevPosition = transform.position;
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            rb.AddForce(transform.forward * МножительИмпульсаВыстрела, ForceMode.Impulse);
            isShooted = true;
        }

        if (isShooted) rb.AddForce(Vector3.up * 9.81f * МножительГравитации, ForceMode.Acceleration);
    }
}