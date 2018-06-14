using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh.JavasciptAstar
{
    class Astar
    {
        // define functor type
        public delegate double Heuristic(GridNode pos0, GridNode pos1);
        public class HeuristicsList
        {
            public Heuristic manhattan =
                (pos0, pos1) =>
                {
                    var d1 = Math.Abs(pos1.x - pos0.x);
                    var d2 = Math.Abs(pos1.y - pos0.y);
                    return d1 + d2;
                };
            public Heuristic diagonal =
                (pos0, pos1) =>
                {
                    var D = 1;
                    var D2 = Math.Sqrt(2);
                    var d1 = Math.Abs(pos1.x - pos0.x);
                    var d2 = Math.Abs(pos1.y - pos0.y);
                    return (D * (d1 + d2)) + ((D2 - (2 * D)) * Math.Min(d1, d2));
                };
        }
        public static readonly HeuristicsList Heuristics = new HeuristicsList();

        public class Options
        {
            // functor
            /// <summary>
            /// Heuristic function (see astar.heuristics).
            /// </summary>
            public Heuristic heuristic;

            /// <summary>
            /// Specifies whether to return the path to the closest node if the target is unreachable.
            /// </summary>
            public bool closest;
            public bool diagonal;
        }


        /*
        * Perform an A* Search on a graph given a start and end node.
        * @param {Graph} graph
        * @param {GridNode} start
        * @param {GridNode} end
        * @param {Object} [options]
        * @param {bool} [options.closest] Specifies whether to return the
                    path to the closest node if the target is unreachable.
        * @param {Function} [options.heuristic] Heuristic function (see
        *          astar.heuristics).
        */
        /// <summary>
        /// Perform an A* Search on a graph given a start and end node.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="graph">graph</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static GridNode[] search<TNode>(Graph<GridNode> graph, GridNode start, GridNode end, Options options)
        {
            graph.cleanDirty();

            Heuristic heuristic;
            if (options.heuristic == null)
                heuristic = Heuristics.manhattan;
            else
                heuristic = options.heuristic;

            var closest = options.closest; // defaults to false

            //var openHeap = getHeap();
            /*
            getHeap() 
            {
                return new BinaryHeap(
                    function(node) 
                    {
                        return node.f;
                    }
                );
            }
            */
            var openHeap = ;
            var closestNode = start; // set the start node to be the closest if required

            start.h = heuristic(start, end);
            graph.markDirty(start);

            openHeap.Add(start);

            return ;
        }

        public static void cleanNode(GridNode node)
        {
            node.f = 0;
            node.g = 0;
            node.h = 0;
            node.visited = false;
            node.closed = false;
            node.parent = null;
        }




    }// class
}// namespace
