namespace IntelligentVacuum.Agent
{
    public class GraphNode
    {
        public IntelligentVacuum.Environments.Room Room {get; private set;}
        public GraphNode Parent {get; private set;}
        internal GraphNode(IntelligentVacuum.Environments.Room room, GraphNode parent, AgentAction action)
        {
           Room=room;
           Parent=parent;
           Action=action;
        }
        public AgentAction Action {get; private set;}

    }
}