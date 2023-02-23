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
        public bool IsPickuped { get; }
        
        public GameObject Pickup();
    }
}