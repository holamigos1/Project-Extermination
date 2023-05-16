namespace Artificial_Intelligence
{
    public class NPCDeathState : NPCState
    {
        
        public NPCStateId GetId()
        {
            return NPCStateId.Death; 
        }

        void NPCState.Enter(NPC_Agent agent)
        {
            if(agent.navMeshAgent != null)
            {
                agent.navMeshAgent.isStopped = true;
            }
            agent.animator.SetTrigger("death");
            agent.aiHealth.isDead = true;
        }
        
        void NPCState.Exit(NPC_Agent agent)
        {
        
        }

    

        void NPCState.Update(NPC_Agent agent)
        {

        }


    }
}

