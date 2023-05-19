using UnityEngine;
using Weapons.Basic;
using Weapons.DamageTypes;

namespace Artificial_Intelligence
{
    public class ZombieHealth : MonoBehaviour , IHittable
    {
        public float maxHealth = 100f;
        public float currentHealth;
        public bool isDead = false;
        public bool  isInjured = false;
        public float dieForce = 2f;
        NPCRagdol ragdol;
        HitBox hitBox;  
        Rigidbody [] rigidBody;
        
        public void Start()
        {
            ragdol = GetComponent<NPCRagdol>();
            rigidBody = GetComponentsInChildren<Rigidbody>();
            for(int i=0;i<rigidBody.Length;i++)
            {
                hitBox = rigidBody[i].gameObject.AddComponent<HitBox>();
                hitBox.health = this;
            }
            currentHealth = maxHealth;
        }

        public void Die(Vector3 direction)
        {
            ragdol.ActivateRagdol();
            ragdol.ApplyForce(direction * dieForce);
        }

        public void ApplyHit(BulletHit hit)
        {
            if(currentHealth > 0)
            {
                currentHealth -= hit.Damage;
            }
            if(currentHealth <= 30f && currentHealth > 0)
            {
                isInjured = true;
            }
            if(currentHealth <= 0 ){
                currentHealth = 0;
                isInjured = false;
                isDead = true;
                Die(hit.HitVector);
            }
        }
    }
}
