using UnityEngine;
using Weapons.Melle;
using Weapons.Range.Base;

namespace Weapons.Basic
{
    public interface IWeaponVerifyer
    {
        public void Verify(Firearm firearm, Collision collision);
        public void Verify(Katana katana, Collision collision);
    }
}