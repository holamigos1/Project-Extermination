using UnityEngine;

//Спиздил
namespace Movement.SourseMovment
{
    /// MouseLook rotates the transform based on the mouse delta.
    /// Minimum and Maximum values can be used to constrain the possible rotation
    /// To make an FPS style character:
    /// - Create a capsule.
    /// - Add the MouseLook script to the capsule.
    /// -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
    /// - Add FPSInputController script to the capsule
    /// -> A CharacterMotor and a CharacterController component will be automatically added.
    /// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
    /// - Add a MouseLook script to the camera.
    /// -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
    [AddComponentMenu("Camera-Control/Mouse Look")]
    public class MouseLook : MonoBehaviour
    {
        public enum RotationAxes
        {
            MouseXAndY = 0,
            MouseX = 1,
            MouseY = 2
        }

        [SerializeField] private bool isAbleToRotatePlayer = true;
        [SerializeField] private Transform HandsRoot;
        [SerializeField] private Transform Player;

        public RotationAxes axes = RotationAxes.MouseXAndY;
        public float sensitivityX = 15F;
        public float sensitivityY = 15F;

        public float minimumX = -360F;
        public float maximumX = 360F;

        public float minimumY = -60F;
        public float maximumY = 60F;

        public bool invertY;
        private float rotationX;

        private float rotationY;

        private void Start()
        {
            // Make the rigid body not change rotation
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().freezeRotation = true;
        }

        private void Update()
        {
            var ySens = sensitivityY;
            if (invertY) ySens *= -1f;

            if (axes == RotationAxes.MouseXAndY)
            {
                //float rotationX = transform.localEulerAngles.y + GetMouseX() * sensitivityX;

                rotationX += GetMouseX() * sensitivityX;
                rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);

                rotationY += GetMouseY() * ySens;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                transform.Rotate(0, GetMouseX() * sensitivityX, 0);
            }
            else
            {
                rotationY += GetMouseY() * ySens;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            }

            if (isAbleToRotatePlayer)
            {
                Player.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
                HandsRoot.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            }
        }

        private void OnEnable()
        {
            rotationY = transform.localRotation.y;
            rotationX = transform.localRotation.x;
        }

        private float GetMouseX()
        {
            return Input.GetAxis("Mouse X");
        }

        private float GetMouseY()
        {
            return Input.GetAxis("Mouse Y");
        }
    }
}