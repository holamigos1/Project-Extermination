using Objects.Base;
using Systems.Base;
using UnityEngine;

namespace Characters.Systems
{
    public class GrubSystem : GameSystem
    {
        public GrubSystem(GameSystemsContainer gameSystemsContainerInst, Transform handPosition) : base(gameSystemsContainerInst)
        {
            _handPosition = handPosition;
        }
        
        private Transform _handPosition;
        private bool _interactKeyPressed;

        public override void OnNotify(string message, object data)
        {
            switch (message)
            { 
                case "KeyDown" when data != null:
                    if(data.ToString() == "Interact") 
                    {
                        GameObject requestResponse = SystemsСontainer.MakeRequest("Get raycast object").GetFirstAs<GameObject>();
                        if (requestResponse == null) break;
                        if (requestResponse.TryGetComponent(out IPickup pickupObject)) Grub(pickupObject);
                    }
                    break;
            }
        }

        private void Grub(IPickup pickupObject)
        {
            GameObject gameObject = pickupObject.Pickup();

            Debug.Log($"gRUB 2: {pickupObject.PickUpType}");
            
            if (pickupObject.PickUpType == PickUpType.InHand)
            {
                SystemsСontainer.NotifySystems("Object pickuped", gameObject);
            }
            
            if (pickupObject.PickUpType == PickUpType.InInventory)
            {
                //Логика перемещения в инвентарь
            }
        }
    }
}