using GameItems.Base;
using UnityEngine;
using Weapons.Basic;
using Weapons.Melle;
using Weapons.Range.Base;

namespace Characters
{
    public class Scarecrow : Unit, IWeaponVerifyer
    {
        public void Verify(Firearm firearm, Collision collision) =>
            SystemsContainer.NotifySystems("Apply Damage", firearm.Damage);

        public void Verify(Katana katana, Collision collision) =>
            SystemsContainer.NotifySystems("Apply Damage", katana.Damage);
    }
}
