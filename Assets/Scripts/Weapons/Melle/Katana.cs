using System.Collections.Generic;
using System.Collections;
using GameData.AnimationTags;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons.Basic;

namespace Weapons.Melle
{
    public class Katana : Weapon
    {
        private List<GameObject> _hitedRefs = new List<GameObject>();
        [ShowInInspector] private bool _isAbleToDamage;

        private void OnCollisionEnter(Collision collision)
        {
            if(_isAbleToDamage == false) return;
            if(_hitedRefs.Contains(collision.gameObject)) return;
            if(collision.gameObject == Owner.gameObject) return;
            
            if (collision.gameObject.TryGetComponent(out IWeaponVerifyer weaponVerifyer))
            {
                Debug.Log("Ударил " + collision.gameObject.name);
                
                _hitedRefs.Add(collision.gameObject);
                weaponVerifyer.Verify(this, collision);
            }
        }
            
        
        public override void PlayFireAction()
        {
            if(IsEquipped == false) return;
            if(IsReady == false) return;
            
            var attackAnimationID = Random.Range((int)1, (int)3+1);//TODO Убери магические числа
            ItemAnimator.SetInteger(AnimationParams.ATTACK_ID, attackAnimationID);
            ItemAnimator.SetTrigger(AnimationParams.PERFORM_ATTACK);
            StartCoroutine(OnFireActionCoroutine());
        }

        private IEnumerator OnFireActionCoroutine()
        {
            while (IsReady == false)
                yield return null; 
            
            _hitedRefs.Clear();
        }

        public void SetAbleToDamage(string isAble) =>
            _isAbleToDamage = bool.Parse(isAble);
        
    }
}
