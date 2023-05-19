using UnityEngine;
using UnityEngine.AI;

namespace Artificial_Intelligence
{
    public class NPCPatrolState : NPCState
    {
        #region Variables

        private bool _walkPointSet;
        private Vector3 _tempTarget;
        private Vector3 _lastTempTarget;

        private float _timer;
        private float _maxTime;

        private NavMeshPath _navMeshPath;
        private Vector3 _initialPosition;
        private RandomPointOnNavMesh _randomPointOnNavMesh;

        #endregion
    
        public NPCStateId GetId()
        {
            return NPCStateId.Patrol;
        }

        void NPCState.Enter(NPC_Agent agent)
        {
            var position = agent.transform.position;
            _randomPointOnNavMesh = new RandomPointOnNavMesh(position);
            _navMeshPath = new NavMeshPath();
            agent.navMeshAgent.isStopped = false;

            agent.navMeshAgent.speed = agent.Config.patrolSpeed;
            agent.navMeshAgent.acceleration = agent.Config.patrolAcceleration;
            agent.navMeshAgent.angularSpeed  = agent.Config.patrolTurnSpeed;
            _maxTime = agent.Config.patrolWaitTime;
            _initialPosition = position;
        }

        void NPCState.Exit(NPC_Agent agent)
        {
            agent.navMeshAgent.isStopped = false;
        }


        void NPCState.Update(NPC_Agent agent)
        {
            _timer -= Time.deltaTime;

            if(agent.aiHealth.isDead == false)
            {
                if(agent.FindThePlayerWithTargetingSystem())
                    agent.StateMachine.ChangeState(NPCStateId.ChasePlayer);
                
                else Patrol(agent);
            }
            
            else
                agent.StateMachine.ChangeState(NPCStateId.Death);
            
            
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

        void SearchingPoint(NPC_Agent agent)
        {
            if(RandomPointOnNavMesh.RandomPoint(agent.transform.position, agent.Config.patrolRadius, out Vector3 result))
            {
                _tempTarget = result;
                _walkPointSet = true;
            }
            else
                _walkPointSet = false;
        }
        
        void FacePatrol(NPC_Agent agent)
        {   
            Vector3 direction = (_tempTarget- agent.navMeshAgent.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3 (direction.x,0,direction.z));
            agent.navMeshAgent.transform.rotation = Quaternion.Lerp(agent.navMeshAgent.transform.rotation, lookRotation,Time.time * agent.Config.patrolTurnSpeed);
        }
        
        
        void Patrol(NPC_Agent agent)
        {
            if(!_walkPointSet)
            {
                SearchingPoint(agent);
            }

            if(_walkPointSet && _timer < 0f)
            {
                FacePatrol(agent);
                agent.navMeshAgent.SetDestination(_tempTarget);
                _lastTempTarget = _tempTarget;
                _timer = _maxTime;
            }
        
            if(agent.navMeshAgent.remainingDistance <= 0.1f)
            {
                _walkPointSet = false;
            }
        }
    }
}

