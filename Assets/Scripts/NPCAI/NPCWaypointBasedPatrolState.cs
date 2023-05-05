using UnityEngine;

namespace baponkar.npc.zombie
{
    public class NPCWaypointBasedPatrolState : NPCState
    {
        private Waypoint _currentWaypoint;
        private float _direction;

        public NPCStateId GetId()
        {
            return NPCStateId.WaypointPatrol;
        }
        
        void NPCState.Enter(NPC_Agent agent)
        {
            _currentWaypoint = agent.Config.waypoints.GetComponentInChildren<Waypoint>();
            _direction = Mathf.RoundToInt(Random.Range(0f,1f));
            agent.navMeshAgent.SetDestination(_currentWaypoint.GetPosition());
        }

        void NPCState.Exit(NPC_Agent agent)
        {

        }

        void NPCState.Update(NPC_Agent agent)
        {
            if(agent.aiHealth.isDead)
                agent.StateMachine.ChangeState(NPCStateId.Death);

            if(agent.TargetingSystem.HasTarget)
                agent.StateMachine.ChangeState(NPCStateId.ChasePlayer);

            if(agent.soundSensor.canHear || agent.call.getCall)
            {
                agent.FacePlayer();
                agent.StateMachine.ChangeState(
                    agent.TargetingSystem.HasTarget ? 
                        NPCStateId.ChasePlayer : 
                        NPCStateId.Alert);
            }
            
            if(agent.aiHealth.currentHealth < agent.aiHealth.maxHealth)
            {
                agent.FacePlayer();
                agent.StateMachine.ChangeState(
                    agent.TargetingSystem.HasTarget ? 
                        NPCStateId.ChasePlayer : 
                        NPCStateId.Alert);
            }

            WaypointPatrol(agent);
        }


        void WaypointPatrol(NPC_Agent agent)
        {
            if(agent.navMeshAgent.remainingDistance >= agent.navMeshAgent.stoppingDistance + 0.1f)
                return;
            
            bool shouldBranch = false;
            
            if(_currentWaypoint.branches != null && _currentWaypoint.branches.Count > 0)
                shouldBranch = Random.Range(0f, 1f) <= _currentWaypoint.branchProbability;

            if(shouldBranch)
                _currentWaypoint = _currentWaypoint.branches[Random.Range(0, _currentWaypoint.branches.Count - 1)];
            else
            {
                if(_direction == 0)
                {
                    if(_currentWaypoint.nextWaypoint != null)
                    {
                        _currentWaypoint = _currentWaypoint.nextWaypoint;
                    }
                    else
                    {
                        _currentWaypoint = _currentWaypoint.prevWaypoint;
                        _direction = 1;
                    }
                }

                if(_direction == 1)
                {
                    if(_currentWaypoint.prevWaypoint != null)
                    {
                        _currentWaypoint = _currentWaypoint.prevWaypoint;
                    }
                    else
                    {
                        _currentWaypoint = _currentWaypoint.nextWaypoint;
                        _direction = 0;
                    }
                }

                agent.navMeshAgent.SetDestination(_currentWaypoint.GetPosition());
            }
        }
    }
}