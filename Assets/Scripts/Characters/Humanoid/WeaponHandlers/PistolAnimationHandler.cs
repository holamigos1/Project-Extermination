using Characters.Humanoid.Base;
using GameAnimation.Data;
using UnityEngine;

namespace Characters.Humanoid.WeaponHandlers
{
    public class PistolAnimationHandler : CharacterWeaponHandler
    {
        public PistolAnimationHandler(Animator animator, HumanoidBodyParameters bodyParameters)  //TODO Жирная инициализация
            : base(animator, bodyParameters, WeaponType.Pistol) 
        {

        }
        
    }
}