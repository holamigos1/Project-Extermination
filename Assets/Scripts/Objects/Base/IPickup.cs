using UnityEngine;

namespace Objects.Base
{
    public interface IPickup
    {
        public GameObject thisObject { get; }

        public GameObject Pickup();
    }
}