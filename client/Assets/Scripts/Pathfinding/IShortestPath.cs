﻿using System.Collections.Generic;

namespace Assets.Scripts.Pathfinding
{
    public interface IShortestPath<TState, TAction>
    {
        /// <summary>
        /// Should return a estimate of shortest distance. The estimate must me admissible (never overestimate)
        /// </summary>
        float Heuristic(TState fromLocation, TState toLocation);

        /// <summary>
        /// Return the legal moves from a state
        /// </summary>
        List<TAction> Expand(TState position);

        /// <summary>
        /// Return the actual cost between two adjecent locations
        /// </summary>
        float ActualCost(TState fromLocation, TAction action);

        /// <summary>
        /// Returns the new state after an action has been applied
        /// </summary>
        TState ApplyAction(TState location, TAction action);
    }
}
