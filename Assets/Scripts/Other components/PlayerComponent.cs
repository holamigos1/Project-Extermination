using UnityEngine;

namespace Scripts.Components
{
    public class PlayerComponent : MonoBehaviour
    {
        // [SerializeField] private float Speed = 5;
        // [SerializeField] private float RunSpeed = 10;
        // [SerializeField] private float Sensitivity = 1;
        // [SerializeField] private float JumpStreght = 100;
        // [SerializeField] private Camera fpsCamera;
        //
        // private bool isInAir = false;
        // private bool isRunning = false;
        // private float rotationY = 0;
        // private float rotationX = 0;
        // private Vector3 lastPosition = new Vector3(0,0,0);
        // private Vector3 currentPosition = new Vector3(0,0,0);
        // private Rigidbody PlayerRigidbody;
        // private GameManager gameManager;
        // private InputHandlersController inputController;
        // private void OnEnable()
        // {
        //     
        //     gameManager = (GameManager) FindObjectOfType(typeof(GameManager));
        //     inputController = gameManager.InputControllerOnScene;
        //     PlayerRigidbody = GetComponent<Rigidbody>();
        //     fpsCamera = (Camera)GetComponentInChildren(typeof(Camera));
        // }
        // private void Start()
        // {
        //     lastPosition = transform.position;
        //     inputController.KeyHolded += keyCode =>
        //     {
        //         if (keyCode == KeyCode.W) Move(GameEnums.Vectors.Forward);
        //         if (keyCode == KeyCode.S) Move(GameEnums.Vectors.Back);
        //         if (keyCode == KeyCode.A) Move(GameEnums.Vectors.Left);
        //         if (keyCode == KeyCode.D) Move(GameEnums.Vectors.Right);
        //     };
        //     inputController.MouseMoved += Rotating;
        //     inputController.KeyTapped += key =>
        //     {
        //         if (key == KeyCode.LeftShift) isRunning = true;
        //     };
        //     inputController.KeyReleased += key =>
        //     {
        //         if (key == KeyCode.LeftShift) isRunning = false;
        //     };
        //     inputController.KeyTapped += keyCode =>
        //     {
        //         if (keyCode == KeyCode.Space) Jump();
        //     };
        // }
        //
        // private void FixedUpdate()
        // {
        //     isInAir = (PlayerRigidbody.velocity.y >= 0.0001 || PlayerRigidbody.velocity.y <= -0.0001)? true : false;
        //     lastPosition = transform.position;
        // }
        //
        // void Jump()
        // {
        //     if(isInAir) return;
        //     Debug.Log((currentPosition - lastPosition) * 10);
        //     PlayerRigidbody.AddForce(Vector3.up * JumpStreght + 
        //                              transform.forward *
        //         (Mathf.Abs(currentPosition - lastPosition )))
        //         , ForceMode.Impulse);
        // }
        // void Rotating(float mouseInputValueX, float mouseInputValueY)
        // {
        //     rotationX -= mouseInputValueY * Sensitivity;
        //     rotationY += mouseInputValueX * Sensitivity;
        //
        //     rotationX = Mathf.Clamp(rotationX, -30f, 30f);
        //     
        //     this.transform.eulerAngles = new Vector2(this.transform.eulerAngles.x, rotationY);
        //     fpsCamera.transform.eulerAngles = new Vector2(rotationX, this.transform.eulerAngles.y);
        // }
        // void Move(GameEnums.Vectors movingVector)
        // {
        //     if(isInAir) return;
        //     float moveSpeed = Speed;
        //     if (isRunning) moveSpeed = RunSpeed;
        //
        //     Vector3 addPosstion = new Vector3(0,0,0);
        //     
        //     if (movingVector == GameEnums.Vectors.Forward)
        //     {
        //         addPosstion = transform.forward * (Time.deltaTime * moveSpeed);
        //     }
        //
        //     if (movingVector == GameEnums.Vectors.Back)
        //     {
        //         addPosstion = -transform.forward * (Time.deltaTime * moveSpeed);
        //     }
        //
        //     if (movingVector == GameEnums.Vectors.Left)
        //     {
        //         addPosstion = -transform.right * (Time.deltaTime * moveSpeed);
        //     }
        //
        //     if (movingVector == GameEnums.Vectors.Right)
        //     {
        //         addPosstion = transform.right * (Time.deltaTime * moveSpeed);
        //     }
        //
        //     lastPosition = transform.position;
        //     transform.position += addPosstion;
        //     currentPosition = transform.position;
        // }
    }
}