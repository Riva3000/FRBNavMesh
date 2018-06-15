using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    class PositionedNodeNamed : PositionedNode<LinkNamed, PositionedNodeNamed>, FlatRedBall.Utilities.INameable
    {
        protected string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
    }


}
