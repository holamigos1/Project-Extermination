using System;
using GameSystems.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Systems
{
    [Serializable]
    public class OldInputMediatorSystem : GameSystem
    {
        //TODO Подумай над использованием новой Input System
        [Title("Проводник старой Input системы.", 
            "Является костылём и уберётся когда у программиста дойдут руки.")] 
        [ShowInInspector] [HideLabel] [DisplayAsString][PropertySpace(SpaceBefore = -5,SpaceAfter = -20)]
        #pragma warning disable CS0219
        private string info = "";
        
        public override void Start()
        {
            SystemsСontainer.Update += Update;
        }

        public override void Stop()
        {
            SystemsСontainer.Update -= Update;
        }

        public override void Update()
        {
            if(Input.GetKeyDown(KeyCode.G)) SystemsСontainer.NotifySystems("KeyDown","Drop");
            if(Input.GetKeyDown(KeyCode.E)) SystemsСontainer.NotifySystems("KeyDown","Interact");
            if(Input.GetKeyUp(KeyCode.E)) SystemsСontainer.NotifySystems("KeyUp","Interact");
            if(Input.GetKeyDown(KeyCode.Mouse0)) SystemsСontainer.NotifySystems("KeyDown","Fire");
            if(Input.GetKeyDown(KeyCode.Mouse1)) SystemsСontainer.NotifySystems("KeyDown","Aim");
        }
    }
}