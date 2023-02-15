using System;
using System.Linq;
using Data.Weapons;
using Movement.SourseMovment;
using Objects.Base;
using Scripts.Weapons.OPS;
using UnityEngine;
using Weapons.Basic;

namespace Weapons.O.P.S_Gun
{
    public class OPS_Gun : RangeWeapon , IPickup
    {
        public GameObject thisObject => gameObject;
        public PickUpType PickUpType => _pickUpType;
        
        [SerializeField] private PickUpType _pickUpType;
        [SerializeField] private AudioSource ConnectionSound;
        [SerializeField] private float MaxDistance = 75;
        [SerializeField] private OPS_Display DisplayScript;
        [SerializeField] private GameObject BulletCharge;
        [SerializeField] private Transform BulletSpawnPoint;
        [SerializeField] private float ShootForce = 100;
        [SerializeField] private LayerMask ObstacleMask;
        private bool _isHaveVisableCharge;
        private Transform _magnetationPlayerPoint;
        private MagnitatonBehaviour _fpsCharController;
        private OPS_ChargeUIPointerPresenter _opsUIPointerScript;
        private LayerMask _otherLayerMask = LayerMask.GetMask();
        private Transform _placedVisableCharge;
        private Vector3 _rayEndPoint; //temp

        private readonly float _sphereChekerRadius = 5f;

        public void Awake()
        {
            //проятгиваем все переменные для пушки
            animator = GetComponent<Animator>();
            _magnetationPlayerPoint = GameObject.FindWithTag(Data.Tags.GameTags.MAGNET_POINT_TAG).transform;
            _opsUIPointerScript = FindObjectOfType<OPS_ChargeUIPointerPresenter>();
            _fpsCharController = FindObjectOfType<MagnitatonBehaviour>();

            //двигаем бит еденицы слоя OPS_CHARGES_LAYER к нужной позиции
            _otherLayerMask = 1 << LayerMask.NameToLayer(Data.Layers.GameLayers.OPS_CHARGES_LAYER);
            //инвертируем все биты чтобы бит слоя OPS_CHARGES_LAYER был 0 а остальные 1
            _otherLayerMask = ~_otherLayerMask;
        }

        /*private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            float distance = Vector3.Distance(transform.position, rayEndPoint);
            float sphereChekerRadiusDinamic = SphereChekerRadius * (distance / MaxDistance);
            Gizmos.DrawSphere(rayEndPoint, sphereChekerRadiusDinamic);
        }*/
        /* Момент с LeftMouseDown += () => animator.SetTrigger(AnimationTags.SHOOT_TRIGGER); 
         * может показаться неочевидным поэтому заведому скажу что метод выстрела вызвается
         * ИЗ АНИМАЦИИ выстрела пушки... Если этой анимации нету то выстрела соответсвенно тоже не будет
         * (ну и при условии что в анимации стоит триггер вызова этого самого метода Shoot() ) 
         */
        public void Start()
        {
            //череда подписок на нажатия кнопок мыши и клавиш (по названию события понятно что было нажато)
            LeftMouseDown += () => animator.SetTrigger(Data.AnimationTags.AnimationTags.SHOOT_TRIGGER);
            RightMouseDown += Detect_PlacedCharge;
            ReloadButtonDown += () => animator.SetTrigger(Data.AnimationTags.AnimationTags.RELOAD_TRIGGER);
            
            SwitchModeButtonDown += () => animator.SetTrigger(Data.AnimationTags.AnimationTags.SWITCH_MODE_TRIGGER);
            
            UpdateAction +=
                () => //костыль, тк метод Update орпеделён в наследуемом классе и тут его переопределить нельзя
                {
                    if (_placedVisableCharge !=
                        null) // метод который на HUD рисует картинку "приконектиться" если игрок навёлся а заряд
                        _opsUIPointerScript.DrawPointer(_placedVisableCharge.transform.position, _isHaveVisableCharge);
                };

            // поставть пушку в изночальный режим 
            DisplayScript.SetCharge((Scripts.GameEnums.OPS_Charge)WeaponMode);
        }

