// Designed by Kinemation, 2023

using System;
using System.Collections.Generic;
using Plugins.Kinemation.FPSFramework.Runtime.Core.Data;
using Plugins.Kinemation.FPSFramework.Runtime.Core.States;
using Plugins.Kinemation.FPSFramework.Runtime.Layers;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core
{
	// An example-controller class
	[RequireComponent(typeof(CharacterController), typeof(Animator), typeof(CoreAnimComponent))]
	[RequireComponent(typeof(LookLayer), typeof(AdsLayer), typeof(BlendingLayer))]
	[RequireComponent(typeof(SwayLayer), typeof(DemoLoco), typeof(SlotLayer))]
	public class FPSController : MonoBehaviour
	{
		private static readonly int s_Sprint  = Animator.StringToHash("sprint");
		private static readonly int s_Crouch1 = Animator.StringToHash("crouch");
		private static readonly int s_Moving  = Animator.StringToHash("moving");
		private static readonly int s_MoveX   = Animator.StringToHash("moveX");
		private static readonly int s_MoveY   = Animator.StringToHash("moveY");

		[Title("Вращение")]
		[FormerlySerializedAs("shouldMove")]
		[LabelText("Может вращать торсом")]
		[ToggleLeft]
		[SerializeField] private bool _isShouldTorsoRotate;
		
		[FormerlySerializedAs("sensitivity")]
		[LabelText("Чувствительность мыши")]
		[Tooltip("Если коэффициент 0 то ввод с мыши будет игнорироваться.")]
		[SuffixLabel("Коэффициент")]
		[ProgressBar(0, 3)]
		[SerializeField]
		private float _mouseSensitivity;

		[LabelText("Углы свободного осмотра")]
		[SuffixLabel("° Градусы")]
		[Wrap(0,81)]
		[Tooltip("При нажатии на X мышка перестаёт управлять телом и управляет только головой.\n Этим параметром можно задать пределы отклонения башки.")]
		[SerializeField] private Vector2  freeLookAngle;
		
		[Title("Процедурные анимации")]
		
		[FormerlySerializedAs("aimMotion")]
		[LabelText("Движение прицеливания пушки")]
		[SerializeField]
		private DynamicMotion _aimMotion;

		[FormerlySerializedAs("leanMotion")]
		[LabelText("Движение наклона торсом")]
		[SerializeField]
		private DynamicMotion _leanMotion;

		[Title("Камера")]
		
		[LabelText("Главная камера")]
		[SuffixLabel("Объект с кости головы")]
		[SerializeField] private Transform mainCamera;
		
		[LabelText("Контейнер с камерой")]
		[SuffixLabel("Объект с кости головы")]
		[Tooltip("Объект, одна из дочек которого является камера")]
		[SerializeField] private Transform cameraHolder;//TODO Это лишнее. Проще управлять объектом самой камеры или дрочить синемашинную камеру.

		[LabelText("Позиция камеры первого лица")]
		[Tooltip("Объект-пустышка на место которого встанет FPS камера.")]
		[SuffixLabel("Объект с кости головы")]
		[FormerlySerializedAs("fpCameraBone")]
		[SerializeField]
		private Transform FirstPersonCameraBone;

		[LabelText("Позиция камеры третьего лица")]
		[Tooltip("Объект-пустышка на место которого встанет TPS камера.")]
		[SuffixLabel("Объект с кости головы")]
		[FormerlySerializedAs("tpCameraBone")]
		[SerializeField] private Transform ThirdPersonCameraBone;

		[Title("Движение")]
		
		[LabelText("Высота в присяди")]
		[Tooltip("На сколько меньше станет высота капсулы Character Controller в момент приседания персонажа по отношению к положения стоя.")]
		[SuffixLabel("Коэффициент")]
		[ProgressBar(0,1)]
		[SerializeField] private float crouchHeight;

		[LabelText("Скорость ходьбы")]
		[SuffixLabel("Метров в секунду")]
		[ProgressBar(0,5)]
		[SerializeField] private float walkingSpeed = 1.65f;
		
		[LabelText("Скорость бега")]
		[SuffixLabel("Метров в секунду")]
		[ProgressBar(0,10)]
		[SerializeField] private float sprintSpeed  = 4f;
		
		[LabelText("Список экипированного оружия")]
		[HideLabel]
		[SerializeField] private List<Weapon>        weapons;

		private bool        _isAiming;
		private int         _bursts;
		private CameraState _cachedCameraState;

		private CameraState         _cameraState = CameraState.FirstPerson;
		private SpringCameraShake   _cameraShake;
		private CharAnimData        _charAnimData;
		private float               _fireTimer = -1f;
		private CharacterController _characterController;
		private CoreAnimComponent   _coreAnimComponent;
		private Animator            _animator;
		private bool                _isFreeLook;
		
		//слои ебаные
		private LookLayer     _lookLayer;
		private AdsLayer      _adsLayer;
		private BlendingLayer _blendLayer;
		private SwayLayer     _swayLayer;
		private DemoLoco      _locoLayer;
		private SlotLayer     _slotLayer;

		// Used for free-look
		private Vector2 _freeLookInput;
		private Vector2 _lastMoveInput;

		private float _lowerCapsuleOffset;

		private Vector2         _playerInput;
		private RecoilAnimation _recoilAnimation;
		private bool            _isReloading;
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

			_lowerCapsuleOffset = _characterController.center.y - _characterController.height / 2f;
			_speed = walkingSpeed;

			_cameraShake = mainCamera.gameObject.GetComponent<SpringCameraShake>();

			InitLayers();
			EquipWeapon();
		}

		private void Awake()
		{
			_characterController = GetComponent<CharacterController>();
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
			_coreAnimComponent = GetComponent<CoreAnimComponent>();
			_animator = GetComponent<Animator>();
			_recoilAnimation = GetComponent<RecoilAnimation>();
			
			_lookLayer = GetComponent<LookLayer>();
			_adsLayer = GetComponent<AdsLayer>();
			_blendLayer = GetComponent<BlendingLayer>();
			_locoLayer = GetComponent<DemoLoco>();
			_swayLayer = GetComponent<SwayLayer>();
			_slotLayer = GetComponent<SlotLayer>();
		}

		private void EquipWeapon()
		{
			Weapon gun = weapons[_weaponIndex];

			_bursts = gun.burstAmount;
			_recoilAnimation.Init(gun.recoilData, gun.fireRate, gun.fireMode);
			_coreAnimComponent.OnGunEquipped(gun.gunData);

			gun.gameObject.SetActive(true);
		}

		private void ChangeWeapon()
		{
			int newIndex = _weaponIndex;
			newIndex++;
			if (newIndex > weapons.Count - 1)
				newIndex = 0;

			weapons[_weaponIndex].gameObject.SetActive(false);
			//todo тут прописать переключение оружий
			_weaponIndex = newIndex;

			EquipWeapon();
		}

		public void ToggleAim()
		{
			_isAiming = !_isAiming;

			if (_isAiming)
			{
				_actionState = FPSActionState.Aiming;
				_adsLayer.SetAdsAlpha(1f);
				_swayLayer.SetFreeAimEnable(false);
				_slotLayer.PlayMotion(_aimMotion);
			}
			else
			{
				_actionState = FPSActionState.None;
				_adsLayer.SetAdsAlpha(0f);
				_adsLayer.SetPointAlpha(0f);
				_swayLayer.SetFreeAimEnable(true);
				_slotLayer.PlayMotion(_aimMotion);
			}

			_recoilAnimation.isAiming = _isAiming;
		}

		public void ChangeScope() =>
			_coreAnimComponent.OnSightChanged(CurrentWeapon.GetScope());

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

			_adsLayer.SetLayerAlpha(0f);
			_lookLayer.SetLayerAlpha(0.4f);
			_blendLayer.SetLayerAlpha(0f);
			_locoLayer.SetSprint(true);
			_locoLayer.SetReadyWeight(0f);

			_movementState = FPSMovementState.Sprinting;
			_actionState = FPSActionState.None;

			_recoilAnimation.Stop();

			_speed = sprintSpeed;
			_animator.SetBool(s_Sprint, true);
		}

		private void SprintReleased()
		{
			if (_poseState == FPSPoseState.Crouching)
				return;

			_adsLayer.SetLayerAlpha(1f);
			_lookLayer.SetLayerAlpha(1f);
			_blendLayer.SetLayerAlpha(1f);
			_locoLayer.SetSprint(false);

			_movementState = FPSMovementState.Walking;

			_speed = walkingSpeed;
			_animator.SetBool(s_Sprint, false);
		}

		private void Crouch()
		{
			float height = _characterController.height;
			height *= crouchHeight;
			_characterController.height = height;
			_characterController.center = new Vector3(0f, _lowerCapsuleOffset + height / 2f, 0f);
			_speed *= 0.7f;

			_lookLayer.SetPelvisWeight(0f);

			_poseState = FPSPoseState.Crouching;
			_animator.SetBool(s_Crouch1, true);
		}

		private void Uncrouch()
		{
			float height = _characterController.height;
			height /= crouchHeight;
			_characterController.height = height;
			_characterController.center = new Vector3(0f, _lowerCapsuleOffset + height / 2f, 0f);
			_speed /= 0.7f;

			_lookLayer.SetPelvisWeight(1f);

			_poseState = FPSPoseState.Standing;
			_animator.SetBool(s_Crouch1, false);
		}

		private void ProcessActionInput() //TODO Использовать NewInputSystem
		{
			_charAnimData.leanDirection = 0;

			if (Input.GetKeyDown(KeyCode.P))
			{
				if (_cameraState == CameraState.TacCamera)
					return;

				if (_cameraState == CameraState.FirstPerson)
				{
					_cameraState = CameraState.ThirdPerson;

					cameraHolder.parent = transform;

					cameraHolder.localPosition = new Vector3(0f, ThirdPersonCameraBone.localPosition.y, 0f);
					cameraHolder.localRotation = Quaternion.identity;

					mainCamera.position = ThirdPersonCameraBone.position;
					mainCamera.localRotation = Quaternion.identity;
				}
				else
				{
					_cameraState = CameraState.FirstPerson;
					cameraHolder.parent = FirstPersonCameraBone;

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
					_slotLayer.PlayMotion(_leanMotion);

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

				if (Input.GetKeyDown(KeyCode.B) && _isAiming)
				{
					if (_actionState == FPSActionState.PointAiming)
					{
						_adsLayer.SetPointAlpha(0f);
						_actionState = FPSActionState.Aiming;
					}
					else
					{
						_adsLayer.SetPointAlpha(1f);
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
					_locoLayer.SetReadyWeight(0f);
					_lookLayer.SetLayerAlpha(1f);
				}
				else
				{
					_actionState = FPSActionState.Ready;
					_locoLayer.SetReadyPose(ReadyPose.LowReady);
					_locoLayer.SetReadyWeight(1f);
					_lookLayer.SetLayerAlpha(.5f);
					OnFireReleased();
				}
			}
		}

		private void ProcessLookInput()
		{
			_isFreeLook = Input.GetKey(KeyCode.X); //TODO Использовать NewInputSystem

			float deltaMouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
			float deltaMouseY = -Input.GetAxis("Mouse Y") * _mouseSensitivity;

			if (_isFreeLook)
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

			if (_isShouldTorsoRotate && !_isFreeLook)
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

			_animator.SetBool(s_Moving, moving);
			_animator.SetFloat(s_MoveX, _smoothMoveInput.x);
			_animator.SetFloat(s_MoveY, _smoothMoveInput.y);

			Vector3 move = transform.right * _smoothMoveInput.x + transform.forward * _smoothMoveInput.y;
			_characterController.Move(move * _speed * Time.deltaTime);

			_lastMoveInput.x = moveX;
			_lastMoveInput.y = moveY;
		}

		private void UpdateAnimValues() =>
			_coreAnimComponent.SetCharData(_charAnimData);

		private void UpdateCameraRotation()
		{
			var finalInput = new Vector2(_isShouldTorsoRotate ?
											 0f :
											 _playerInput.x, _playerInput.y); // + _freeLookInput;
			cameraHolder.rotation = transform.rotation * Quaternion.Euler(finalInput.y, finalInput.x, 0f);
			mainCamera.rotation = cameraHolder.rotation * Quaternion.Euler(_freeLookInput.y, _freeLookInput.x, 0f);
		}
	}
}