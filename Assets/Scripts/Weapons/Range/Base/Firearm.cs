using GameData.AnimationTags;
using GameData.Layers;
using UnityEngine;
using Weapons.Ammo;
using Weapons.Basic;

namespace Weapons.Range.Base
{
    public class Firearm : Weapon
    {
        [SerializeField] private float _shootForce;
        [SerializeField] private Projectile _ammoType;
        [SerializeField] private Transform _launchProjectilePoint;

        protected void Awake()
        {
            //TODO Вынести это в другой класс, тк его нужно вызвать единажды на сцене а не в каждой пушке
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(GameLayers.FIRST_PERSON_LAYER), 
                                         LayerMask.NameToLayer(GameLayers.PROJECTILE_LAYER));
        }

        public void LaunchProjectile()
        {
            Projectile projectileObj = Instantiate(_ammoType, _launchProjectilePoint.position, Quaternion.identity);
            projectileObj.ProjectileHit += OnHit;
            projectileObj.ProjectileTransform.forward = _launchProjectilePoint.forward;
            projectileObj.ProjectileRigidbody.AddForce(projectileObj.transform.forward * _shootForce);
        }

        private void OnHit(Projectile projectileObj, Collision collision)
        {
            projectileObj.ProjectileHit -= OnHit;

            if (collision.gameObject.TryGetComponent(out IWeaponVerifyer weaponVerify))
                weaponVerify.Verify(this, collision);
        }
        
        public override void PlayFireAction()
        {
            if(!IsEquipped) return;
            if(!IsReady) return;
            
            ItemAnimator.SetTrigger(AnimationParams.PERFORM_ATTACK);
        }
    }
}   