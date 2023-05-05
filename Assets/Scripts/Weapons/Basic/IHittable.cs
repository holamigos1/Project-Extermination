using Weapons.DamageTypes;

namespace Weapons.Basic
{
    public interface IHittable
    {
        public void ApplyHit(BulletHit hit);
    }
}