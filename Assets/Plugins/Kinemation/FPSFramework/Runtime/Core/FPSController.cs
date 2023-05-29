// Designed by Kinemation, 2023

using System.Collections.Generic;
using Plugins.Kinemation.FPSFramework.Runtime.Core.Data;
using Plugins.Kinemation.FPSFramework.Runtime.Core.States;
using Plugins.Kinemation.FPSFramework.Runtime.Layers;
using UnityEngine;
using Weapons;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core
{
	// An example-controller class
	public class FPSController : MonoBehaviour
	{
		private static readonly int s_Sprint  = Animator.StringToHash("sprint");
		private static readonly int s_Crouch1 = Animator.StringToHash("crouch");
		private static readonly int s_Moving  = Animator.StringToHash("moving");
		private static readonly int s_MoveX   = Animator.StringToHash("moveX");
		private static readonly int s_MoveY   = Animator.StringToHash("moveY");

		[Header("FPS Animator Core")]
		[SerializeField] private CoreAnimComponent coreAnimComponent;
		[SerializeField] private Animator animator;
		[SerializeField] private Vector2  freeLookAngle;

		[Header("FPS Animator Layers")]
		[SerializeField] private LookLayer     lookLayer;
		[SerializeField] private AdsLayer      adsLayer;
		[SerializeField] private BlendingLayer blendLayer;
		[SerializeField] private SwayLayer     swayLayer;
		[SerializeField] private DemoLoco      locoLayer;
		[SerializeField] private SlotLayer     slotLayer;

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

		[Header("Movement")]
		[SerializeField] private bool                shouldMove;
		[SerializeField] private float               walkingSpeed = 10f;
		[SerializeField] private float               sprintSpeed  = 25f;
		[SerializeField] private CharacterController controller;
		[SerializeField] private List<Weapon>        weapons;
		
		private bool        _aiming;
		private int         _bursts;
		private CameraState _cachedCameraState;

		private CameraState       _cameraState = CameraState.FirstPerson;
		private SpringCameraShake _cameraShake;
		private CharAnimData      _charAnimData;
		private float             _fireTimer = -1f;

		private bool _freeLook;

		// Used for free-look
		private Vector2 _freeLookInput;
		private Vector2 _lastMoveInput;

		private float _lowerCapsuleOffset;

		private Vector2         _playerInput;
		private RecoilAnimation _recoilAnimation;
		private bool            _reloading;
		private Vector2         _smoothMoveInput;
		private float           _speed;

		private int              _weaponIndex;
		private FPSActionState   _actionState;
		private FPSMovementState _movementState;
		private FPSPoseState     _poseState;

		private Weapon CurrentWeapon => weapons[_weaponIndex];

		private void Start()
		{
			Application.targetFrameRate = 120;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			_lowerCapsuleOffset = controller.center.y - controller.height / 2f;
			_speed = walkingSpeed;

			_cameraShake = mainCamera.gameObject.GetComponent<SpringCameraShake>();

			InitLayers();
			EquipWeapon();
		}

		private void Update()
		{
			ProcessActionInput();
			ProcessLookInput();
			UpdateFiring();
			UpdateMovement();
			UpdateAnimValues();
		}

		private void LateUpdate() =>
			UpdateCameraRotation();

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

		private void EquipWeapon()
		{
			Weapon gun = weapons[_weaponIndex];

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
				newIndex = 0;

			weapons[_weaponIndex].gameObject.SetActive(false);
			_weaponIndex = newIndex;

			EquipWeapon();
		}

		public void ToggleAim()
		{
			_aiming = !_aiming;

			if (_aiming)
			{
				_actionState = FPSActionState.Aiming;
				adsLayer.SetAdsAlpha(1f);
				swayLayer.SetFreeAimEnable(false);
				slotLayer.PlayMotion(aimMotion);
			}
			else
			{
				_actionState = FPSActionState.None;
				adsLayer.SetAdsAlpha(0f);
				adsLayer.SetPointAlpha(0f);
				swayLayer.SetFreeAimEnable(true);
				slotLayer.PlayMotion(aimMotion);
			}

			_recoilAnimation.isAiming = _aiming;
		}

		public void ChangeScope() =>
			coreAnimComponent.OnSightChanged(CurrentWeapon.GetScope());

		private void Fire()
		{
			_cameraShake.PlayCameraShake();
			CurrentWeapon.OnFire();
			_recoilAnimation.Play();
		}

		private void OnFirePressed()
		{
			Fire();
			_bursts = CurrentWeapon.burstAmount - 1;
			_fireTimer = 0f;
		}


		private void OnFireReleased()
		{
			_recoilAnimation.Stop();
			_fireTimer = -1f;
		}

		private void SprintPressed()
		{
			if (_poseState == FPSPoseState.Crouching)
				return;

			adsLayer.SetLayerAlpha(0f);
			lookLayer.SetLayerAlpha(0.4f);
			blendLayer.SetLayerAlpha(0f);
			locoLayer.SetSprint(true);
			locoLayer.SetReadyWeight(0f);

			_movementState = FPSMovementState.Sprinting;
			_actionState = FPSActionState.None;

			_recoilAnimation.Stop();

			_speed = sprintSpeed;
			animator.SetBool(s_Sprint, true);
		}

		private void SprintReleased()
		{
			if (_poseState == FPSPoseState.Crouching)
				return;

			adsLayer.SetLayerAlpha(1f);
			lookLayer.SetLayerAlpha(1f);
			blendLayer.SetLayerAlpha(1f);
			locoLayer.SetSprint(false);

			_movementState = FPSMovementState.Walking;

			_speed = walkingSpeed;
			animator.SetBool(s_Sprint, false);
		}

		private void Crouch()
		{
			float height = controller.height;
			height *= crouchHeight;
			controller.height = height;
			controller.center = new Vector3(0f, _lowerCapsuleOffset + height / 2f, 0f);
			_speed *= 0.7f;

			lookLayer.SetPelvisWeight(0f);

			_poseState = FPSPoseState.Crouching;
			animator.SetBool(s_Crouch1, true);
		}

		private void Uncrouch()
		{
			float height = controller.height;
			height /= crouchHeight;
			controller.height = height;
			controller.center = new Vector3(0f, _lowerCapsuleOffset + height / 2f, 0f);
			_speed /= 0.7f;

			lookLayer.SetPelvisWeight(1f);

			_poseState = FPSPoseState.Standing;
			animator.SetBool(s_Crouch1, false);
		}

		private void ProcessActionInput() //TODO Использовать NewInputSystem
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit(0);

			_charAnimData.leanDirection = 0;

			if (Input.GetKeyDown(KeyCode.P))
			{
				if (_cameraState == CameraState.TacCamera)
					return;

				if (_cameraState == CameraState.FirstPerson)
				{
					_cameraState = CameraState.ThirdPerson;

					cameraHolder.parent = transform;

					cameraHolder.localPosition = new Vector3(0f, tpCameraBone.localPosition.y, 0f);
					cameraHolder.localRotation = Quaternion.identity;

					mainCamera.position = tpCameraBone.position;
					mainCamera.localRotation = Quaternion.identity;
				}
				else
				{
					_cameraState = CameraState.FirstPerson;
					cameraHolder.parent = fpCameraBone;

					cameraHolder.localPosition = Vector3.zero;
					cameraHolder.localRotation = Quaternion.identity;

					mainCamera.localPosition = Vector3.zero;
					mainCamera.localRotation = Quaternion.identity;
				}
			}

			if (Input.GetKeyDown(KeyCode.LeftShift))
				SprintPressed();

			if (Input.GetKeyUp(KeyCode.LeftShift))
				SprintReleased();

			if (Input.GetKeyDown(KeyCode.F))
				ChangeWeapon();

			if (_movementState == FPSMovementState.Sprinting)
				return;

			if (_actionState != FPSActionState.Ready)
			{
				if (Input.GetKeyDown(KeyCode.Q)
					|| Input.GetKeyUp(KeyCode.Q)
					|| Input.GetKeyDown(KeyCode.E)
					|| Input.GetKeyUp(KeyCode.E))
					slotLayer.PlayMotion(leanMotion);

				if (Input.GetKey(KeyCode.Q))
					_charAnimData.leanDirection = 1;
				else if (Input.GetKey(KeyCode.E))
					_charAnimData.leanDirection = -1;

				if (Input.GetKeyDown(KeyCode.Mouse0))
					OnFirePressed();

				if (Input.GetKeyUp(KeyCode.Mouse0))
					OnFireReleased();

				if (Input.GetKeyDown(KeyCode.Mouse1))
					ToggleAim();

				if (Input.GetKeyDown(KeyCode.V))
					ChangeScope();

				if (Input.GetKeyDown(KeyCode.B) && _aiming)
				{
					if (_actionState == FPSActionState.PointAiming)
					{
						adsLayer.SetPointAlpha(0f);
						_actionState = FPSActionState.Aiming;
					}
					else
					{
						adsLayer.SetPointAlpha(1f);
						_actionState = FPSActionState.PointAiming;
					}
				}
			}

			if (Input.GetKeyDown(KeyCode.C))
			{
				if (_poseState == FPSPoseState.Standing)
					Crouch();
				else
					Uncrouch();
			}

			if (Input.GetKeyDown(KeyCode.H))
			{
				if (_actionState == FPSActionState.Ready)
				{
					_actionState = FPSActionState.None;
					locoLayer.SetReadyWeight(0f);
					lookLayer.SetLayerAlpha(1f);
				}
				else
				{
					_actionState = FPSActionState.Ready;
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
				transform.Rotate(Vector3.up * deltaMouseX);

			_charAnimData.AddAimInput(new Vector2(deltaMouseX, deltaMouseY));
		}

		private void UpdateFiring()
		{
			if (_recoilAnimation.fireMode != FireMode.Semi && _fireTimer >= 60f / CurrentWeapon.fireRate)
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
				_fireTimer += Time.deltaTime;
		}

		private void UpdateMovement()
		{
			float moveX = Input.GetAxis("Horizontal");
			float moveY = Input.GetAxis("Vertical");

			if ((moveY - _lastMoveInput.y < 0f || moveY <= 0f) && _movementState == FPSMovementState.Sprinting)
				SprintReleased();

			_charAnimData.moveInput = new Vector2(moveX, moveY);

			if (_movementState == FPSMovementState.Sprinting)
				moveX = 0f;

			_smoothMoveInput.x = CoreToolkitLib.Glerp(_smoothMoveInput.x, moveX, 5f);
			_smoothMoveInput.y = CoreToolkitLib.Glerp(_smoothMoveInput.y, moveY, 4f);

			bool moving = Mathf.Abs(moveX) >= 0.4f || Mathf.Abs(moveY) >= 0.4f;

			animator.SetBool(s_Moving, moving);
			animator.SetFloat(s_MoveX, _smoothMoveInput.x);
			animator.SetFloat(s_MoveY, _smoothMoveInput.y);

			Vector3 move = transform.right * _smoothMoveInput.x + transform.forward * _smoothMoveInput.y;
			controller.Move(move * _speed * Time.deltaTime);

			_lastMoveInput.x = moveX;
			_lastMoveInput.y = moveY;
		}

		private void UpdateAnimValues() =>
			coreAnimComponent.SetCharData(_charAnimData);

		private void UpdateCameraRotation()
		{
			var finalInput = new Vector2(shouldMove ?
											 0f :
											 _playerInput.x, _playerInput.y); // + _freeLookInput;
			cameraHolder.rotation = transform.rotation * Quaternion.Euler(finalInput.y, finalInput.x, 0f);
			mainCamera.rotation = cameraHolder.rotation * Quaternion.Euler(_freeLookInput.y, _freeLookInput.x, 0f);
		}
	}
}