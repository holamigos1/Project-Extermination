using System.Collections;
using GameData.AnimationTags;
using GameData.Layers;
using GameData.Tags;
using GameObjects.Base;
using Misc;
using UnityEngine;

namespace Weapons
{
    public abstract class Weapon : GameItem, IEquip, IDrop
    {
        public Unit Owner => _owner;
        public bool IsEquipped => _isEquipped;
        public bool IsInHand => (ItemTransform.parent != null) && 
                                (ItemTransform.parent.CompareTag(GameTags.HAND_TAG));

        public bool IsReady => ItemAnimator.GetCurrentAnimatorStateInfo(0).IsName(AnimationParams.IDLE);
        public float Damage => _damage;
        public Transform RightHandGrip => _rightHandGrip;
        
        [SerializeField] 
        private float _damage = 10f;
        
        
        protected bool _isEquipped;
        protected Unit _owner;
        
        private void Start()
        {
            if (IsInHand) Equip();
        }
        
        public void Equip()
        {
            ItemGameObject.SetActive(true);
            ItemAnimator.enabled = true;
            ItemAnimator.SetTrigger(AnimationParams.ITEM_EQUIP_TRIGGER);
            ItemAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            
            ItemRigidbody.isKinematic = true;
            ItemRigidbody.useGravity = false;

            StartCoroutine(OnEquipCoroutine());
        }

        private IEnumerator OnEquipCoroutine()
        {
            while (IsReady == false) //да она дохрена раз обращается к GetCurrentAnimatorStateInfo, и чо
                yield return null;
            
            ItemAnimator.SetBool(AnimationParams.IS_ITEM_EQUIPPED, _isEquipped = true);
        }

        public void SetOwner(Unit newOwner) =>
            _owner = newOwner;

        public virtual void PlayFireAction() { }
        
        public void PlayAimAction() { }

        private void OnDrawGizmos()
        {
            //TODO Удали как будет не нужно
            Gizmos.color = Color.yellow;
            Bounds boudns = transform.RenderBounds();
            Gizmos.DrawWireCube(boudns.center, boudns.size);
        }
        
        public void Drop()
        {
            _isEquipped = false;
            _owner = null;
            
            ItemRigidbody.isKinematic = false;
            ItemRigidbody.useGravity = true;
            
            ItemAnimator.StopPlayback();
            ItemAnimator.cullingMode = AnimatorCullingMode.CullCompletely;
            ItemAnimator.SetBool(AnimationParams.IS_ITEM_EQUIPPED, false);

            ItemGameObject.ChangeGameObjsLayers(GameLayers.DEFAULT_LAYER);
        }
    }
}