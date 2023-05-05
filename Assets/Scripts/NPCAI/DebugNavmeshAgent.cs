using UnityEngine;

namespace NPCAI
{
    [ExecuteInEditMode]
    public class DebugNavmeshAgent : MonoBehaviour
    {
        [Header("Green Line")]
        public bool velocity;
        [Header("Red Line")]
        public bool desiredVelocity;
        [Header("Black Line")]
        public bool path;

        [Header ("Blue Sphere")]
        public bool attack;
        [Header ("Yellow Sphere")]
        public bool chase;
        [Header ("Red Sphere")]
        public bool patrol;
        
        
        NPC_Agent agent;
        void Start()
        {
            agent = GetComponent<NPC_Agent>();
        }

        
        void OnDrawGizmos()
        {
            if(velocity){
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + agent.navMeshAgent.velocity);
            }
            if(desiredVelocity){
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + agent.navMeshAgent.desiredVelocity);
            }
            if(path){
                Gizmos.color = Color.black;
                var agentPath = agent.navMeshAgent.path;
                Vector3 prevCorner = transform.position;

                foreach(var corner in agentPath.corners){
                    Gizmos.DrawLine(prevCorner,corner);
                    Gizmos.DrawSphere(corner,0.1f);
                    prevCorner = corner;
                }
            }

            if(attack){
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, agent.Config.attackRadius);
            }
            if(chase){
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, agent.Config.chaseRadius);
            }

            if(patrol){
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, agent.Config.patrolRadius);
            }

        }
    }
}


