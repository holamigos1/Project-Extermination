using Objects.Base;

namespace Characters.Systems
{
    public class GrubSystem
    {
        public GrubSystem()
        {

        }

        public void Grub(IPickup pickupableObject)
        {
            pickupableObject.Pickup();

        }
    }
}