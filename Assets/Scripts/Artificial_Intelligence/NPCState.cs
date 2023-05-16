namespace Artificial_Intelligence
{
    public enum NPCStateId
    {
        Idle,
        Patrol,
        ChasePlayer,
        Attack,
        Death,
        Flee,
        Alert,
        WaypointPatrol
    }
    
    public interface NPCState 
    {
        NPCStateId GetId();
        void Enter(NPC_Agent agent);
        void Update(NPC_Agent agent);
        void Exit(NPC_Agent agent);
    }
}