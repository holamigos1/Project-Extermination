using Scripts.GameEnums;
using Scripts.TagHolders;
using UnityEngine;

namespace Scripts.GameEnums
{
    public enum WeaponMode
    {
        ModeOne,
        ModeTwo,
        ModeThree
    }
}

namespace Scripts.Handlers
{
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] public WeaponType weaponType;
        [SerializeField] public WeaponFireMode FireMode;
        [SerializeField] private Animator weaponAnimator;
        [SerializeField] private GameObject muzzleFlash;
        [SerializeField] private AudioSource shootSound, reloadSound;

        private void Awake()
        {
            weaponAnimator = GetComponent<Animator>();
        }

        public void Shoot()
        {
            Debug.Log("Shoot");
            weaponAnimator.SetTrigger(AnimationTags.SHOOT_TRIGGER);
        }

        public void HideAway()
        {
            weaponAnimator.SetTrigger(AnimationTags.HIDE_TRIGGER);
        }

        private void MuzzleFlash_Show(bool isActive)
        {
            if (muzzleFlash != null) muzzleFlash.SetActive(isActive);
        }

        public void PlaySound(AudioSource source)
        {
            if (source != null) source.Play();
        }

        #region AnimationEvent

        public void PlaySound_Reload()
        {
            PlaySound(reloadSound);
        }

        public void PlaySound_Shoot()
        {
            PlaySound(shootSound);
        }

        public void MuzzzleFlash_Show_On()
        {
            MuzzleFlash_Show(true);
        }

        public void MuzzzleFlash_Show_OFF()
        {
            MuzzleFlash_Show(false);
        }

        public void Turn_Off_This()
        {
            gameObject.SetActive(false);
        }

        #endregion
    }
}