using UnityEngine;
using UnityEngine.AI;

namespace NPCAI
{
   public class NPCLocomotion : MonoBehaviour
    {
        NavMeshAgent agent;
        Animator animator;
        float speed;

        
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            speed = 0;
        }

        void Update()
        {
            speed = Mathf.Clamp(agent.velocity.magnitude , 0.0f ,5.0f);
            animator.SetFloat("Speed", speed);
        }
    }
}