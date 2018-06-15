using FRBNavMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    public static class PositionedNodeListExtensionMethods
    {
        public static float PathDistanceSquared<TNode, TLink>(this List<TNode> path) where TNode : PositionedNode<TNode, TLink>
        {
            float total = 0;

            for (int i = 0; i < path.Count - 1; i++)
            {
                total += (path[i].Position - path[i + 1].Position).LengthSquared();
            }

            return total;
        }
    }
}
