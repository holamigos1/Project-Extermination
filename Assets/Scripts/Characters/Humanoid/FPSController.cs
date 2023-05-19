// Designed by Kinemation, 2023

using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.Core;
using Kinemation.FPSFramework.Runtime.Layers;
using UnityEngine;
using Weapons;

namespace Characters.Humanoid
{
    public enum FPSMovementState //TODO Сделать через стейт машину
    {
        Idle,
        Walking,
        Running,
        Sprinting
    }
    
    public enum FPSPoseState //TODO Сделать через стейт машину
    {
        Standing,
        Crouching
    }
    
    public enum FPSActionState //TODO Сделать через стейт машину
    {
        None,
        Ready,
        Aiming,
        PointAiming,
    }

    public enum CameraState //TODO Сделать через стейт машину
    {
        FirstPerson,
        ThirdPerson,
        TacCamera
    }
    
    // An example-controller class
    public class FPSController : MonoBehaviour
    {
        [Header("FPS Animator Core")]
        [SerializeField] private CoreAnimComponent coreAnimComponent;
        [SerializeField] private Animator animator;
        [SerializeField] private Vector2 freeLookAngle;

        [Header("FPS Animator Layers")]
        // Animation Layers
        [SerializeField] private LookLayer lookLayer;
        [SerializeField] private AdsLayer adsLayer;
        [SerializeField] private BlendingLayer blendLayer;
        [SerializeField] private SwayLayer swayLayer;
        [SerializeField] private DemoLoco locoLayer;
        [SerializeField] private SlotLayer slotLayer;
        // Animation Layers

        [Header("FPS Animator Dynamic Motions")]
        [SerializeField] private DynamicMotion aimMotion;
        [SerializeField] private DynamicMotion leanMotion;

        [Header("Character Controls")]
        [SerializeField] private float crouchHeight;
        [SerializeField] private float sensitivity;
        
        [Header("Camera")]
        [SerializeField] private Transform mainCamera;
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private Transform fpCameraBone;
        [SerializeField] private Transform tpCameraBone;
        [SerializeField] private Transform tacCameraBone;
        private SpringCameraShake cameraShake;

        private CameraState cameraState = CameraState.FirstPerson;
        private CameraState cachedCameraState;

        [Header("Movement")] 
        [SerializeField] private bool shouldMove;
        private float speed;
        [SerializeField] private float walkingSpeed = 10f;
        [SerializeField] private float sprintSpeed = 25f;
        [SerializeField] private CharacterController controller;
        
        [SerializeField] private List<Weapon> weapons;
        private RecoilAnimation _recoilAnimation;

        private Vector2 _playerInput;
        // Used for free-look
        private Vector2 _freeLookInput;
        private Vector2 _smoothMoveInput;
        private Vector2 lastMoveInput;
        
        private int _weaponIndex;

        private float _fireTimer = -1f;
        private int _bursts;
        private bool _aiming;
        private bool _freeLook;
        private bool _reloading;

        private float _lowerCapsuleOffset;
        private CharAnimData _charAnimData;
        private FPSActionState actionState;
        private FPSMovementState movementState;
        private FPSPoseState poseState;
        
        private static readonly int Sprint = Animator.StringToHash("sprint");
        private static readonly int Crouch1 = Animator.StringToHash("crouch");
        private static readonly int Moving = Animator.StringToHash("moving");
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int MoveY = Animator.StringToHash("moveY");
        
        private void InitLayers()
        {
            coreAnimComponent = GetComponent<CoreAnimComponent>();
            animator = GetComponent<Animator>();
            _recoilAnimation = GetComponent<RecoilAnimation>();

            lookLayer = GetComponent<LookLayer>();
            adsLayer = GetComponent<AdsLayer>();
            blendLayer = GetComponent<BlendingLayer>();
            locoLayer = GetComponent<DemoLoco>();
            swayLayer = GetComponent<SwayLayer>();
            slotLayer = GetComponent<SlotLayer>();
        }

        private void Start()
        {
            Application.targetFrameRate = 120;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _lowerCapsuleOffset = controller.center.y - controller.height / 2f;
            speed = walkingSpeed;

            cameraShake = mainCamera.gameObject.GetComponent<SpringCameraShake>();

            InitLayers();
            EquipWeapon();
        }

        private void EquipWeapon()
        {
            var gun = weapons[_weaponIndex];
            
            _bursts = gun.burstAmount;
            _recoilAnimation.Init(gun.recoilData, gun.fireRate, gun.fireMode);
            coreAnimComponent.OnGunEquipped(gun.gunData);
            
            gun.gameObject.SetActive(true);
        }
        
        private void ChangeWeapon()
        {
            int newIndex = _weaponIndex;
            newIndex++;
            if (newIndex > weapons.Count - 1)
            {
                newIndex = 0;
            }

            weapons[_weaponIndex].gameObject.SetActive(false);
            _weaponIndex = newIndex;
            
            EquipWeapon();
        }

