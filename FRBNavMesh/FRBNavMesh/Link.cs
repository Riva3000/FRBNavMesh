using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace FRBNavMesh
{
    #region XML Docs
    /// <summary>
    /// Represents a one-way const-including path to a PositionedNode.
    /// </summary>
    #endregion
    public class Link<TPositionedNode> //: /*ILink,*/ IEquatable<Link<TPositionedNode>>
        where TPositionedNode : PositionedNode<Link<TPositionedNode>>, new()
    {
        #region Fields
        protected bool mActive; 

        private float mCost;

        private TPositionedNode mNodeLinkingTo;

        private Line mPortal;

        #endregion

        #region Properties

        public bool Active
        {
            get { return mActive; }
            set { mActive = value; }
        }

        #region XML Docs
        /// <summary>
        /// The cost to travel the link.
        /// </summary>
        /// <remarks>
        /// This is by default the distance to travel; however it can manually
        /// be changed to be any value to reflect different terrain, altitude, or other
        /// travelling costs.
        /// </remarks>
        #endregion
        public float Cost
        {
            get { return mCost; }
            set { mCost = value; }
        }

        #region XML Docs
        /// <summary>
        /// The destination PositionedNode.  The starting PositionedNode is not stored by the Link instance.
        /// </summary>
        #endregion
        public TPositionedNode NodeLinkingTo
        {
            get { return mNodeLinkingTo; }
            set { mNodeLinkingTo = value; }
        }

        public Line Portal
        {
            get { return mPortal; }
        }

        #endregion

        #region Methods

        #region XML Docs
        /// <summary>
        /// Creates a new Link.
        /// </summary>
        /// <param name="nodeLinkingTo">The node to link to.</param>
        /// <param name="cost">The cost to travel the link.</param>
        #endregion
        public Link(TPositionedNode nodeLinkingTo, float cost)
        {
            mNodeLinkingTo = nodeLinkingTo;
            mCost = cost;
            mActive = true; 
        }

        public override string ToString()
        {
            if (NodeLinkingTo == null)
                return Cost + " <not linking to any node>";
            else
                //return Cost.ToString(); // + " " + NodeLinkingTo.Name;
                return Cost + " " + NodeLinkingTo.Id;
        }

        #endregion


        #region IEquatable<Link> Members

        /*bool IEquatable<Link>.Equals(Link<TPositionedNode> other)
        {
            return this == other;
        }

        bool IEquatable<ILink>.Equals(Link<TPositionedNode> other)
        {
            return this == other;
        }*/

        #endregion
    }
}
