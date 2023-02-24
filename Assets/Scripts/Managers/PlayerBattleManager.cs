using Scripts.Managers;
using UnityEngine;

public class PlayerBattleManager : MonoBehaviour
{
    public float fireRate = 15;
    public float damage = 20;
    private bool isAbleToShoot = true;
    private float nextTimeToFire;
    private WeaponManager weaponManager;

    private void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)) WeaponShoot();
    }

    private void WeaponShoot()
    {
        /*if (weaponManager.CurrentPickedWeapon.FireMode == WeaponFireMode.auto)
        {
            
        }

        if (weaponManager.CurrentPickedWeapon.FireMode == WeaponFireMode.single)
        {
            if(!isAbleToShoot) 
                if (Input.GetMouseButtonUp(0)) isAbleToShoot = true;
                else return;
      

            if (weaponManager.CurrentPickedWeapon.gameObject.tag == UnityTags.MELLE_WEAPON_TAG)
            {
                weaponManager.CurrentPickedWeapon.Shoot();
            }

            if (weaponManager.CurrentPickedWeapon.gameObject.tag == UnityTags.RANGE_WEAPON_TAG)
            {
                Debug.Log("SUKA");
                weaponManager.CurrentPickedWeapon.Shoot();
                //спауним пулю
            }
            isAbleToShoot = false;
        }*/
    }
}