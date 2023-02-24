using Systems.Base;
using UnityEngine;

namespace Characters.Systems
{
    public class PlayerInputMediatorSystem : GameSystem
    {
        public PlayerInputMediatorSystem(GameSystemsContainer container) : base(container)
        {
            
        }

        public override void Update()
        {
            base.Update();
            
            if(Input.GetKeyDown(KeyCode.G)) SystemsСontainer.NotifySystems("KeyDown","Drop");
            if(Input.GetKeyDown(KeyCode.E)) SystemsСontainer.NotifySystems("KeyDown","Interact");
            if(Input.GetKeyUp(KeyCode.E)) SystemsСontainer.NotifySystems("KeyUp","Interact");
        }
    }
}