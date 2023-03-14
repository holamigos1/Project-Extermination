using UnityEngine;


namespace Objects.Base
{
   public interface IPickup
    {
        public bool IsPickuped { get; }
        
        public Item Pickup();
    }
}