        public void FixedUpdate()
        {
            //метод проверки на расположение заряд и можно ли к ним приконектиться
            CheckFor_PlacedCharge();
        }

        public override void Shoot()
        {
            //спавн снаряда
            var chargeObj = Instantiate(BulletCharge, BulletSpawnPoint);
            
            chargeObj.GetComponent<OPS_Charge>().Setup((Scripts.GameEnums.OPS_Charge)WeaponMode);

            //Делаем заряд независимым от пушки иначе заряд будет двигаться вместе с пушкой
            chargeObj.transform.parent = null;
            
            chargeObj.transform.eulerAngles = new Vector3(
                chargeObj.transform.eulerAngles.x,
                chargeObj.transform.eulerAngles.y, 0);

            //выплёвываем снаряд из ствола вперёд с множителем силы импульса
            chargeObj.GetComponent<Rigidbody>().AddForce(chargeObj.transform.forward * ShootForce, ForceMode.Impulse);
        }

        /// <summary>
        ///     метод переключения режима оружия
        /// </summary>
        private void SwitchOPSmode()
        {
            //две строчки ротации режима пушки путём инкременции 
            if ((int)WeaponMode == Enum.GetValues(typeof(WeaponMode)).Cast<int>().Max()) SwitchMode(WeaponMode.ModeOne);
            else SwitchMode(WeaponMode + 1);

            //отображение нынешнего режима пушки интопретированой к перечеслению OPScharge
            DisplayScript.SetCharge((Scripts.GameEnums.OPS_Charge)WeaponMode);
        }

        /// <summary>
        ///     метод, который рисует сферу радуосом переммнной SphereChekerRadius,
        ///     находит в этой сфере все заряды на слое OPS_CHARGES_LAYER,
        ///     из центра сферы проводит к ним луч и возврощает ближайший заряд к центру сферы
        /// </summary>
        private void CheckFor_PlacedCharge()
        {
            RaycastHit sphereHit;
            //ебани луч из центра главной камеры в мир
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            /* если луч касется чего либо в диапозоне MaxDistancе то возвращает Vector3
             места касания + векотр нормали от точки касания чтобы этот Vector3 был немного приподнят */
            if (Physics.Raycast(ray, out sphereHit, MaxDistance)) _rayEndPoint = sphereHit.point + sphereHit.normal / 30;
            //если луч нифига не коснулся в диапозоне то пусть вернт крайнюю точку диапозона
            else _rayEndPoint = ray.GetPoint(MaxDistance);

            //дистанция от пушки до этой самой точки
            var distance = Vector3.Distance(transform.position, _rayEndPoint);
            //расчёт динамического изменения радуса сферы проверки в зависиости от того
            //как далеко находится пушка и точка луча (потом будет понятно нахера)
            var sphereChekerRadiusDinamic = _sphereChekerRadius * (distance / MaxDistance);

            //ресуем эту самую сфреу с динамическим радуиусом sphereChekerRadiusDinamic
            //и возвращаем все коллайдеры на слове OPS_CHARGES_LAYER
            var chragesInSphere = Physics.OverlapSphere(
                _rayEndPoint, sphereChekerRadiusDinamic, LayerMask.GetMask(Data.Layers.GameLayers.OPS_CHARGES_LAYER));

            //если коолайдеров таких нет в диапозоне сферы то мы нифига не видим и выходим с этого метода
            if (chragesInSphere is null || chragesInSphere.Length == 0)
            {
                _isHaveVisableCharge = false;
                return;
            }

            //временные переменные для удобства
            var minDistance = sphereChekerRadiusDinamic;
            var closestChargeCollider = new Collider();
            var isNonObstacleRay = false;

            //если же коллайдеры в сфере есть то foreachЕМ проверяем самый ближайший видимый коллайдер к центру сферы
            foreach (var collider in chragesInSphere)
            {
                //Debug.DrawLine(rayEndPoint, collider.transform.position, Color.red);

                //определяем направление луча от ценра сферы к коллайдеру
                var diretionToTarget = (collider.transform.position - _rayEndPoint).normalized;

                //если луч нашёл припятсвие в виде обекта другого слоя по напрвленю к сфере
                //то проверяем следующий коллайдер в сфере (если же они есть)
                if (Physics.Raycast(_rayEndPoint, diretionToTarget,
                        out sphereHit, Vector3.Distance(collider.transform.position, _rayEndPoint), _otherLayerMask))
                    continue;
                //иначе же мы видим коолайдер
                isNonObstacleRay = true;

                //получаем растояния от заряда до сферы
                var distanse = Vector3.Distance(collider.transform.position, _rayEndPoint);

                //и определяем самый ближайший к центру сферы в масиве зарядов
                if (distanse < minDistance)
                {
                    minDistance = distanse;
                    closestChargeCollider = collider;
                }
            }

            _isHaveVisableCharge = isNonObstacleRay; //если луч нашёл припятсвие то мы нихера не видим
            //инче же выводим ближайший заряд closestChargeCollider
            if (_isHaveVisableCharge && closestChargeCollider != null)
                _placedVisableCharge = closestChargeCollider.transform;
        }

