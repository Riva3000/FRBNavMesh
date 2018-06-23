using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace FRBNavMesh
{
    /// <summary>
    /// Represents a one-way const-including path to a PositionedNode.
    /// </summary>
    public abstract class LinkBase<TLink, TNode> : IEquatable<TLink>
        where TLink : LinkBase<TLink, TNode>, new()
        where TNode : PortalNodeBase<TLink, TNode>, new()
    {
        #region --- Properties
        protected bool mActive;
        public bool Active
        {
            get { return mActive; }
            set { mActive = value; }
        }

        protected float mCost;
        /// <summary>
        /// The cost to travel the link to it's destination.
        /// </summary>
        /// <remarks>
        /// This is by default the distance to travel; however it can manually
        /// be changed to be any value to reflect different terrain, altitude, or other
        /// travelling costs.
        /// </remarks>
        public float Cost
        {
            get { return mCost; }
            set { mCost = value; }
        }

        protected TNode mNodeLinkingTo;
        /// <summary>
        /// The destination PositionedNode.  The starting PositionedNode is not stored by the Link instance.
        /// </summary>
        public TNode NodeLinkingTo
        {
            get { return mNodeLinkingTo; }
            set { mNodeLinkingTo = value; }
        }

        protected SimpleLine mPortal;
        public SimpleLine Portal
        {
            get { return mPortal; }
            set { mPortal = value; }
        }
        #endregion --- Properties END




        /// <summary>
        /// Creates a new Link.
        /// </summary>
        /// <param name="nodeLinkingTo">The node to link to.</param>
        /// <param name="cost">The cost to travel the link.</param>
        /// <param name="portal">Portal line facing from parent PortalNode to destination node (for Channel construction).</param>
        public static TLink Create(TNode nodeLinkingTo, float cost, SimpleLine portal)
        {
            return new TLink
            {
                mNodeLinkingTo = nodeLinkingTo,
                mCost = cost,
                mPortal = portal,
                mActive = true,
            };
        }




        #region --- Methods

        public override string ToString()
        {
            if (NodeLinkingTo == null)
                return Cost + " <not linking to any node>";
            else
                //return Cost.ToString(); // + " " + NodeLinkingTo.Name;
                return Cost + " " + NodeLinkingTo.ID;
        }

        // - IEquatable implementation
        public bool Equals(TLink other)
        {
            return this == other;
        }

        #endregion --- Methods END


        
    }
}
