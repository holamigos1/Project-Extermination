using System;
using GameSystems.Base;
using UnityEngine;
using GameData.AnimationTags;

namespace Characters.Systems
{
    [Serializable]
    public class DollySystem : GameSystem
    {
        [SerializeField] private Animator _dollyAnimator;

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
                    Debug.Log("DSADSA");
                    _dollyAnimator.SetTrigger(AnimationParams.DAMAGED); 
                    break;
            }
        }
    }
}