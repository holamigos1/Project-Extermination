using System;
using Characters.ConsciousnessEntities.Base;
using Characters.Data.Base;
using NewInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.ConsciousnessEntities
{
    [CreateAssetMenu(fileName = "Player Entity", menuName = "Game Data/Player Entity", order = 1)]
    public class PlayerEntityData : ConsciousnessEntityData, IHumanEntityCreator
    {
        public IHumanEntity CreateEntityInstance()
        {
            var playerHumanInstance = new PlayerHumanEntity(GetInstanceID());
            
            //TODO Сохранить ссылку на playerHumanInstance в синглтоне-помощнике для PlayerEntity
            
            return playerHumanInstance;
        }
    }
}
