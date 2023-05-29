namespace GameItems.Base
{
   public interface IPickup
    {
        public bool IsPickup { get; }
        
        public GameItem Pickup();
    }
}