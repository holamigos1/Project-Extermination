using UnityEngine.VFX;

namespace Weapons
{
    public class MuzzleFlashVFXAttributeContainer
    {
        public MuzzleFlashVFXAttributeContainer(VisualEffect effectComponent)
        {
            _visualEffectComponent = effectComponent;
        }

        private VisualEffect _visualEffectComponent;

       
    }
}