        public void ToggleAim()
        {
            _aiming = !_aiming;

            if (_aiming)
            {
                actionState = FPSActionState.Aiming;
                adsLayer.SetAdsAlpha(1f);
                swayLayer.SetFreeAimEnable(false);
                slotLayer.PlayMotion(aimMotion);
            }
            else
            {
                actionState = FPSActionState.None;
                adsLayer.SetAdsAlpha(0f);
                adsLayer.SetPointAlpha(0f);
                swayLayer.SetFreeAimEnable(true);
                slotLayer.PlayMotion(aimMotion);
            }
            
            _recoilAnimation.isAiming = _aiming;
        }
        
        public void ChangeScope()
        {
            coreAnimComponent.OnSightChanged(GetGun().GetScope());
        }

        private void Fire()
        {
            cameraShake.PlayCameraShake();
            GetGun().OnFire();
            _recoilAnimation.Play();
        }

        private void OnFirePressed()
        {
            Fire();
            _bursts = GetGun().burstAmount - 1;
            _fireTimer = 0f;
        }

        private Weapon GetGun()
        {
            return weapons[_weaponIndex];
        }

        private void OnFireReleased()
        {
            _recoilAnimation.Stop();
            _fireTimer = -1f;
        }

        private void SprintPressed()
        {
            if (poseState == FPSPoseState.Crouching)
            {
                return;
            }
            
            adsLayer.SetLayerAlpha(0f);
            lookLayer.SetLayerAlpha(0.4f);
            blendLayer.SetLayerAlpha(0f);
            locoLayer.SetSprint(true);
            locoLayer.SetReadyWeight(0f);

            movementState = FPSMovementState.Sprinting;
            actionState = FPSActionState.None;
            
            _recoilAnimation.Stop();

            speed = sprintSpeed;
            animator.SetBool(Sprint, true);
        }
        
        private void SprintReleased()
        {
            if (poseState == FPSPoseState.Crouching)
            {
                return;
            }
            
            adsLayer.SetLayerAlpha(1f);
            lookLayer.SetLayerAlpha(1f);
            blendLayer.SetLayerAlpha(1f);
            locoLayer.SetSprint(false);
            
            movementState = FPSMovementState.Walking;
            
            speed = walkingSpeed;
            animator.SetBool(Sprint, false);
        }

        private void Crouch()
        {
            var height = controller.height;
            height *= crouchHeight;
            controller.height = height;
            controller.center = new Vector3(0f, _lowerCapsuleOffset + height / 2f, 0f);
            speed *= 0.7f;

            lookLayer.SetPelvisWeight(0f);
            
            poseState = FPSPoseState.Crouching;
            animator.SetBool(Crouch1, true);
        }

        private void Uncrouch()
        {
            var height = controller.height;
            height /= crouchHeight;
            controller.height = height;
            controller.center = new Vector3(0f, _lowerCapsuleOffset + height / 2f, 0f);
            speed /= 0.7f;
            
            lookLayer.SetPelvisWeight(1f);

            poseState = FPSPoseState.Standing;
            animator.SetBool(Crouch1, false);
        }

        private void ProcessActionInput() //TODO Использовать NewInputSystem
        {
            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                Application.Quit(0);
            }

            _charAnimData.leanDirection = 0;
            
            if(Input.GetKeyDown(KeyCode.P))
            {
                if (cameraState == CameraState.TacCamera)
                {
                    return;
                }
                
                if (cameraState == CameraState.FirstPerson)
                {
                    cameraState = CameraState.ThirdPerson;

                    cameraHolder.parent = transform;
                    
                    cameraHolder.localPosition = new Vector3(0f, tpCameraBone.localPosition.y, 0f);
                    cameraHolder.localRotation = Quaternion.identity;

                    mainCamera.position = tpCameraBone.position;
                    mainCamera.localRotation = Quaternion.identity;
                }
                else
                {
                    cameraState = CameraState.FirstPerson;
                    cameraHolder.parent = fpCameraBone;
                    
                    cameraHolder.localPosition = Vector3.zero;
                    cameraHolder.localRotation = Quaternion.identity;
                    
                    mainCamera.localPosition = Vector3.zero;
                    mainCamera.localRotation = Quaternion.identity;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SprintPressed();
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                SprintReleased();
            }
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                ChangeWeapon();
            }
            
            if (movementState == FPSMovementState.Sprinting)
            {
                return;
            }
            
            if (actionState != FPSActionState.Ready)
            {
                if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyUp(KeyCode.Q)
                    || Input.GetKeyDown(KeyCode.E) || Input.GetKeyUp(KeyCode.E))
                {
                    slotLayer.PlayMotion(leanMotion);
                }
                
