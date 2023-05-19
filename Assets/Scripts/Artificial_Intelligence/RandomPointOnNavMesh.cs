// RandomPointOnNavMesh

using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Artificial_Intelligence
{
    public class RandomPointOnNavMesh
    {
        private Vector3 _navMeshWorldPoint;
        
        public RandomPointOnNavMesh(Vector3 pointWorldPos)
        {
            _navMeshWorldPoint = pointWorldPos;
        }
        
        public float range = 10.0f;

        public static bool RandomPoint(Vector3 center, float inRange, out Vector3 result)
        {
            for (int i = 0; i < 30; i++) //TODO Магическая 30
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * inRange;
                
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }

        void Update()
        {
            if (RandomPoint(_navMeshWorldPoint, range, out Vector3 point))
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
        }
    }
}