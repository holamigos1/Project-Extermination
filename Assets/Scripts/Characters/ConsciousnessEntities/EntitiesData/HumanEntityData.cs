using Characters.ConsciousnessEntities.Base;
using UnityEngine;

namespace Characters.ConsciousnessEntities.EntitiesData
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