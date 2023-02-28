namespace GameSystems.Base
{
    public interface IObserver
    {
        public void OnNotify(string message, System.Object data);
        public System.Object OnRequest(string message, object requestObject);
    }
}