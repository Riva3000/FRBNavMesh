using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using FlatRedBall;

using FlatRedBall.Math;

using FlatRedBall.Utilities;

using Microsoft.Xna.Framework;
using FlatRedBall.Math.Geometry;

namespace FRBNavMesh
{
    /// <summary>
    /// An object which has position properties 
    /// </summary>
    public abstract class PositionedNodeBase<TLink, TNode> : IStaticPositionable //, INameable
        where TLink : LinkBase<TLink, TNode>, new()
        where TNode : PositionedNodeBase<TLink, TNode>, new()
    {
        #region Fields

        //string mName;

        public Vector3 Position
        {
            get { return mPolygon.Position; }
            //private set { mPolygon.Position = value; }
        }

        // made internal for speed boosts
        protected internal List<TLink> mLinks = new List<TLink>();
        ReadOnlyCollection<TLink> mLinksReadOnly;

        public int PropertyField;

        public AStarState AStarState;

        /// <summary>
        /// The node that links to this node.  This is reset every time the 
        /// containing NodeNetwork searches for a path.
        /// </summary>
        protected internal TNode mParentNode;

        /// <summary>
        /// The cost to get to this node from the start node.  This variable is
        /// set when the containing NodeNetwork searches for a path.
        /// </summary>
        protected internal float mCostToGetHere;

        /// <summary>
        /// Only active nodes are included in pathfinding and find node searches. 
        /// </summary>
        /// Update February 10, 2013
        /// Nodes should always start 
        /// out as active.  
        protected bool mActive = true;

        // -- NavMesh
        protected int mId;
        protected AxisAlignedRectangle mPolygon;

        //protected SimpleLine[] mEdges;
        //protected SimpleLine mLeftEdge

        private SimpleLine _EdgeLeft;
        public SimpleLine EdgeLeft { get { return _EdgeLeft; } }
        private SimpleLine _EdgeRight;
        public SimpleLine EdgeRight { get { return _EdgeRight; } }
        private SimpleLine _EdgeTop;
        public SimpleLine EdgeTop { get { return _EdgeTop; } }
        private SimpleLine _EdgeBottom;
        public SimpleLine EdgeBottom { get { return _EdgeBottom; } }
        #endregion

        #region Properties

        /// <summary>
        /// Returns the cost to get to this node from the start node.  This
        /// value is only accurate if the node is contained in list returned
        /// by the last call to NodeNetwork.GetPath.
        /// </summary>
        /// <remarks>
        /// This value is reset anytime GetPath is called on the containing NodeNetwork.
        /// </remarks>
        public float CostToGetHere
        {
            get { return mCostToGetHere; }
        }

        /// <summary>
        /// The Node's name.  Mainly used for saving NodeNetworks since saved Links reference
        /// PositionedNodes by name.
        /// </summary>
        /*public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }*/

        /// <summary>
        /// The X position of the PositionedNode.
        /// </summary>
        public float X
        {
            get
            {
                return mPolygon.Position.X;
            }
            set
            {
                mPolygon.Position.X = value ;
            }
        }


        /// <summary>
        /// The Y position of the PositionedNode.
        /// </summary>
        public float Y
        {
            get
            {
                return mPolygon.Position.Y;
            }
            set
            {
                mPolygon.Position.Y = value; ;
            }
        }

        /// <summary>
        /// The Z position of the PositionedNode.
        /// </summary>
        public float Z
        {
            get
            {
                return mPolygon.Position.Z;
            }
            set
            {
                mPolygon.Position.Z = value ;
            }
        }

        /// <summary>
        /// The links belonging to this PositionedNode.
        /// </summary>
        /// <remarks>
        /// This is a list of Links which reference the PositionedNodes that this links to.
        /// Links are one-way and PositionedNodes that this links to do not necessarily contain
        /// Links back to this.
        /// </remarks>
        public ReadOnlyCollection<TLink> Links
        {
            get { return mLinksReadOnly; }
        }

        /// <summary>
        /// Only active nodes are included in pathfinding and find node searches. 
        /// </summary>
        public virtual bool Active
        {
            get { return mActive; }
            set { mActive = value; }
        }

        // -- NavMesh
        public int Id { get { return mId; } }

        public AxisAlignedRectangle Polygon
        {
            get { return mPolygon; }
            set { mPolygon = value; }
        }

        //public SimpleLine[] Edges { get { return mEdges; } }
        #endregion







        #region --- Methods

        /// <summary>Creates a new PositionedNode.</summary>
        public PositionedNodeBase()
        {
            mLinksReadOnly = new ReadOnlyCollection<TLink>(mLinks);
        }

        /// <summary>Creates a new PositionedNode.</summary>
        public static TNode Create(int id, AxisAlignedRectangle polygon)
        {
            var node = new TNode
            {
                mId = id,
                mPolygon = polygon,
                //mActive = true,
            };

            // Assign edges
            node._CalculateEdges();

            return node;
        }


        // --- Links
        #region XML Docs
        /// <summary>
        /// Disconnects all Links between this and the argument node.
        /// </summary>
        /// <param name="node">The PositionedNode to break links between.</param>
        #endregion
        public void BreakLinkBetween(TNode node)
        {
            for (int i = 0; i < node.mLinks.Count; i++)
            {
                if (node.mLinks[i].NodeLinkingTo == this)
                {
                    node.mLinks.RemoveAt(i);
                    break;
                }
            }

            for (int i = 0; i < mLinks.Count; i++)
            {
                if (mLinks[i].NodeLinkingTo == node)
                {
                    mLinks.RemoveAt(i);
                    break;
                }
            }
        }

        public TLink GetLinkTo(TNode node)
        {
            foreach (TLink link in mLinks)
            {
                if (link.NodeLinkingTo == node)
                {
                    return link;
                }
            }
            return null;
        }

        #region XML Docs
        /// <summary>
        /// Returns whether this has a Link to the argument PositionedNode.
        /// </summary>
        /// <remarks>
        /// If this does not link to the argument PositionedNode, but the argument
        /// links back to this, the method will return false.  It only checks links one-way.
        /// </remarks>
        /// <param name="node">The argument to test linking.</param>
        /// <returns>Whether this PositionedNode links to the argument node.</returns>
        #endregion
        public bool IsLinkedTo(TNode node)
        {
            foreach (TLink link in mLinks)
            {
                if (link.NodeLinkingTo == node)
                {
                    return true;
                }
            }
            return false;
        }

        public void LinkTo(TNode nodeToLinkTo, SimpleLine portalForThisNode)
        {
#if DEBUG
			if (nodeToLinkTo == this)
				throw new ArgumentException("Cannot have a node link to itself");
            if (portalForThisNode == null)
				throw new ArgumentNullException("portal");
#endif
            float distanceToTravel = (Position - nodeToLinkTo.Position).Length();

            LinkTo(nodeToLinkTo, distanceToTravel, portalForThisNode);
        }

        #region XML Docs
        /// <summary>
        /// Creates Links from this to the argument nodeToLinkTo, and another Link from the
        /// argument nodeToLinkTo back to this.
        /// </summary>
        /// <remarks>
        /// If either this or the argument nodeToLinkTo already contains a link to the other 
        /// PositionedNode, then the cost of the link is set to the argument costTo.
        /// </remarks>
        /// <param name="nodeToLinkTo">The other PositionedNode to create Links between.</param>
        /// <param name="costTo">The cost to travel between this and the argument nodeToLinkTo.</param>
        #endregion
        public void LinkTo(TNode nodeToLinkTo, float costTo, SimpleLine portalForThisNode)
        {
            LinkTo(nodeToLinkTo, costTo, costTo, portalForThisNode);
        }

        #region XML Docs
        /// <summary>
        /// Creates Links from this to the argument nodeToLinkTo, and another Link from the
        /// argument nodeToLinkTo back to this.
        /// </summary>
        /// <remarks>
        /// If either this or the argument nodeToLinkTo already contains a link to the other 
        /// PositionedNode, then the cost of the link is set to the argument costTo or costFrom as appropriate.
        /// </remarks>
        /// <param name="nodeToLinkTo">The other PositionedNode to create the Links between.</param>
        /// <param name="costTo">The cost to travel from this to the argument nodeToLinkTo.</param>
        /// <param name="costFrom">The cost to travel from the nodeToLinkTo back to this.</param>
        #endregion
        public void LinkTo(TNode nodeToLinkTo, float costTo, float costFrom, SimpleLine portalForThisNode)
        {
#if DEBUG
			if (nodeToLinkTo == this)
				throw new ArgumentException("Cannot have a node link to itself");
#endif
            bool updated = false;

            for (int i = 0; i < mLinks.Count; i++)
            {
                if (mLinks[i].NodeLinkingTo == nodeToLinkTo)
                {
                    mLinks[i].Cost = costTo;
                    mLinks[i].Portal = portalForThisNode;
                    updated = true;
                    break;
                }
            }
            if (!updated)
            {
                //mLinks.Add(new TLink(nodeToLinkTo, costTo));
                mLinks.Add( LinkBase<TLink, TNode>.Create(nodeToLinkTo, costTo, portalForThisNode) );
            }

            // Now do the same for the other node
            updated = false;
            for (int i = 0; i < nodeToLinkTo.mLinks.Count; i++)
            {
                if (nodeToLinkTo.mLinks[i].NodeLinkingTo == this)
                {
                    nodeToLinkTo.mLinks[i].Cost = costFrom;

                    //nodeToLinkTo.mLinks[i].Portal.Start = portalForThisNode.End;
                    //nodeToLinkTo.mLinks[i].Portal.End = portalForThisNode.Start;
                    nodeToLinkTo.mLinks[i].Portal = new SimpleLine(portalForThisNode.End, portalForThisNode.Start);

                    updated = true;
                    break;
                }
            }
            if (!updated)
            {
                //nodeToLinkTo.mLinks.Add(new TLink(this, costFrom));
                nodeToLinkTo.mLinks.Add( LinkBase<TLink, TNode>.Create(this as TNode, costFrom, new SimpleLine(portalForThisNode.End, portalForThisNode.Start)) );
            }
        }

        #region XML Docs
        /// <summary>
        /// Creates a link from this PositionedNode to the argument nodeToLinkTo.  Links
        /// on the argument nodeToLinkTo are not modified.
        /// </summary>
        /// <remarks>
        /// If this already links to the arugment nodeToLinkTo, the cost is set to the argument
        /// costTo.
        /// </remarks>
        /// <param name="nodeToLinkTo">The PositionedNode to create a link to.</param>
        /// <param name="costTo">The cost to travel from this to the argument nodeToLinkTo.</param>
        #endregion
        public void LinkToOneWay(TNode nodeToLinkTo, float costTo, SimpleLine portalForThisNode)
        {
            foreach (TLink link in mLinks)
            {
                if (link.NodeLinkingTo == nodeToLinkTo)
                {
                    link.Cost = costTo;
                    return;
                }
            }

            mLinks.Add( LinkBase<TLink, TNode>.Create(nodeToLinkTo, costTo, portalForThisNode) );
        }
        // --- Links END

        // --- Other
        public TNode Clone()
        {
            TNode newNode = (TNode)this.MemberwiseClone();

            newNode.mLinks = new List<TLink>();
            newNode.mLinksReadOnly = new ReadOnlyCollection<TLink>(newNode.mLinks);
            newNode.mParentNode = null;
            newNode.mCostToGetHere = 0;

            return newNode;
        }

        #region XML Docs
        /// <summary>
        /// Returns the string representation of this.
        /// </summary>
        /// <returns>The string representation of this.</returns>
        #endregion
        public override string ToString()
        {
            //return mName + string.Format(" ({0},{1},{2})", X, Y, Z);
            return mId + string.Format(" ({0},{1},{2})", X, Y, Z);
        }
        // --- Other END

        // --- NavMesh
        /*protected void _CalculateEdges()
        {
            // Assign edges
            mEdges = new SimpleLine[4];

            //  0 > > 3
            //  v     ^
            //  v     ^
            //  1 > > 2

            // Left - top-left to bottom-left
            mEdges[0] = new SimpleLine(mPolygon.Left, mPolygon.Top,    mPolygon.Left, mPolygon.Bottom);
            // Bottom - bottom-left to bottom-right
            mEdges[1] = new SimpleLine(mPolygon.Left, mPolygon.Bottom,    mPolygon.Right, mPolygon.Bottom);
            // Right - bottom-right to top-right
            mEdges[2] = new SimpleLine(mPolygon.Right, mPolygon.Bottom,    mPolygon.Right, mPolygon.Top);
            // Top - top-left to top-right
            mEdges[3] = new SimpleLine(mPolygon.Left, mPolygon.Top,    mPolygon.Right, mPolygon.Top);
        }*/
        protected void _CalculateEdges()
        {
            // Assign edges

            //  v1 old:
            //  0 > > 3
            //  v     v
            //  v     v
            //  1 > > 2

            //  v2 current - counter-clocwise order of points:
            //  0 < < 3
            //  v     ^
            //  v     ^
            //  1 > > 2

            // Left - top-left to bottom-left
            _EdgeLeft = new SimpleLine(mPolygon.Left, mPolygon.Top,    mPolygon.Left, mPolygon.Bottom);
            // Bottom - bottom-left to bottom-right
            _EdgeBottom = new SimpleLine(mPolygon.Left, mPolygon.Bottom,    mPolygon.Right, mPolygon.Bottom);
            // Right - bottom-right to top-right
            _EdgeRight = new SimpleLine(mPolygon.Right, mPolygon.Bottom,    mPolygon.Right, mPolygon.Top);
            // Top - top-right to top-left
            _EdgeTop = new SimpleLine(mPolygon.Right, mPolygon.Top,    mPolygon.Left, mPolygon.Top);

            /*Debug.ShowLine(_EdgeRight, Color.Green);
            Debug.ShowLine(_EdgeLeft, Color.DarkGreen);
            Debug.ShowLine(_EdgeTop, Color.Cyan);
            Debug.ShowLine(_EdgeBottom, Color.DarkCyan);*/
        }

        /*protected static float _CalculateRadius(AxisAlignedRectangle rect)
        {
            //var boundingRadius = 0;
            //foreach (Point point in this.polygon.points) {
            //    var d = this.centroid.distance(point);
            //    if (d > boundingRadius) boundingRadius = d;
            //}
            //return boundingRadius;

            // wrong. AARect.Top etc. return absolute (world) coordinates of edges of the AARect: return Math.Max(Polygon.Top, Polygon.Left);
            // wrong: return Math.Max(rect.Width, rect.Left) / 2;
            // All vertices in rectangle have same distance to center
            RCommonFRB.Geometry.Distance(rect.BoundingRadius)
        }*/
        // --- NavMesh END
        #endregion --- Methods END

    }
}
