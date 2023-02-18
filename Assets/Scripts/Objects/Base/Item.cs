using Systems.Base;
using UnityEngine;

namespace Objects.Base
{
    public abstract class Item : MonoBehaviour
    {
        public GameSystemsContainer SystemsContainer => _systemsContainer;
        private GameSystemsContainer _systemsContainer;
        
    }
}