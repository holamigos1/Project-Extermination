using UnityEngine;

namespace Artificial_Intelligence
{
    [ExecuteInEditMode]
    public class NPC_Call : MonoBehaviour
    {
        public Color color;
        public float callingDistance = 20f;
        public float offset = 5f;
        public bool getCall;

        private NPC_Agent[] _agents;
        private NPC_Agent _agent;
        
        void Start()
        {
            _agents = FindObjectsOfType<NPC_Agent>();
            _agent = GetComponent<NPC_Agent>();
        }
        
        void Update()
        {
            Call();
        }

        private void Call()
        {
            if (_agent.EnemySeen == false) return;
            
            foreach (NPC_Agent npcAgent in _agents)
            {
                float distance = Vector3.Distance(npcAgent.transform.position, transform.position);

                if ((distance < callingDistance) == false || npcAgent.aiHealth.isDead) 
                    continue;
                    
                if (npcAgent.transform == _agent.transform) 
                    continue;
                        
                var nearAgent = npcAgent.transform.GetComponent<NPC_Agent>(); //TODO GetComponent каждый фрейм
                var nearCalling = npcAgent.transform.GetComponent<NPC_Call>();
                    
                nearAgent.NoticedEnemy(npcAgent.TargetingSystem.TargetPosition);
                nearCalling.getCall = true;
                if(getCall == false) //TODO ??? 
                    nearAgent.StateMachine.ChangeState(NPCStateId.Alert);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, callingDistance);
        }
    }
}

