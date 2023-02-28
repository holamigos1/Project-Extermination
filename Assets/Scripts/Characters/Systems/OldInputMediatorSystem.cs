using GameSystems.Base;
using UnityEngine;

namespace Characters.Systems
{
    public class OldInputMediatorSystem : GameSystem
    {
        public override void Update()
        {
            base.Update();
            
            if(Input.GetKeyDown(KeyCode.G)) SystemsСontainer.NotifySystems("KeyDown","Drop");
            if(Input.GetKeyDown(KeyCode.E)) SystemsСontainer.NotifySystems("KeyDown","Interact");
            if(Input.GetKeyUp(KeyCode.E)) SystemsСontainer.NotifySystems("KeyUp","Interact");
            if(Input.GetKeyDown(KeyCode.Mouse0)) SystemsСontainer.NotifySystems("KeyDown","Fire");
            if(Input.GetKeyDown(KeyCode.Mouse1)) SystemsСontainer.NotifySystems("KeyDown","Aim");
        }
    }
}