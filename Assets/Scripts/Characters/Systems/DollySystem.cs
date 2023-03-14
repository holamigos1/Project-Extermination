using System;
using GameSystems.Base;
using UnityEngine;
using GameData.AnimationTags;
using Sirenix.OdinInspector;

namespace Characters.Systems
{
    [Serializable]
    public class DollySystem : GameSystem
    {
        [Title("Система куклы.", 
            "Ответчает за отображение урона, получаемое куклой.")]
        [ShowInInspector] [HideLabel] [DisplayAsString][PropertySpace(SpaceBefore = -5,SpaceAfter = -20)]
        #pragma warning disable CS0219
        private string info = "";

        [SerializeField] 
        private Animator _dollyAnimator;

        public override void Start()
        {
            SystemsСontainer.Notify += OnNotify;
        }
        
        public override void Stop()
        {
            SystemsСontainer.Notify -= OnNotify;
        }

        public override void OnNotify(string message, object data)
        {
            switch (message)
            {
                case "Damage applied":
                    _dollyAnimator.SetTrigger(AnimationParams.DAMAGED); 
                    break;
            }
        }
    }
}