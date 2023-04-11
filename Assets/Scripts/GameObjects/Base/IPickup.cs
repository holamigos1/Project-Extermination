namespace GameObjects.Base
{
   public interface IPickup
    {
        public bool IsPickuped { get; }
        
        public Item Pickup();
    }
}