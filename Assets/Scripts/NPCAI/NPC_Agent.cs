using UnityEngine;
using UnityEngine.AI;

namespace baponkar.npc.zombie
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ZombieHealth))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(NPC_TargetingSystem))]
    public class NPC_Agent : MonoBehaviour
    {
        public NPCStateMachine StateMachine => _agentStateMachine;
        public NPCAgentConfig Config => _agentConfig;
        public bool EnemySeen => _enemySeen;
        public NPC_TargetingSystem TargetingSystem => _targetingSystem;
        
        #region Variables //TODO Жирные поля убирай в структуру
        [HideInInspector] public Animator animator; //TODO Скрой нахуй
        [HideInInspector] public CapsuleCollider capsuleCollider;
        [HideInInspector] public NavMeshAgent navMeshAgent;
        [HideInInspector] public NPCVisonSensor visonSensor;
        [HideInInspector] public NPCSoundSensor soundSensor;
        [HideInInspector] public NPC_Call call;
        [HideInInspector] public ZombieHealth aiHealth;
        
        [SerializeField] private NPCStateId initialState;
        [SerializeField] private NPCStateId currentState;
        [SerializeField] private NPCAgentConfig _agentConfig;
        [Tooltip("Хитрая система путей движения бота через иерархию")]
        [SerializeField] private GameObject waypoints;
        [SerializeField] private bool _enemySeen = false;
        
        private NPC_TargetingSystem _targetingSystem;
        private NPCStateMachine _agentStateMachine;
        #endregion
        

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            visonSensor = GetComponentInChildren<NPCVisonSensor>();
            soundSensor = GetComponentInChildren<NPCSoundSensor>();
            call = GetComponentInChildren<NPC_Call>();
            aiHealth = GetComponent<ZombieHealth>();
            animator = GetComponentInChildren<Animator>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            _targetingSystem = GetComponent<NPC_TargetingSystem>();
            
            _agentStateMachine = new NPCStateMachine(this);
            _agentStateMachine.RegisterState(new NPCChasePlayerState());
            _agentStateMachine.RegisterState(new NPCDeathState());
            _agentStateMachine.RegisterState(new NPCIdleState());
            _agentStateMachine.RegisterState(new NPCPatrolState());
            _agentStateMachine.RegisterState(new NPCAttackState());
            _agentStateMachine.RegisterState(new NPCFleeState());
            _agentStateMachine.RegisterState(new NPCAlertState());
            _agentStateMachine.RegisterState(new NPCWaypointBasedPatrolState());

            _agentStateMachine.ChangeState(initialState);
        }


        void Update()
        {
            _agentStateMachine.Update();
            currentState = _agentStateMachine.currentState;
        }

        public void NoticedEnemy(Transform enemyObj)
        {
            _enemySeen = true;
        }
        
        public void CalmDown()
        {
            _enemySeen = false;
        }

        public void FaceTarget()
        {
            Vector3 direction = (TargetingSystem.TargetPosition - navMeshAgent.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.time * 720f);
        }

        public void FacePlayer()
        {
            Vector3 direction = (TargetingSystem.TargetPosition - navMeshAgent.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.time * 720f);
        }
        

        public bool FindThePlayerWithTargetingSystem()
        {
            //finding Player by using Targeting System
            if (TargetingSystem.HasTarget)
            {
                if (TargetingSystem.Target.CompareTag("Player"))
                {
                    _enemySeen = true;
                    return true;
                }
            }
            _enemySeen = false;
            return false;
        }
    }
}

