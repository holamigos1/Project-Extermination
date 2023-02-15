using UnityEngine;


namespace Objects.Base
{
    public enum PickUpType
    {
        InHand,
        InInventory
    }
    
    public interface IPickup
    {
        public GameObject thisObject { get; }
        public PickUpType PickUpType { get; }
        
        public GameObject Pickup();
    }
}