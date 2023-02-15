using System.Collections.Generic;

namespace Systems.Base
{
    public interface IObserver
    {
        public GameSystemsContainer SystemsСontainer { get; }
        public void OnNotify(string message, System.Object data);
        protected void NotifyOtherObservers(string message, System.Object data);
    }
}