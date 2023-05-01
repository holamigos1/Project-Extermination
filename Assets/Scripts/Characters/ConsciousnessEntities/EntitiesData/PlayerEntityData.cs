using Characters.ConsciousnessEntities.Base;
using UnityEngine;

namespace Characters.ConsciousnessEntities.EntitiesData
{
    [CreateAssetMenu(fileName = "Player Entity", menuName = "Scriptable Data/Player Entity", order = 1)]
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
