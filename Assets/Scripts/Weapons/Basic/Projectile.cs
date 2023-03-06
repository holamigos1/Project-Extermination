using System;
using UnityEngine;
using Weapons.Range.Base;

namespace Weapons.Basic
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public abstract class Projectile : MonoBehaviour
    {
        public event Action<Projectile ,Collision> ProjectileHit;
        public float Damage => _damage;
        public Rigidbody Rigidbody => _rigidbody;
        
        [SerializeField] 
        private Rigidbody _rigidbody;
        private float _damage;

        private void Reset() => 
            _rigidbody = GetComponent<Rigidbody>();
        
        public void Init(float damage) =>
            _damage = damage;
        
        protected virtual void OnCollisionEnter(Collision collision)
        {
            //TODO Спаун следа от пули
            Debug.Log(collision.GetMaterialType());
            ProjectileHit?.Invoke(this, collision);
        }
    }
}