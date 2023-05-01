namespace GameObjects.Base
{
   public interface IPickup
    {
        public bool IsPickup { get; }
        
        public GameItem Pickup();
    }
}