using UnityEngine;

namespace Artificial_Intelligence
{
    public class NPCFleeState : NPCState
    {
        int multiplier = 1; // or more

        public NPCStateId GetId()
        {
            return NPCStateId.Flee;
        }

        void NPCState.Enter(NPC_Agent agent)
        {
            agent.navMeshAgent.isStopped = false;
        }

        void NPCState.Exit(NPC_Agent agent)
        {
            agent.navMeshAgent.isStopped = false;
        }

        void NPCState.Update(NPC_Agent agent)
        {
            Vector3 runTo = agent.transform.position + (agent.transform.position - agent.TargetingSystem.TargetPosition) * multiplier;
            float distance = Vector3.Distance(agent.transform.position, agent.TargetingSystem.TargetPosition);
            if (distance < agent.Config.fleeRange)
            {
                agent.navMeshAgent.SetDestination(runTo);
            }    
        }
    }
}
