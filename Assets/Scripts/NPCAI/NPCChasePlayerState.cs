using UnityEngine;

namespace baponkar.npc.zombie
{
    public class NPCChasePlayerState : NPCState
    {
        float _timer;
        public NPCStateId GetId()
        {
            return NPCStateId.ChasePlayer;
        }

        void NPCState.Enter(NPC_Agent agent)
        {
            agent.NoticedEnemy(agent.TargetingSystem.Target.transform);
            agent.navMeshAgent.isStopped = false;
            agent.navMeshAgent.stoppingDistance = agent.Config.attackRadius;
        }

        void NPCState.Exit(NPC_Agent agent)
        {
            agent.navMeshAgent.stoppingDistance = 0.0f;
            agent.navMeshAgent.isStopped = false;
        }

        void NPCState.Update(NPC_Agent agent)
        {
           ChasePlayer(agent);
        }

        private static void ChasePlayer(NPC_Agent agent)
        {
            if(agent.TargetingSystem.HasTarget)
            {
                float distance = Vector3.Distance(agent.TargetingSystem.TargetPosition, agent.transform.position);
                
                if (distance > agent.Config.attackRadius)
                {
                    agent.animator.SetBool("isAttacking", false);//TODO Сделай через листы аниматора
                    //agent.navMeshAgent.speed = agent.config.chaseWalkingSpeed + agent.config.offsetChaseSpeed;
                    agent.animator.SetFloat("Speed", 5f);
                    agent.navMeshAgent.destination = agent.TargetingSystem.TargetPosition;
                }
                else
                    agent.StateMachine.ChangeState(NPCStateId.Attack);
            }
            else
            {
                agent.StateMachine.ChangeState(NPCStateId.Patrol);
                agent.CalmDown();
            }
        }
    }
}