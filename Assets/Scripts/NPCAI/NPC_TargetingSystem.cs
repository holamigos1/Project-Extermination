using UnityEngine;

namespace baponkar.npc.zombie
{
    [ExecuteInEditMode]
    public class NPC_TargetingSystem : MonoBehaviour
    {
        //TODO Много магии
        public float memorySpan = 3.0f;
        public float distanceWeight = 1.0f;
        public float angleWeight = 1.0f;
        public float ageWeight = 1.0f;

        public bool HasTarget => _bestMemory != null;
        
        public GameObject Target => HasTarget ? 
                _bestMemory.gameObject : 
                null;
        public Vector3 TargetPosition => HasTarget ? 
                _bestMemory.gameObject.transform.position : 
                Vector3.zero;
        
        public Transform TargetTransform => HasTarget ? 
                _bestMemory.gameObject.transform : 
                null;
        
        public bool TargetInSight => HasTarget ? 
                _bestMemory.Age < 0.5f : //TODO Магичиские 0.5f
                false;
        public float TargetDistance => HasTarget ? 
                _bestMemory.distance : 
                Mathf.Infinity;

        private NPC_MemoryCell _bestMemory = null;
        private readonly NPC_SensoryMemory _aiMemory = new NPC_SensoryMemory(10);
        private NPCVisonSensor _aiSensor;
        
        private void Start()
        {
            _aiSensor = GetComponent<NPCVisonSensor>();
        }

        private void Update()
        {
            _aiMemory.UpdateSenses(_aiSensor);
            _aiMemory.ForgetMemories(memorySpan);
            EvaluateScores();
        }

        private void EvaluateScores()
        {
            _bestMemory = null;
            
            foreach(NPC_MemoryCell memory in _aiMemory.memories)
            {
                if (memory.gameObject == this.gameObject)
                    continue;
                
                memory.score = CalculateScore(memory);
                if(_bestMemory == null || memory.score > _bestMemory.score)
                {
                    _bestMemory = memory;
                }
            }
        }

        private static float Normalize(float value, float maxValue)
        {
            return 1.0f - (value / maxValue);
        }

        private float CalculateScore(NPC_MemoryCell memory) //TODO Прикольная система оценивания приоритетных целей
        {
            float distanceScore = Normalize(memory.distance, _aiSensor.distance) * distanceWeight;
            float angleScore = Normalize(memory.angle, _aiSensor.angle) * angleWeight;
            float ageScore = Normalize(memory.Age, memorySpan) * ageWeight;
            float score = distanceScore + angleScore + ageScore;
            return score;
        }


        private void OnDrawGizmos()
        {
            float maxScore = float.MinValue;
            foreach(NPC_MemoryCell memory in _aiMemory.memories)
                maxScore = Mathf.Max(maxScore, memory.score);
            
            foreach(NPC_MemoryCell memory in _aiMemory.memories)
            {
                Color color = Color.red;
                
                if(memory == _bestMemory)
                    color = Color.yellow;
                
                color.a = memory.score / maxScore;
                Gizmos.color = color;
                Gizmos.DrawSphere(memory.position, 0.4f);
            }
        }
    }
}
