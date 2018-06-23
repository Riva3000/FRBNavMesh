using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using FlatRedBall;

using FlatRedBall.Math;

using FlatRedBall.Utilities;

using Microsoft.Xna.Framework;
using FlatRedBall.Math.Geometry;
using Point = FlatRedBall.Math.Geometry.Point;
using D = System.Diagnostics.Debug;

namespace FRBNavMesh
{
    /// <summary>
    /// An object which has position properties 
    /// </summary>
    public abstract class PortalNodeBase<TLink, TNode> : IStaticPositionable //, INameable
        where TLink : LinkBase<TLink, TNode>, new()
        where TNode : PortalNodeBase<TLink, TNode>, new()
    {
        #region --- Properties

        /// <summary>Portal center position</summary>
        public Vector3 Position;

        // made internal for speed boosts
        protected internal List<TLink> mLinks = new List<TLink>();
        ReadOnlyCollection<TLink> mLinksReadOnly;
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


        protected NavArea<TNode,TLink> mParentNavArea1;
        public NavArea<TNode,TLink> ParentNavArea1
        {
            get { return mParentNavArea1; }
        }
        protected NavArea<TNode,TLink> mParentNavArea2;
        public NavArea<TNode,TLink> ParentNavArea2
        {
            get { return mParentNavArea2; }
        }

        protected SimpleLine mPortalSide1;
        protected int mPortalSide1ParentAreaId;
        protected SimpleLine mPortalSide2;
        protected int mPortalSide2ParentAreaId;


        #region    -- A* internal properties
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
        #endregion -- A* internal properties END



        // -- NavMesh
        protected int mID;
        public int ID { get { return mID; } }

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

        /*string mName;
        /// <summary>
        /// The Node's name.  Mainly used for saving NodeNetworks since saved Links reference
        /// PositionedNodes by name.
        /// </summary>
        public string Name
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
                return Position.X;
            }
            set
            {
                Position.X = value ;
            }
        }

        /// <summary>
        /// The Y position of the PositionedNode.
        /// </summary>
        public float Y
        {
            get
            {
                return Position.Y;
            }
            set
            {
                Position.Y = value; ;
            }
        }

        /// <summary>
        /// The Z position of the PositionedNode.
        /// </summary>
        public float Z
        {
            get
            {
                return Position.Z;
            }
            set
            {
                Position.Z = value ;
            }
        }

        

        /// <summary>
        /// Only active nodes are included in pathfinding and find node searches. 
        /// </summary>
        /// Update February 10, 2013
        /// Nodes should always start 
        /// out as active.  
        protected bool mActive = true;
        /// <summary>
        /// Only active nodes are included in pathfinding and find node searches. 
        /// </summary>
        public virtual bool Active
        {
            get { return mActive; }
            set { mActive = value; }
        }

        #endregion --- Properties END





        #region --- Methods

        // * This class cannot use constructor with params !

        /// <summary>Creates a new PositionedNode.</summary>
        public PortalNodeBase()
        {
            mLinksReadOnly = new ReadOnlyCollection<TLink>(mLinks);
        }

        /// <summary>Creates a new PositionedNode.</summary>
        public static TNode Create(int id, NavArea<TNode,TLink> parentNavArea, NavArea<TNode,TLink> otherParentNavArea, SimpleLine portalForFirstParent)
        {
            Point portalCenter = _LineCenter(portalForFirstParent);

            var node = new TNode();
            node.mID = id;

            node.mParentNavArea1 = parentNavArea;
            node.mParentNavArea2 = otherParentNavArea;

            node.mPortalSide1 = portalForFirstParent;
            node.mPortalSide1ParentAreaId = parentNavArea.ID;
            node.mPortalSide2 = NavMesh<TNode, TLink>._GetInvertedLine(portalForFirstParent);
            node.mPortalSide2ParentAreaId = otherParentNavArea.ID;

            node.Position.X = (float)portalCenter.X;
            node.Position.Y = (float)portalCenter.Y;
            //node.mActive = true;

            return node;
        }

        /// <summary></summary>
        /// <param name="position">World position of this fake Node.</param>
        /// <param name="availablePortals">Portals leading out of NavArea this fake Node is positioned in.</param>
        private static TNode _CreateFakeNode(ref Point position, List<TNode> availablePortals, int nodeID)
        {
            var node = new TNode();
            node.mID = nodeID; // fake Node

            node.Position.X = (float)position.X;
            node.Position.Y = (float)position.Y;

            return node;
        }
        
        /// <summary></summary>
        /// <param name="position">World position of this fake Node.</param>
        /// <param name="availablePortals">Portals leading out of NavArea this fake Node is positioned in.</param>
        /// <param name="containingNavArea">NavArea this fake Node is positioned in.</param>
        public static TNode CreateFakeNodeDebug(ref Point position, List< TNode > availablePortals, bool isEndNode, NavArea<TNode,TLink> containingNavArea)
        {
            TNode node;
            if (isEndNode)
                node = CreateFakeEndNode(ref position, availablePortals, containingNavArea);
            else
                node = CreateFakeStartNode(ref position, availablePortals);

            node.mParentNavArea1 = containingNavArea;
            node.mParentNavArea2 = containingNavArea;

            return node;
        }
        
        /// <summary>
        /// - Create "fake" Portal Node in the "middle" of NavArea.
        /// - Tell all Portal Nodes in containing NavArea to link (one way) to the new fake Node.
        /// = other Portal Nodes will create links to this fake Node.
        ///   CleanupFakeEndNodeLinks() has to be called after fake Node is not needed anymore to remove those "fake" links.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="availablePortals"></param>
        /// <param name="containingNavAreaID"></param>
        /// <returns></returns>
        public static TNode CreateFakeEndNode(ref Point position, List<TNode> availablePortals, NavArea<TNode,TLink> containingNavArea)
        {
            var fakePortalNode = _CreateFakeNode(ref position, availablePortals, -2);

            float distanceBetweeNodes;
            foreach (var otherPortalNode in availablePortals)
            {
                distanceBetweeNodes = (float)RCommonFRB.Geometry.Distance2D(ref fakePortalNode.Position, ref otherPortalNode.Position);

                otherPortalNode.LinkToOneWay(
                            fakePortalNode,
                            distanceBetweeNodes, // cost
                            // Add real portal line here.
                            containingNavArea
                        );
            }

            return fakePortalNode;
        }

        /// <summary>
        /// Create "fake" Portal Node inside NavArea.
        /// I will auto-link one way to other Portal Nodes. = other Portal Nodes will not be affected, will not know about the links.
        /// </summary>
        public static TNode CreateFakeStartNode(ref Point position, List<TNode> availablePortals)
        {
            var node = _CreateFakeNode(ref position, availablePortals, -1);

            float distanceToTravel;
            foreach (var otherPortalNode in availablePortals)
            {
                distanceToTravel = (float)RCommonFRB.Geometry.Distance2D(ref node.Position, ref otherPortalNode.Position);

                node._FakeLinkToOneWay( otherPortalNode, distanceToTravel );
            }

            return node;
        }






        private static Point _LineCenter(SimpleLine line)
        {
            //x=(x1+x2) / 2
            //y=(y1+y22) / 2
            return new Point(
                            (line.Start.X + line.End.X) / 2,
                            (line.Start.Y + line.End.Y) / 2
                       );
        }

        public SimpleLine GetPortalSideFor(NavArea<TNode,TLink> navArea)
        {
            return GetPortalSideFor(navArea.ID);
        }
        public SimpleLine GetPortalSideFor(int navAreaID)
        {
            if (mPortalSide1ParentAreaId == navAreaID)
                //return mPortalSide1;
                return mPortalSide2;
#if DEBUG
            else if (mPortalSide2ParentAreaId == navAreaID)
                //return mPortalSide2;
                return mPortalSide1;
            else
                throw new ArgumentException("navAreaID", "NavArea not connected to this PortalNode.");
#else
            else
                //return mPortalSide2;
                return mPortalSide1;
#endif
        }

        

        // --- Links
        public static void CleanupFakeEndNodeLinks(List< TNode > fakeEndNodeLinkedPortals)
        {
            foreach (var portalNode in fakeEndNodeLinkedPortals)
                //portalNode._RemoveFakeLink();
                portalNode.mLinks.RemoveAt(portalNode.mLinks.Count - 1);
        }
        /*protected void _RemoveFakeLink()
        {
            mLinks.RemoveAt(mLinks.Count - 1);
        }*/

        /// <summary>
        /// Disconnects all Links between this and the argument node.
        /// </summary>
        /// <param name="node">The PositionedNode to break links between.</param>
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


        /// <summary>
        /// Returns whether this has a Link to the argument PositionedNode.
        /// </summary>
        /// <remarks>
        /// If this does not link to the argument PositionedNode, but the argument
        /// links back to this, the method will return false.  It only checks links one-way.
        /// </remarks>
        /// <param name="node">The argument to test linking.</param>
        /// <returns>Whether this PositionedNode links to the argument node.</returns>
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

        public void LinkTo(TNode nodeToLinkTo, NavArea<TNode,TLink> navArea)
        {
#if DEBUG
            if (nodeToLinkTo == this)
                //throw new ArgumentException("Cannot have a node link to itself");
                return;
            if (navArea == null)
				throw new ArgumentNullException( nameof(navArea) );
#endif
            float distanceToTravel = (Position - nodeToLinkTo.Position).Length();

            LinkTo(nodeToLinkTo, distanceToTravel, navArea);
        }

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
        public void LinkTo(TNode nodeToLinkTo, float costTo, NavArea<TNode,TLink> navArea)
        {
            LinkTo(nodeToLinkTo, costTo, costTo, navArea);
        }

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
        public void LinkTo(TNode nodeToLinkTo, float costTo, float costFrom, NavArea<TNode,TLink> navArea)
        {
#if DEBUG
            if (nodeToLinkTo == this)
                //throw new ArgumentException("Cannot have a node link to itself");
                return;
            if (navArea == null)
				throw new ArgumentNullException( nameof(navArea) );
#endif
            bool updatedExisting = false;

            // For this node
            for (int i = 0; i < mLinks.Count; i++)
            {
                if (mLinks[i].NodeLinkingTo == nodeToLinkTo)
                {
                    mLinks[i].Cost = costTo;
                    mLinks[i].Portal = GetPortalSideFor(navArea);
                    updatedExisting = true;
                    break;
                }
            }
            if (!updatedExisting)
            {
                //mLinks.Add(new TLink(nodeToLinkTo, costTo));
                mLinks.Add( LinkBase<TLink, TNode>.Create( nodeToLinkTo, costTo, GetPortalSideFor(navArea) ) );
            }

            // Now do the same for the other node
            updatedExisting = false;
            for (int i = 0; i < nodeToLinkTo.mLinks.Count; i++)
            {
                if (nodeToLinkTo.mLinks[i].NodeLinkingTo == this)
                {
                    nodeToLinkTo.mLinks[i].Cost = costFrom;

                    // v1
                    //nodeToLinkTo.mLinks[i].Portal.Start = portalForThisNode.End;
                    //nodeToLinkTo.mLinks[i].Portal.End = portalForThisNode.Start;
                    // v2
                    //nodeToLinkTo.mLinks[i].Portal = new SimpleLine(portalForThisNode.End, portalForThisNode.Start);
                    // v3
                    nodeToLinkTo.mLinks[i].Portal = nodeToLinkTo.GetPortalSideFor(navArea);

                    updatedExisting = true;
                    break;
                }
            }
            if (!updatedExisting)
            {
                //nodeToLinkTo.mLinks.Add(new TLink(this, costFrom));
                nodeToLinkTo.mLinks.Add( LinkBase<TLink, TNode>.Create(this as TNode, costFrom, nodeToLinkTo.GetPortalSideFor(navArea) ) );
            }
        }

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
        public void LinkToOneWay(TNode nodeToLinkTo, float costTo, NavArea<TNode,TLink> navArea)
        {
            foreach (TLink link in mLinks)
            {
                if (link.NodeLinkingTo == nodeToLinkTo)
                {
                    link.Cost = costTo;
                    return;
                }
            }

            mLinks.Add( LinkBase<TLink, TNode>.Create(nodeToLinkTo, costTo, this.GetPortalSideFor(navArea) ) );
        }

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
        protected void _FakeLinkToOneWay(TNode nodeToLinkTo, float costTo)
        {
            mLinks.Add( LinkBase<TLink, TNode>.Create(nodeToLinkTo, costTo, null ) );
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

        /// <summary>
        /// Returns the string representation of this.
        /// </summary>
        /// <returns>The string representation of this.</returns>
        public override string ToString()
        {
            return $"PortalNode {ID} Pos {X},{Y},{Z} Connecting Areas {ParentNavArea1.ID},{ParentNavArea2.ID}";
        }
        // --- Other END

#endregion --- Methods END

    }
}
