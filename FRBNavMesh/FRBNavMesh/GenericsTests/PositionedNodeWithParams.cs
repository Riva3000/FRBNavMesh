using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    class PositionedNodeWithParams : PositionedNode<LinkWithParams, PositionedNodeWithParams>
    {
        public bool IsStairs;
    }


}
