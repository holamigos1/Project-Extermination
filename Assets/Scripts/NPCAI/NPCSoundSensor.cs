using UnityEngine;

namespace NPCAI
{
    [ExecuteInEditMode]
    public class NPCSoundSensor : MonoBehaviour
    {
        SphereCollider sphereCollider;
        public Color color;
        public float radius = 5f;
        public float memoryTime = 2f;
        bool forgetMemory = false;
        float timer;
        public bool canHear = false;

        NPC_Agent agent;

        void Start()
        {
            sphereCollider = GetComponent<SphereCollider>();
            agent = GetComponent<NPC_Agent>();
            if(sphereCollider == null)
            {
                sphereCollider = gameObject.AddComponent<SphereCollider>();
            }
            sphereCollider.radius = radius;
            sphereCollider.isTrigger = true;
            timer = memoryTime;
        }

        
        void Update()
        {
            if(forgetMemory)
            {
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    canHear = false;
                    forgetMemory = false;
                    timer = memoryTime;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                canHear = true;
            }   
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                forgetMemory = true;   
            }   
        }

   

        void OnDrawGizmos()
        {
            Gizmos.color = color;
            Vector3 position = transform.position;
            position.y = position.y + 0.9f;
            Gizmos.DrawSphere(transform.position, radius);

        }
    }
}