        /// <summary>
        ///     метод, при условии что игрок види заряд, говрит - "конектиться" или 2отконектиться2 от заряда
        /// </summary>
        private void Detect_PlacedCharge()
        {
            //если есть видимый заряд то примагниться
            if (_isHaveVisableCharge)
            {
                Magnetize_ToPlacedCharge(_placedVisableCharge);
            }
            else
            {
                //если нет то проверь, примагничен ли я уже
                //если да то отмагниться
                if (_fpsCharController.IsMagnetized) _fpsCharController.UnMagnetize();
                //если нет то покажи на пушке что "нету видимого заряда"
                else ShowDisplay_NoVisableCharge();
            }
        }

        /// <summary>
        ///     костыльный метод, говорит контроллеру на игроке выубить сурсовкое
        ///     движение и вызвать таам метод "примагнититься", а вот как оно магнититься надо смотреть уже там.
        /// </summary>
        /// <param name="chargePosition"></param>
        private void Magnetize_ToPlacedCharge(Transform chargePosition)
        {
            _fpsCharController.enabled = true;
            _fpsCharController.Magnetize_ToPointByBack(chargePosition);
        }

        //регион с открытыми методами для вызова их из анимций пушки

        #region Animator Region

        //открытые методы которые иницализирутся с евентов анимации пушки 
        public void ShowDisplay_TurningOff()
        {
            //вывести на экран пушки в текстовое поле состояния конкретного текста...
            DisplayScript.SetStatus("Shutting down..");
        }

        public void ShowDisplay_TurningOn()
        {
            DisplayScript.SetStatus("Start up..");
        }

        public void ShowDisplay_NoVisableCharge()
        {
            DisplayScript.SetStatus("There is no detected charges..");
        }

        public void ShowDisplay_WaitingForInput()
        {
            DisplayScript.SetStatus("Waiting for input..");
        }

        public void ShowDisplay_Reloading()
        {
            DisplayScript.SetStatus("Reloading..");
        }

        public void ShowDisplay_SwitchingMode()
        {
            DisplayScript.SetStatus("Switching Charge..");
        }

        public void ShowDisplay_Fire()
        {
            DisplayScript.SetStatus("OOPs..");
        }

        public void ShowDisplay_Connecting()
        {
            DisplayScript.SetStatus("Connecting to quantum point..");
        }

        public void PlaySound_Connection()
        {
            if (ConnectionSound != null) ConnectionSound.Play();
        }

        #endregion


        public GameObject Pickup()
        {
            throw new NotImplementedException();
        }
    }
}