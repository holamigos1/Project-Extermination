using System.Collections.Generic;
using UnityEngine;

namespace Artificial_Intelligence
{
    public class NPC_MemoryCell
    {
        public GameObject gameObject;
        public Transform transform;
        public Vector3 position;
        public Vector3 direction;
        public float distance;
        public float angle;
        public float lastSeen;
        public float score;
        public float Age => Time.time - lastSeen;
    }
    public class NPC_SensoryMemory 
    {
        public List<NPC_MemoryCell> memories = new List<NPC_MemoryCell>();
        private readonly GameObject [] _characters;
        private const string CREATURE_LAYER_NAME = "Creature";

        public NPC_SensoryMemory(int maxPlayers)
        {
            _characters = new GameObject[maxPlayers];
        }

        public void UpdateSenses(NPCVisonSensor sensor)
        {
            int targets = sensor.Filter(_characters, CREATURE_LAYER_NAME);
            
            for(int i =0; i< targets; ++i)
            {
                GameObject target = _characters[i];
                RefreshMemory(sensor.gameObject, target);
            }
        }

        public void RefreshMemory(GameObject agent, GameObject target)
        {
            NPC_MemoryCell memory = FetchMemory(target);
            memory.gameObject = target;
            memory.position = target.transform.position;
            memory.direction = target.transform.position - agent.transform.position;
            memory.distance = memory.direction.magnitude;
            memory.angle = Vector3.Angle(agent.transform.forward, memory.direction);
            memory.lastSeen = Time.time;
        }

        public NPC_MemoryCell FetchMemory(GameObject gameObject)
        {
            NPC_MemoryCell memory = memories.Find(x => x.gameObject == gameObject);
            
            if (memory != null) 
                return memory;
            
            memory = new NPC_MemoryCell();
            memories.Add(memory);
            return memory;
        }

        public void ForgetMemories(float olderThan)
        {
            memories.RemoveAll(m => m.Age > olderThan); // Remove all memories older than olderThan
            memories.RemoveAll(m => !m.gameObject); // Remove all memories that have no gameObject
            //memories.RemoveAll(m => m.gameObject.GetComponent<Health>().isDead); // Remove all memories that already dead
            //memories.RemoveAll(m => m.gameObject.GetComponent<HitBox>().health.isDead); // Remove all memories that already dead
            // var toRemove = memories.Find(m => m.gameObject.GetComponent<Health>().isDead); // Remove all memories that already dead
            // if(toRemove != null) memories.Remove(toRemove); // Remove all memories that already dead
        }
        
    }
}
