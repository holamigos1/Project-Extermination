namespace Artificial_Intelligence
{
    public class NPCIdleState : NPCState
    {
        public NPCStateId GetId() => NPCStateId.Idle;

        void NPCState.Enter(NPC_Agent agent)
        {
            agent.navMeshAgent.isStopped = true;
        }

        void NPCState.Update(NPC_Agent agent)
        {
            if(agent.aiHealth.isDead)
            {
                agent.StateMachine.ChangeState(NPCStateId.Death);
            }
            
            if(agent.TargetingSystem.HasTarget)
            {
                agent.StateMachine.ChangeState(NPCStateId.ChasePlayer);
            }

            if(agent.soundSensor.canHear || agent.call.getCall)
            {
                agent.FacePlayer();
                agent.StateMachine.ChangeState(agent.TargetingSystem.HasTarget
                    ? NPCStateId.ChasePlayer
                    : NPCStateId.Alert);
            }

            if(agent.aiHealth.currentHealth < agent.aiHealth.maxHealth)
            {
                agent.FacePlayer();
                agent.StateMachine.ChangeState(agent.TargetingSystem.HasTarget
                    ? NPCStateId.ChasePlayer
                    : NPCStateId.Alert);
            }
        }

        void NPCState.Exit(NPC_Agent agent)
        {
            agent.navMeshAgent.isStopped = false;
        }
    }
}