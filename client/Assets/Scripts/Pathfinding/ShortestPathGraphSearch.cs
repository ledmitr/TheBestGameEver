using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Pathfinding
{
    public class ShortestPathGraphSearch<TState, TAction>
    {
        class SearchNode<TNodeState, TNodeAction> : IComparable<SearchNode<TNodeState, TNodeAction>>
        {
            public SearchNode<TNodeState, TNodeAction> parent;
            public TNodeState NodeState;
            public TNodeAction NodeAction;
            public float g; // cost
            public float f; // estimate
            public SearchNode(SearchNode<TNodeState, TNodeAction> parent, float g, float f, TNodeState NodeState, TNodeAction NodeAction)
            {
                this.parent = parent;
                this.g = g;
                this.f = f;
                this.NodeState = NodeState;
                this.NodeAction = NodeAction;
            }
            // Reverse sort order (smallest numbers first)
            public int CompareTo(SearchNode<TNodeState, TNodeAction> other)
            {
                return other.f.CompareTo(f);
            }
            public override string ToString()
            {
                return "SN {f:" + f + ", state: " + NodeState + " action: " + NodeAction + "}";
            }
        }
        private IShortestPath<TState, TAction> info;
        public ShortestPathGraphSearch(IShortestPath<TState, TAction> info)
        {
            this.info = info;
        }
        public List<TAction> GetShortestPath(TState fromState, TState toState)
        {
            PriorityQueue<float, SearchNode<TState, TAction>> frontier = new PriorityQueue<float, SearchNode<TState, TAction>>();
            HashSet<TState> exploredSet = new HashSet<TState>();
            Dictionary<TState, SearchNode<TState, TAction>> frontierMap = new Dictionary<TState, SearchNode<TState, TAction>>();
            SearchNode<TState, TAction> startNode = new SearchNode<TState, TAction>(null, 0, 0, fromState, default(TAction));
            frontier.Enqueue(startNode, 0);
            frontierMap.Add(fromState, startNode);
            while (true)
            {
                if (frontier.IsEmpty) return null;
                SearchNode<TState, TAction> node = frontier.Dequeue();
                if (node.NodeState.Equals(toState)) return BuildSolution(node);
                exploredSet.Add(node.NodeState);
                // expand node and add to frontier
                foreach (TAction action in info.Expand(node.NodeState))
                {
                    TState child = info.ApplyAction(node.NodeState, action);
                    SearchNode<TState, TAction> frontierNode = null;
                    bool isNodeInFrontier = frontierMap.TryGetValue(child, out frontierNode);
                    if (!exploredSet.Contains(child) && !isNodeInFrontier)
                    {
                        SearchNode<TState, TAction> searchNode = CreateSearchNode(node, action, child, toState);
                        frontier.Enqueue(searchNode, searchNode.f);
                        exploredSet.Add(child);
                    }
                    else if (isNodeInFrontier)
                    {
                        SearchNode<TState, TAction> searchNode = CreateSearchNode(node, action, child, toState);
                        if (frontierNode.f > searchNode.f)
                        {
                            frontier.Replace(frontierNode, frontierNode.f, searchNode.f);
                        }
                    }
                }
            }
        }
        private SearchNode<TState, TAction> CreateSearchNode(SearchNode<TState, TAction> node, TAction action, TState child, TState toState)
        {
            float cost = info.ActualCost(node.NodeState, action);
            float heuristic = info.Heuristic(child, toState);
            return new SearchNode<TState, TAction>(node, node.g + cost, node.g + cost + heuristic, child, action);
        }
        private List<TAction> BuildSolution(SearchNode<TState, TAction> seachNode)
        {
            List<TAction> list = new List<TAction>();
            while (seachNode != null)
            {
                if ((seachNode.NodeAction != null) && (!seachNode.NodeAction.Equals(default(TAction))))
                {
                    list.Insert(0, seachNode.NodeAction);
                }
                seachNode = seachNode.parent;
            }
            return list;
        }
    }
}
