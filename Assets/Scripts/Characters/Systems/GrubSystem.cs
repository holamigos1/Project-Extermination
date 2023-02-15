using JetBrains.Annotations;
using Objects.Base;
using Systems.Base;
using UnityEngine;

namespace Characters.Systems
{
    public class GrubSystem : GameSystem
    {
        private Transform _handPosition;
        
        public GrubSystem(GameSystemsContainer gameSystemsContainerInst, Transform handPosition) : base(gameSystemsContainerInst)
        {
            _handPosition = handPosition;
        }

        public override void OnNotify(string message, object data)
        {
            if (message == "Raycast Update")
            {
                if (data != null)
                {
                    GameObject gameObj = data as GameObject;
                    Debug.Log($"Notify message:{message} Data: {gameObj.name}");
                }
                else Debug.Log($"Notify message:{message} Data: {data.GetType()}");
            }
        }

        private void Grub(IPickup pickupObject)
        {
            GameObject pickupedObj = pickupObject.Pickup();

            if (pickupObject.PickUpType == PickUpType.InHand)
            {
                //Логика перемещения в руку
            }
            
            if (pickupObject.PickUpType == PickUpType.InInventory)
            {
                //Логика перемещения в инвентарь
            }
        }
    }
}