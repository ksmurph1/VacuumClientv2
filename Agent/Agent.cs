namespace IntelligentVacuum.Agent
{
    using System;
    using System.Collections.Generic;
    using Environments;
    using System.Linq;

    public class Agent
    {
        private struct roomComparer : IEqualityComparer<GraphNode>
        {
            public bool Equals(GraphNode x, GraphNode y)
            {
                return x.Room == y.Room;
            }

            public int GetHashCode(GraphNode node)
            {
               return node.Room.GetHashCode();
            }
        }
        private Sensor sensor;
        private Queue<GraphNode> explored=new Queue<GraphNode>();
        public Agent(Sensor sensor)
        {
           this.sensor=sensor;
        }

        public AgentAction DecideAction(Room room)
        {
            AgentAction curAction=AgentAction.NONE;
            GraphNode curNode;
            bool success=explored.TryDequeue(out curNode);
            if (!success)
            {
               SetPath(room);
               success=explored.TryDequeue(out curNode);
            }   
            if (success)
            {
               curAction=curNode.Action;
            }
            return curAction;
        }

        public void SetPath(Room room)
        {
        
        // search for a goal node
           Queue<GraphNode> frontier= new Queue<GraphNode>();
           GraphNode node=new GraphNode(room,null,AgentAction.NONE);
           frontier.Enqueue(node);
           bool empty=false;
           do
           {
               if (!frontier.TryDequeue(out node)) // see if frontier empty
               {
                   empty=true;
               }
               else
               {
                   List<GraphNode> newNodes=Explore(node);
                   explored.Enqueue(node);
                
                   foreach (GraphNode newNode in newNodes.Except(explored,new roomComparer())) // iterate over nodes except already explored
                   {
                      frontier.Enqueue(newNode);  //nodes to be explored
                   }
               }
           } while (!node.Room.IsDirty && !empty);  //dirty room is goal or empty if no dirty rooms

           if (empty)
           {
              explored.Clear();
           }
        }

        private List<GraphNode> Explore(GraphNode node)
        {
            List<GraphNode> discovered=new List<GraphNode>();
            AgentAction[] moveActions=new AgentAction[]
            {AgentAction.MOVE_DOWN, AgentAction.MOVE_LEFT, AgentAction.MOVE_RIGHT,AgentAction.MOVE_UP};
            foreach (AgentAction action in moveActions)
            {
                GraphNode newNode=Transition(node, action);
                if (newNode != null)
                {
                    discovered.Add(newNode);
                }
            }
            return discovered;
        }
        private GraphNode Transition(GraphNode node, AgentAction action)
        {
            GraphNode newNode=null;
            Tuple<int,int> xydir=null;
            switch (action)
            {
                case AgentAction.MOVE_DOWN:
                   xydir=Tuple.Create(0,1);
                   break;
                case AgentAction.MOVE_UP:
                   xydir=Tuple.Create(0,-1);
                   break;
                case AgentAction.MOVE_RIGHT:
                   xydir=Tuple.Create(1,0);
                   break;
                case AgentAction.MOVE_LEFT:
                   xydir=Tuple.Create(-1,0);
                   break;
            }

            if (xydir != null)
            {
                 Room newRoom=sensor.SenseRoom(node.Room.XAxis+xydir.Item1, node.Room.YAxis+xydir.Item2);
                   if (!newRoom.IsLocked) // don't include obstacles
                   {
                      if (newRoom.IsDirty)  
                         newNode=new GraphNode(newRoom,node,AgentAction.CLEAN);
                      else
                          newNode=new GraphNode(newRoom,node, action);
                   }
            }
            return newNode;
        }
    }
}