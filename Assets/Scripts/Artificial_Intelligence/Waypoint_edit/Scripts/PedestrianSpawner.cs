using System.Collections;
using UnityEngine;

namespace Artificial_Intelligence.Waypoint_edit.Scripts
{
    public class PedestrianSpawner : MonoBehaviour
    {
        public GameObject [] pedestrianPrefab;
        public int pedestrianCount = 10;
        
        void Start()
        {
            StartCoroutine(SpawnPedestrians());
        }

        
        void Update()
        {
            
        }

        IEnumerator SpawnPedestrians()
        {
            for (int i = 0; i < pedestrianCount; i++)
            {
                GameObject pedestrian = Instantiate(pedestrianPrefab[(int) Random.Range(0,pedestrianPrefab.Length)]);
                Transform child = transform.GetChild(Random.Range(0, transform.childCount - 1));

                pedestrian.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
                pedestrian.transform.position = child.position;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
