using UnityEngine;

namespace NPCAI
{
    public class NPCAttackState : NPCState
    {
        private float _timer;
        private float _attackTime;
        private Vector3 _offset;

        private RaycastHit _hit;
        private RaycastHit _hitL;
        private RaycastHit _hitR;

        private Vector3 _direction;
        private Vector3 _directionL;
        private Vector3 _directionR;

        public NPCStateId GetId()
        {
            return NPCStateId.Attack;
        }
        
        void NPCState.Enter(NPC_Agent agent)
        {
            _attackTime = agent.Config.attackTime;
            agent.navMeshAgent.isStopped = true;
            _offset = new Vector3(Random.Range(-1f,1f),0f,Random.Range(-1f,1f));
            //playerHealth = agent.playerTransform.GetComponent<Health>();
            agent.animator.SetBool("isAttacking", true); //TODO Используй animator листы
            agent.FacePlayer();
        }

        void NPCState.Update(NPC_Agent agent)
        {
            _timer -= Time.deltaTime;

            if (agent.aiHealth.isDead)
                agent.StateMachine.ChangeState(NPCStateId.Death);
            
            if(agent.StateMachine.currentState == NPCStateId.Death)
                return;

            switch (agent.TargetingSystem.HasTarget)
            {
                case true when agent.TargetingSystem.TargetDistance <= agent.Config.attackRadius:
                {
                    if(_timer <= 0f )
                    {
                        Attack(agent);
                        _timer = _attackTime;
                    }

                    break;
                }
                
                case true when agent.TargetingSystem.TargetDistance > agent.Config.attackRadius:
                    agent.StateMachine.ChangeState(NPCStateId.ChasePlayer);
                    break;
                
                default:
                    agent.StateMachine.ChangeState(NPCStateId.Alert);
                    break;
            }
        }

        void NPCState.Exit(NPC_Agent agent)
        {
            agent.navMeshAgent.isStopped = false;
            agent.animator.SetBool("isAttacking", false);
        }

        private void Attack(NPC_Agent agent)
        {
            _direction = (agent.TargetingSystem.TargetPosition - agent.transform.position).normalized;
            bool isRayblock = Physics.Raycast(agent.transform.position, _direction, out _hit, agent.Config.attackRadius);

            if (isRayblock == false) return;
            
            if (_hit.collider.gameObject.CompareTag("Player")) //TODO нахуй extern
                Debug.Log("Attacking");
        }
    }
}
