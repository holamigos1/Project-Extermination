using System.Collections.Generic;
using UnityEngine;

namespace Movement.SourseMovment
{
    public class CameraWaterCheck : MonoBehaviour
    {
        private readonly List<Collider> triggers = new();

        private void OnTriggerEnter(Collider other)
        {
            if (!triggers.Contains(other))
                triggers.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (triggers.Contains(other))
                triggers.Remove(other);
        }

        public bool IsUnderwater()
        {
            foreach (var trigger in triggers)
                if (trigger.GetComponentInParent<Water>())
                    return true;

            return false;
        }
    }
}