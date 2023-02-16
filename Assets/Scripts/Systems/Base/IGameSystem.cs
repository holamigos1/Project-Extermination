using System.Collections.Generic;

namespace Systems.Base
{
    public interface IGameSystem
    {
        public void Start();
        public void Update();
        public void Stop();
    }
}