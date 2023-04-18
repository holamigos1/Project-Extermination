using System;
using Characters.Humanoid.Base;
using Characters.Humanoid.WeaponHandlers;
using GameAnimation.Sheets;
using GameObjects.Base;
using Misc;
using UnityEngine;
using Weapons;

namespace Characters.Humanoid
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class HumanoidBody : MonoBehaviour
    {
        //TODO ОЧЕНЬ ЖИРНОЕ ПОЛЕ
        public Vector3 RootPositionDelta => _rootPositionDelta;
        public Quaternion RootRotationDelta => _rootRotationDelta;
        public event Action<GameItem> ItemPickuped = delegate {  };

        public GameItem ItemInRightHand { get; private set; } = null;
        public GameItem ItemInLeftHand { get; private set; }= null;
        
        public HumanHead HeadController { get; private set; }

        [SerializeField] private HumanParametersSheet _humanAnimatorSheet;
        [SerializeField] private HumanRigsHandler.HumanRigsSettings _rigsSettings;
        [SerializeField, HideInInspector] private Transform _transform;
        [SerializeField, HideInInspector] private GameObject _gameObject;
        [SerializeField, HideInInspector] private Animator _animator;

        //TODO Контроллер глаз
        
        private HumanoidBodyParameters _humanoidBodyParameters;
        private WeaponsHandlerContainer _weaponHandler;
        private Vector3 _rootPositionDelta;
        private Quaternion _rootRotationDelta;
        private HumanRigsHandler _rigsHandler;

        #if UNITY_EDITOR
        private void Reset()
        {
            _animator = GetComponent<Animator>();
            _transform = transform;
            _gameObject = gameObject;   
            _humanAnimatorSheet = AssetDataBaseExtensions.LoadAssetAtFilter<HumanParametersSheet>
                ($"t:{nameof(HumanParametersSheet)}");
            
            if (_humanAnimatorSheet == null)
                Debug.LogError($"Создай список {nameof(HumanParametersSheet)} в проекте!");
        }
        #endif

        private void Awake()
        {
            if (_transform == null) Debug.LogError($"Reset {nameof(HumanoidBody)}!");
            if (_gameObject == null) Debug.LogError($"Reset {nameof(HumanoidBody)}!");
            if (_animator == null) Debug.LogError($"Reset {nameof(HumanoidBody)}!");
            
            HeadController = new HumanHead(_animator);
            _rigsHandler = new HumanRigsHandler(HeadController.HeadTransform, _rigsSettings);
            _humanoidBodyParameters = new HumanoidBodyParameters(_animator, _humanAnimatorSheet);
            _weaponHandler = new WeaponsHandlerContainer(_animator, _humanoidBodyParameters);
            _animator.stabilizeFeet = true;
        }

        private void Update()
        {
            _humanoidBodyParameters.FrameTick();
        }

        public void ApplyMovementDirection(Vector2 directionDelta)
        {
            _humanoidBodyParameters.ForwardDeltaParameter = directionDelta.y;
            _humanoidBodyParameters.SideDeltaParameter = directionDelta.x;
        }

        public void ApplyRotationDirection(Vector2 rotationDelta)
        {
            rotationDelta.x *= 2; //TODO Магический костыль!
            _humanoidBodyParameters.RotationDeltaParameter = rotationDelta.x; 
            _rootRotationDelta = Quaternion.AngleAxis(rotationDelta.x, Vector3.up);
        }

        public void ApplySprint(bool isSprinting)
        {
            if (isSprinting) _humanoidBodyParameters.CurrentMovementType = HumanoidBodyParameters.MovementType.Sprinting;
            else _humanoidBodyParameters.CurrentMovementType = HumanoidBodyParameters.MovementType.Walking;
        }
        
        public void ApplyJump()
        {
            
        }
        
        public void ApplyAim(bool isAiming)
        {
            if (ItemInRightHand != null &&
                _weaponHandler.IsMatchToCurrentEquipedWeapon(ItemInRightHand) == false) return;
            
            _weaponHandler.CurrentWeaponHandler.AimWeapon(isAiming);
        }
        
        public void ApplyAttack()
        {
            if (ItemInRightHand != null &&
                _weaponHandler.IsMatchToCurrentEquipedWeapon(ItemInRightHand) == false) return;
            
            _weaponHandler.CurrentWeaponHandler.AttackWeapon();
        }
        
        public void ApplyReload()
        {
            if (ItemInRightHand != null &&
                _weaponHandler.IsMatchToCurrentEquipedWeapon(ItemInRightHand) == false) return;
            
            _weaponHandler.CurrentWeaponHandler.ReloadWeapon();
        }
        
        public void ApplyHideItem()
        {
            if ((ItemInRightHand != null) && 
                (_weaponHandler.IsMatchToCurrentEquipedWeapon(ItemInRightHand))) 
                _weaponHandler.CurrentWeaponHandler.HideWeapon();
            
            //TODO Код прятанья предметов типа карты, компаса, хилок..
        }
        
        public void ApplyEquipItem()
        {
            if (ItemInRightHand == null) return;
            if (ItemInRightHand is Weapon weapon)
                _weaponHandler.TakeWeaponHandler(weapon).EquipWeapon();
            
            //TODO Код подбора предметов типа карты, компаса, хилок..
        }
        
        public void ApplyThrowItem()
        {
            
        }

        public void ApplyPickUp(GameItem item) //TODO груда ифов
        {
            if (item == null) return;
            if (_rigsHandler.ItemInRightHand == null) {
                StartCoroutine(_rigsHandler.PickUpItem(item, ApplyPickUp));
                return;
            }
            if (_rigsHandler.ItemInRightHand != null) ItemInRightHand = _rigsHandler.ItemInRightHand;
            if (ItemInRightHand == null) return;
            
            ApplyEquipItem();
        }
        
        private void OnAnimatorMove()
        {
            _rootPositionDelta += _animator.deltaPosition;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            
        }

        private void FixedUpdate() //TODO Не забудь поменять Execution Order в пользу контроллера персонажа а не его тела
        {
            _animator.stabilizeFeet =
                _rootRotationDelta == Quaternion.identity;

            _rootPositionDelta = Vector3.zero;
            _rootRotationDelta = Quaternion.identity;
        }
    }
}
