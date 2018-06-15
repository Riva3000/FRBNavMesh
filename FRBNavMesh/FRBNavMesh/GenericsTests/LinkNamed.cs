using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    class LinkNamed : Link<LinkNamed, PositionedNodeNamed>, FlatRedBall.Utilities.INameable
    {
        protected string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }


        /*
        public LinkWithParams()
        {

        }

        public LinkWithParams(PositionedNodeWithParams nodeLinkingTo, float cost)// : base(nodeLinkingTo, cost)
        {
        }
        */


    }
}
