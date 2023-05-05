using UnityEngine;
using Weapons.Basic;

namespace Weapons.DamageTypes
{
    public record BulletHit : IDamageType
    {
        public BulletHit(float damage, Vector3 hitVector)
        {
            Damage = damage;
            HitVector = hitVector;
        }
        
        public float Damage { get; }
        public Vector3 HitVector { get; }
    }
}