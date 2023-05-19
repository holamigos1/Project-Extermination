using UnityEngine;
using UnityEngine.UI;

namespace Artificial_Intelligence
{
    public class NPCHealthBar : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;

        [HideInInspector] public ZombieHealth health;

        [HideInInspector] public Slider slider;
        
        void Start()
        {
            health = GetComponentInParent<ZombieHealth>();
            slider = GetComponentInChildren<Slider>();
        }
    

        void LateUpdate()
        {
            if(!health.isDead)
            {
                transform.position =Camera.main.WorldToScreenPoint(target.position + offset);
                slider.value = health.currentHealth / health.maxHealth;
            }
            else
            {
                slider.gameObject.SetActive(false);
            }
        }
    }
}
