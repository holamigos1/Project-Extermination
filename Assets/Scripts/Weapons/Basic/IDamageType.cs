using UnityEngine;

namespace Weapons.Basic
{
    public interface IDamageType
    {
        public float Damage { get; }
        public Vector3 HitVector { get; }
    }
}