                if (Input.GetKey(KeyCode.Q))
                {
                    _charAnimData.leanDirection = 1;
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    _charAnimData.leanDirection = -1;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    OnFirePressed();
                }
            
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    OnFireReleased();
                }
            
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    ToggleAim();
                }
            
                if (Input.GetKeyDown(KeyCode.V))
                {
                    ChangeScope();
                }
            
                if (Input.GetKeyDown(KeyCode.B) && _aiming)
                {
                    if (actionState == FPSActionState.PointAiming)
                    {
                        adsLayer.SetPointAlpha(0f);
                        actionState = FPSActionState.Aiming;
                    }
                    else
                    {
                        adsLayer.SetPointAlpha(1f);
                        actionState = FPSActionState.PointAiming;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (poseState == FPSPoseState.Standing)
                {
                    Crouch();
                }
                else
                {
                    Uncrouch();
                }
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                if (actionState == FPSActionState.Ready)
                {
                    actionState = FPSActionState.None;
                    locoLayer.SetReadyWeight(0f);
                    lookLayer.SetLayerAlpha(1f);
                }
                else
                {
                    actionState = FPSActionState.Ready;
                    locoLayer.SetReadyPose(ReadyPose.LowReady);
                    locoLayer.SetReadyWeight(1f);
                    lookLayer.SetLayerAlpha(.5f);
                    OnFireReleased();
                }
            }
        }

        private void ProcessLookInput()
        {
            _freeLook = Input.GetKey(KeyCode.X); //TODO Использовать NewInputSystem

            float deltaMouseX = Input.GetAxis("Mouse X") * sensitivity;
            float deltaMouseY = -Input.GetAxis("Mouse Y") * sensitivity;

            if (_freeLook)
            {
                // No input for both controller and animation component. We only want to rotate the camera
                
                _freeLookInput.x += deltaMouseX;
                _freeLookInput.y += deltaMouseY;
            
                _freeLookInput.x = Mathf.Clamp(_freeLookInput.x, -freeLookAngle.x, freeLookAngle.x);
                _freeLookInput.y = Mathf.Clamp(_freeLookInput.y, -freeLookAngle.y, freeLookAngle.y);
                
                return;
            }

            _freeLookInput = CoreToolkitLib.Glerp(_freeLookInput, Vector2.zero, 15f);
            
            _playerInput.x += deltaMouseX;
            _playerInput.y += deltaMouseY;
            
            _playerInput.x = Mathf.Clamp(_playerInput.x, -90f, 90f);
            _playerInput.y = Mathf.Clamp(_playerInput.y, -90f, 90f);
            
            if (shouldMove && !_freeLook)
            {
                transform.Rotate(Vector3.up * deltaMouseX);
            }
            
            _charAnimData.AddAimInput(new Vector2(deltaMouseX, deltaMouseY));
        }

        private void UpdateFiring()
        {
            if (_recoilAnimation.fireMode != FireMode.Semi && _fireTimer >= 60f / GetGun().fireRate)
            {
                Fire();

                if (_recoilAnimation.fireMode == FireMode.Burst)
                {
                    _bursts--;

                    if (_bursts == 0)
                    {
                        _fireTimer = -1f;
                        OnFireReleased();
                    }
                    else
                    {
                        _fireTimer = 0f;
                    }
                }
                else
                {
                    _fireTimer = 0f;
                }
            }

            if (_fireTimer >= 0f)
            {
                _fireTimer += Time.deltaTime;
            }
        }

        private void UpdateMovement()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            if ((moveY - lastMoveInput.y < 0f || moveY <= 0f) && movementState == FPSMovementState.Sprinting)
            {
                SprintReleased();
            }
            
            _charAnimData.moveInput = new Vector2(moveX, moveY);
            
            if (movementState == FPSMovementState.Sprinting)
            {
                moveX = 0f;
            }

            _smoothMoveInput.x = CoreToolkitLib.Glerp(_smoothMoveInput.x, moveX, 5f);
            _smoothMoveInput.y = CoreToolkitLib.Glerp(_smoothMoveInput.y, moveY, 4f);
            
            bool moving = Mathf.Abs(moveX) >= 0.4f || Mathf.Abs(moveY) >= 0.4f;

            animator.SetBool(Moving, moving);    
            animator.SetFloat(MoveX, _smoothMoveInput.x);
            animator.SetFloat(MoveY, _smoothMoveInput.y);
                
            Vector3 move = transform.right * _smoothMoveInput.x + transform.forward * _smoothMoveInput.y;
            controller.Move(move * speed * Time.deltaTime);

            lastMoveInput.x = moveX;
            lastMoveInput.y = moveY;
        }

        private void UpdateAnimValues()
        {
            coreAnimComponent.SetCharData(_charAnimData);
        }

        private void Update()
        {
            ProcessActionInput();
            ProcessLookInput();
            UpdateFiring();
            UpdateMovement();
            UpdateAnimValues();
        }

        private void UpdateCameraRotation()
        {
            Vector2 finalInput = new Vector2(shouldMove ? 0f : _playerInput.x, _playerInput.y);// + _freeLookInput;
            cameraHolder.rotation = transform.rotation * Quaternion.Euler(finalInput.y, finalInput.x, 0f);
            mainCamera.rotation = cameraHolder.rotation * Quaternion.Euler(_freeLookInput.y, _freeLookInput.x, 0f);
        }
        
        private void LateUpdate()
        {
            UpdateCameraRotation();
        }
    }
}