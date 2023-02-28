namespace GameSystems.Base
{
    public interface IGameSystem
    {
        public bool IsEnabled { get; }
        
        public void Start();
        public void Update();
        public void Stop();
    }
}