using System;
using Characters.ConsciousnessEntities.Base;
using Characters.Data.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.ConsciousnessEntities
{
    [CreateAssetMenu(fileName = "Character Entity", menuName = "Game Data/Character Entity", order = 1)]
    public class HumanEntityData : ConsciousnessEntityData, IHumanEntityCreator
    {
        public IHumanEntity CreateEntityInstance()
        {
            return new HumanEntity(GetInstanceID());
        }
    }
}