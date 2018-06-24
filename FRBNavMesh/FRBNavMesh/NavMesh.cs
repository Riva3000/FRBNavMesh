//#define RemovedForTesting
//#define o // if defined, classes methods write debug info into debug output

using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xna = Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

using System.Diagnostics;
using D = System.Diagnostics.Debug;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
//using FDebug = FRBNavMesh.Debug;

namespace FRBNavMesh
{
    /*
    * The workhorse that represents a navigation mesh built from a series of polygons. Once built, the
    * mesh can be asked for a path from one point to another point. It has debug methods for 
    * visualizing paths and visualizing the individual polygons. Some internal terminology usage:
    * 
    * - neighbor: a polygon that shares part of an edge with another polygon
    * - portal / portal node: when two neighbor's have edges that overlap, the portal is the overlapping line segment
    * - channel: the path of polygons from starting point to end point
    * - pull the string: run the funnel algorithm on the channel so that the path hugs the edges of the
    *   channel. Equivalent to having a string snaking through a hallway and then pulling it taut.
    */

    public struct RLineAndPoint
    {
        public SimpleLine Line;
        public Point Point;

        public RLineAndPoint(SimpleLine line, Point point)
        {
            Line = line;
            Point = point;
        }
    }

    /// <summary>
    /// The workhorse that represents a navigation mesh built from a series of axis-allinged rectangles.
    /// Once built, the mesh can be asked for a path from one point to another point.
    /// <para>
    /// Some internal terminology usage:
    ///  - neighbor: a polygon that shares part of an edge with another polygon
    ///  - portal / portal node: when two neighbor's have edges that overlap, the portal is the overlapping line segment
    ///  - channel: the path of polygons from starting point to end point
    ///  - pull the string: run the funnel algorithm on the channel so that the path hugs the edges of the
    ///    channel. Equivalent to having a string snaking through a hallway and then pulling it taut.
    /// </para>
    /// <para>
    /// This class is generic, so both class for PortalNodes and Links can be extended to contain additional 
    /// data or functionality for your specific implementation.
    /// </para>
    /// </summary>
    /// <typeparam name="TNode">Class representing Portal Node. Has to be concrete class derived from PortalNodeBase.</typeparam>
    /// <typeparam name="TLink">Class representing link between Nodes. Has to be concrete class derived from LinkBase.</typeparam>
    public class NavMesh<TNode, TLink>
        where TNode : PortalNodeBase<TLink, TNode>, new()
        where TLink : LinkBase<TLink, TNode>, new()
    {
        #region    --- Vars
        protected readonly List<NavArea<TNode, TLink>> _NavAreas;
        protected readonly ReadOnlyCollection< NavArea<TNode, TLink> > _NavAreasReadonly;
        /// <summary>
        /// Read-only list of recangular areas that define walkable space for this NavMesh.
        /// </summary>
        public ReadOnlyCollection< NavArea<TNode, TLink> > NavAreas { get { return _NavAreasReadonly; } }

        protected readonly List<TNode> _PortalNodes;
        protected readonly ReadOnlyCollection<TNode> _PortalNodesReadonly;
        /// <summary>
        /// Read-only list of Nodes, that represent "portals" that path finsing agent can traverse between two areas in this NavMesh.
        /// </summary>
        public ReadOnlyCollection<TNode> PortalNodes { get { return _PortalNodesReadonly; } }

        #region    -- For A*
        // This reduces memory allocation during runtime and also reduces the argument list size
        protected List<TNode> _ClosedList = new List<TNode>(30);
        protected List<TNode> _OpenList = new List<TNode>(30);

        protected float _ShortestPath;
        #endregion -- For A* END 
        #endregion --- Vars END






        /// <summary>Create new NavMesh to be used for path finding.</summary>
        /// <param name="rectangles">Collection of FlatRedBall AxisAllignedRectangles 
        /// new NavMesh will use as base for definig it's areas and portals.</param>
        /// <param name="meshShrinkAmount">The amount (in pixels) that the navmesh has been
        /// shrunk around obstacles (a.k.a the amount obstacles have been expanded)</param>
        public NavMesh(IList<AxisAlignedRectangle> rectangles /*, int meshShrinkAmount = 0*/)
        {
            // Construct NavArea instances for each polygon
            _NavAreas = new List<NavArea<TNode,TLink>>(rectangles.Count);
            _NavAreasReadonly = new ReadOnlyCollection<NavArea<TNode, TLink>>(_NavAreas);
            for (int i = 0; i < rectangles.Count; i++)
            {
                _NavAreas.Add( new NavArea<TNode,TLink>(rectangles[i], i) );
            }

            // Connect the NavAreas
            _PortalNodes = new List<TNode>(rectangles.Count * 3);
            _PortalNodesReadonly = new ReadOnlyCollection<TNode>(_PortalNodes);

            _CalculateNeighbors();
        }





        #region    --- Methods

        /// <summary>Find a path from the start point to the end point using this nav mesh.</summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="drawPolyPath">Whether or not to visualize the path through the polygons - e.g. the path that astar found.</param>
        /// <param name="drawFinalPath">Whether or not to visualize the path through the path that was returned.</param>
        /// <returns>An array of points and nodes if a path is found, or nulls if no path</returns>
        public List<Point> FindPath(Point startPoint, Point endPoint, out List<TNode> portalNodesPath
            /*, bool drawPolyPath = false, bool drawFinalPath = false*/)
        {
            #region    --- Find the closest poly for the starting and ending point
            NavArea<TNode, TLink> startArea = FindNavRectFromPoint(ref startPoint);
            NavArea<TNode, TLink> endArea = FindNavRectFromPoint(ref endPoint);

            // No matching polygons locations for the start or end, so no path found
            // = start or end point not on nav mesh
            if (startArea == null || endArea == null)
            {
                portalNodesPath = null;
                return null;
            }

            // If the start and end polygons are the same, return a direct path
            if (startArea == endArea)
            {
                List<Point> pointsPath = new List<Point> { startPoint, endPoint };
                //if (drawFinalPath) this.debugDrawPath(phaserPath, 0xffd900, 10);
                portalNodesPath = null; // not traversing any Portal Nodes
                return pointsPath;
            }
            #endregion --- Find the closest poly for the starting and ending point END



            #region    --- A* search
            // --- Search!
            portalNodesPath = new List<TNode>();
            GetPath(
#if DEBUG
                PortalNodeBase<TLink, TNode>.CreateFakeNodeDebug(ref startPoint, startArea.Portals, false, startArea),
                PortalNodeBase<TLink, TNode>.CreateFakeNodeDebug(ref endPoint, endArea.Portals, true, endArea),
#else
                PortalNodeBase<TLink, TNode>.CreateFakeStartNode(ref startPoint, startArea.Portals),
                PortalNodeBase<TLink, TNode>.CreateFakeEndNode(ref endPoint, endArea.Portals, endArea),
#endif
                portalNodesPath
            );

            if (portalNodesPath.Count == 0)
            {
                // While the start and end polygons may be valid, no path between them

                portalNodesPath = null;
                return null;
            }
            #endregion --- A* search END



            #region    --- Funnel algorithm
            // We have a path, so now time for the funnel algorithm
#if o
            D.WriteLine("======== Path search ========");
            D.WriteLine("  --- Path ---");
            foreach (var pathNode in portalNodesPath)
            {
                D.WriteLine("    " + pathNode.ID);
            }
            D.WriteLine("  --- Channel ---");
#endif
            Channel channel = new Channel();
            channel.Add(startPoint);

            SimpleLine portal;
            int countLimit = portalNodesPath.Count - 1;
            TNode pathPortalNode;
            TNode nextPathPortalNode;
            for (int i = 1; i < countLimit; i++) // skipping first Node - I don't need it's Links - I don't need their Portals
            {
                pathPortalNode = portalNodesPath[i];
                nextPathPortalNode = portalNodesPath[i + 1];
#if o
                D.WriteLine($"    pathPortalNode: {pathPortalNode.ID} and pathPortalNode: {nextPathPortalNode.ID}");
#endif
                // Find the portal
                portal = null;
                foreach (var link in pathPortalNode.Links)
                {
                    //if ( link.NodeLinkingTo.ReferenceEquals(nextPathPortalNode) )
                    if (link.NodeLinkingTo.ID == nextPathPortalNode.ID)
                    {
#if o
                        D.WriteLine($"      link to {link.NodeLinkingTo.ID} - link found and portal added");
#endif
                        portal = link.Portal;
                        // Push the portal vertices into the channel
                        channel.Add(portal.Start, portal.End);
                        break;
                    }
#if o
                    else
                    {
                        D.WriteLine($"      link to {link.NodeLinkingTo.ID} - link NOT found !");
                    }
#endif
                }
            }

            channel.Add(endPoint);


            // Pull a string along the channel to run the funnel
            channel.StringPull();

            PortalNodeBase<TLink, TNode>.CleanupFakeEndNodeLinks(endArea.Portals);

            // Clone path, excluding duplicates - needed ? @
            Point? lastPoint = null;
            List<Point> finalPointsPath = new List<Point>();
            foreach (var point in channel.Path)
            {
                //var newPoint = p.clone();
                var newPoint = point;
                //if (!lastPoint || !newPoint.equals(lastPoint)) 
                if (lastPoint.HasValue == false || newPoint != lastPoint)
                    finalPointsPath.Add(newPoint);
                lastPoint = newPoint;
            }

            return finalPointsPath;
            #endregion --- Funnel algorithm END
        }

        /// <summary>Goes through all Nodes and ties to find Node which rect the point is inside of.</summary>
        /// <returns>Node of which rect the point is inside of, or null if point is not inside any node's rect.</returns>
        public NavArea<TNode, TLink> FindNavRectFromPoint(ref Point point)
        {
            NavArea<TNode, TLink> containingNavArea = null;
            double bestDistance = double.PositiveInfinity;
            double d;
            float r;

            // Find the closest poly for the starting and ending point
            foreach (var navArea in _NavAreas)
            {
                r = navArea.Polygon.BoundingRadius;
                // Start
                d = RUtils.Distance2D(ref navArea.Polygon.Position, ref point);
                if (d <= bestDistance && d <= r && navArea.Polygon.IsPointInside((float)point.X, (float)point.Y)) // @
                {
                    containingNavArea = navArea;
                    bestDistance = d;
                }
            }

            // No matching polygons locations for the point (so no path can be found)
            // = point not on nav mesh
            return containingNavArea;
        }

        #region    -- A*
        public void GetPath(TNode start, TNode end, List<TNode> listToFill)
        {
            if (start.Active == false || end.Active == false)
            {
                return;
            }
            else if (start == end)
            {
                listToFill.Add(start);
                return;
            }
            else
            {
                start.mParentNode = null;
                end.mParentNode = null;
                start.mCostToGetHere = 0;
                end.mCostToGetHere = 0;

                _OpenList.Clear();
                _ClosedList.Clear();

                _OpenList.Add(start);
                start.AStarState = AStarState.Open;

                _ShortestPath = float.PositiveInfinity;
                while (_OpenList.Count != 0)
                {
                    _GetPathCalculate(_OpenList[0], end);
                }

                // inefficient, but we'll do this for now
                if (end.mParentNode != null)
                {

                    TNode nodeOn = end;

                    listToFill.Insert(0, nodeOn);

                    while (nodeOn.mParentNode != null)
                    {
                        listToFill.Insert(0, nodeOn.mParentNode);

                        nodeOn = nodeOn.mParentNode;
                    }

                }

                for (int i = _ClosedList.Count - 1; i > -1; i--)
                {
                    _ClosedList[i].AStarState = AStarState.Unused;
                }
                for (int i = _OpenList.Count - 1; i > -1; i--)
                {
                    _OpenList[i].AStarState = AStarState.Unused;
                }
            }
        }

        protected virtual void _GetPathCalculate(TNode currentNode, TNode endNode)
        {
            _OpenList.Remove(currentNode);
            _ClosedList.Add(currentNode);
            currentNode.AStarState = AStarState.Closed;
            bool partOfOpen = false;

            int linkCount = currentNode.Links.Count;

            foreach (TLink currentLink in currentNode.Links)
            //for (int i = 0; i < linkCount; i++)
            {
                //Link currentLink = currentNode.Links[i];

                ///Links can be turned off, and when they are in that 
                ///state they should be ignored by pathfinding calls. 
                if (!currentLink.Active)
                {
                    continue;
                }
                TNode nodeLinkingTo = currentLink.NodeLinkingTo; //currentNode.Links[i].NodeLinkingTo;

                if (nodeLinkingTo.AStarState != AStarState.Closed && nodeLinkingTo.Active)
                {
                    float cost = currentNode.mCostToGetHere + currentLink.Cost;

                    if (cost < _ShortestPath)
                    {
                        partOfOpen = nodeLinkingTo.AStarState == AStarState.Open;

                        if (partOfOpen == false ||
                            cost <= nodeLinkingTo.CostToGetHere)
                        {
                            nodeLinkingTo.mParentNode = currentNode;
                            nodeLinkingTo.mCostToGetHere =
                                currentNode.mCostToGetHere + currentLink.Cost;

                            if (nodeLinkingTo == endNode)
                            {
                                _ShortestPath = nodeLinkingTo.mCostToGetHere;
                                /// September 6th, 2012 - Jesse Crafts-Finch
                                /// Removed the break because it prevents the currentNode from checking
                                ///  alternative links which may end up creating a cheaper path to the endNode.                                
                                //break;
                            }

                            if (partOfOpen)
                            {
                                _OpenList.Remove(nodeLinkingTo);
                                nodeLinkingTo.AStarState = AStarState.Unused;
                            }
                        }


                        _AddNodeToOpenList(nodeLinkingTo);
                    }
                }
            }
        }

        protected void _AddNodeToOpenList(TNode node)
        {
            bool added = false;

            // See if the node is already part of the open node list
            // If it is, just remove it and re-add it just in case its
            // cost has changed, then exit.
            if (node.AStarState == AStarState.Open)
            {
                _OpenList.Remove(node);
                node.AStarState = AStarState.Unused;
            }

            for (int i = 0; i < _OpenList.Count; i++)
            {
                if (node.mCostToGetHere < _OpenList[i].mCostToGetHere)
                {
                    _OpenList.Insert(i, node);
                    node.AStarState = AStarState.Open;
                    added = true;
                    break;
                }
            }

            if (added == false)
            {
                _OpenList.Add(node);
                node.AStarState = AStarState.Open;
            }
        }
        #endregion -- A* END

        private void _CalculateNeighbors()
        {
#if o
            D.WriteLine(" * NavMesh._CalculateNeighbors()");
#endif
            // Fill out the neighbor information for each NavArea
            // Find and create Portals (Portal Nodes) for each NavArea
            #region    
            NavArea<TNode, TLink> navArea;
            NavArea<TNode, TLink> otherNavArea;
            AxisAlignedRectangle navAreaPolygon;
            AxisAlignedRectangle otherNavAreaPolygon;
            TNode portalNode;
            SimpleLine portal;
            int portalId = 1;
            for (int i = 0; i < _NavAreas.Count; i++)
            {
                navArea = _NavAreas[i];
                navAreaPolygon = navArea.Polygon;
#if o
                D.WriteLine("   navArea: " + navAreaPolygon.Name);
                //(navArea as PositionedNode).CheckedAsMain = true;
#endif
                for (int j = i + 1; j < _NavAreas.Count; j++)
                {
                    otherNavArea = _NavAreas[j];
                    otherNavAreaPolygon = otherNavArea.Polygon;
#if o
                    D.WriteLine("     otherNavPoly: " + otherNavAreaPolygon.Name);
#endif
                    #region    - Check polygons distance
                    // Check if the other navpoly is within range to touch
                    // Distance between centers
                    var distanceBetweenCenters = RUtils.Distance2D(ref navAreaPolygon.Position, ref otherNavAreaPolygon.Position);
                    // If Distance between centers is bigger than combined radii, they are not in range
                    // If Distance between centers is smaller or equal, they are in range (not necessarily touching)
                    if (distanceBetweenCenters >= navAreaPolygon.BoundingRadius + otherNavAreaPolygon.BoundingRadius)
                    {
                        // Not in range => proceed to another navpoly
#if o
                        D.WriteLine($"       Not in range (distanceBetweenCenters: {distanceBetweenCenters} totalRadii: {navAreaPolygon.BoundingRadius + otherNavAreaPolygon.BoundingRadius})");
#endif
                        continue;
                    }
#if o
                    D.WriteLine($"       In range (distanceBetweenCenters: {distanceBetweenCenters} totalRadii: {navAreaPolygon.BoundingRadius + otherNavAreaPolygon.BoundingRadius})");\
#endif
                    #endregion - Check polygons distance END

                    // The are in range, so check each edge pairing
                    // to find shared edge and shared part of edges = portal
                    #region    - Using common sense
                    // Here I know they do overlap
                    //      Calculate the portal between the two polygons 
                    //      - THIS NEEDS TO BE IN CLOCKWISE ORDER, RELATIVE TO EACH POLYGON
                    // _GetSegmentOverlap() 
                    //  returns horizontal: always left-to-right
                    //  returns vertical: always bottom-to-top
                    portal = null;
                    // other is above
                    if (navAreaPolygon.Top == otherNavAreaPolygon.Bottom)
                    {
#if o
                        D.WriteLine($"       Other above -- Touching this Top ({navAreaPolygon.Top}) - other Bottom ({otherNavAreaPolygon.Bottom})");
#endif
                        portal = _GetSegmentOverlap(navArea.EdgeTop, otherNavArea.EdgeBottom, true, false);
#if o
                        // Debug visuals
                        if (portal != null)
                        {
                            //_DrawDebugVisualForPortal(portal.Start.X + 3f, portal.Start.Y - 3f, portal.End.X - 3f, portal.End.Y - 3f);
                            Debug.ShowLine(portal, Color.Yellow);
                        }
#endif
                    }
                    // other is below
                    else if (navAreaPolygon.Bottom == otherNavAreaPolygon.Top)
                    {
#if o
                        D.WriteLine($"       Other below -- Touching this Bottom ({navAreaPolygon.Bottom}) - other Top ({otherNavAreaPolygon.Top})");
#endif
                        portal = _GetSegmentOverlap(navArea.EdgeBottom, otherNavArea.EdgeTop, true, true);
#if o
                        // Debug visuals
                        if (portal != null)
                        {
                            //_DrawDebugVisualForPortal(portal.Start.X - 3f, portal.Start.Y + 3f, portal.End.X + 3f, portal.End.Y + 3f);
                            Debug.ShowLine(portal, Color.Yellow);
                        }
#endif
                    }
                    // other is to left
                    else if (navAreaPolygon.Left == otherNavAreaPolygon.Right)
                    {
#if o
                        D.WriteLine($"       Other to left -- Touching this Left ({navAreaPolygon.Left}) - other Right ({otherNavAreaPolygon.Right})");
#endif
                        portal = _GetSegmentOverlap(navArea.EdgeLeft, otherNavArea.EdgeRight, false, false);
#if o
                        // Debug visuals
                        if (portal != null)
                        {
                            //_DrawDebugVisualForPortal(portal.Start.X + 3f, portal.Start.Y + 3f, portal.End.X + 3f, portal.End.Y - 3f);
                            Debug.ShowLine(portal, Color.Yellow);
                        }
#endif
                    }
                    // other is to right
                    else if (navAreaPolygon.Right == otherNavAreaPolygon.Left)
                    {
#if o
                        D.WriteLine($"       Other to right -- Touching this Right ({navAreaPolygon.Right}) - other Left ({otherNavAreaPolygon.Left})");
#endif
                        portal = _GetSegmentOverlap(navArea.EdgeRight, otherNavArea.EdgeLeft, false, true);
#if o
                        // Debug visuals
                        if (portal != null)
                        {
                            //_DrawDebugVisualForPortal(portal.Start.X - 3f, portal.Start.Y - 3f, portal.End.X - 3f, portal.End.Y + 3f);
                            Debug.ShowLine(portal, Color.Yellow);
                        }
#endif
                    }
                    // not touching
                    else
                    {
                        // Not actually touching => proceed to another navpoly
#if o
                        D.WriteLine($"       Not Touching");
#endif
                        continue;
                    }
                    #endregion - Using common sense END


                    if (portal != null) // this check IS needed
                    {
                        // Found portal between 2 NavAreas
                        // - have to add portal Node
                        portalNode = PortalNodeBase<TLink, TNode>.Create(portalId, navArea, otherNavArea, portal);
                        portalId++;
                        //
                        // - have to link this portal node to all other Portal Nodes in both this and other NavArea
                        //   BUT at this point I dont know all Portal Nodes in this or the other NavArea
                        //   => store both portal lines (sides) in this Portal Node
                        //   => store both NavAreas in this Portal Node
                        //   => do the linking at the end of NavMesh creation
                        //
                        //   - assign Portal Node to this NavArea
                        navArea.Portals.Add(portalNode);
                        //
                        //   - assign Portal Node to other NavArea
                        otherNavArea.Portals.Add(portalNode);
                        //

                        //navArea.LinkTo(otherNavArea, portal);

                        _PortalNodes.Add(portalNode);
#if o
                        #region    -- Debug / visuals
                        Debug.ShowText(ref portalNode.Position, portalNode.ID.ToString());

                        /*Debug.ShowLine(portal, Color.Yellow);
                        var circle = ShapeManager.AddCircle();
                        circle.Radius = 6f;
                        circle.Color = Color.Yellow;
                        circle.X = (float)portal.Start.X;
                        circle.Y = (float)portal.Start.Y;
                        Debug.ShowLine(navAreaPolygon.Position, otherNavAreaPolygon.Position, Debug.Gray32);*/
                        #endregion -- Debug / visuals END
#endif
                    }
#if o
                    else
                    { 
                        D.WriteLine($"       Not Touching");
                    
                    }
#endif

                }// for other j
            }// for one i 
            #endregion

            // Link Portal Nodes together inside each NavArea
            #region
            navArea = null;
            portalNode = null;
#if o
            TNode otherPortalNode = null;
            //D.WriteLine("========== Portals' Linking ==========");
#endif
            for (int i = 0; i < _NavAreas.Count; i++)
            {
                navArea = _NavAreas[i];
#if o
                //D.WriteLine("NavArea Portals: " + navArea.Portals.Count + " " + navArea.Polygon.Name);
#endif
                if (navArea.Portals.Count > 1)
                {
                    for (int iPortalNode = 0; iPortalNode < navArea.Portals.Count; iPortalNode++)
                    {
                        portalNode = navArea.Portals[iPortalNode]; // as TNode;
#if o
                        //D.WriteLine("  PortalNode " + portalNode.ID);
#endif
                        for (int iOtherPortalNode = iPortalNode + 1; iOtherPortalNode < navArea.Portals.Count; iOtherPortalNode++)
                        {
#if o
                            otherPortalNode = navArea.Portals[iOtherPortalNode]; // as TNode;

                            //D.WriteLine("    Links to " + otherPortalNode.ID);

                            portalNode.LinkTo(otherPortalNode, navArea);
#else
                            portalNode.LinkTo(navArea.Portals[iOtherPortalNode], navArea);
#endif
                        }
                    }
                }
            }
#if o
            //D.WriteLine("====================================\n");
#endif
            #endregion

            #region    -- Debug / visuals for links
#if o
            //D.WriteLine("========== Portals' Links ==========");
            Xna.Vector3 linkLineShift = new Xna.Vector3(2f, 2f, 0f);
            foreach (var navAreaa in NavAreas)
            {
                //D.WriteLine("NavArea " + navAreaa.Polygon.Name);

                foreach (var portalNodee in navAreaa.Portals)
                {
                    //D.WriteLine("  PortalNode " + portalNodee.ID);

                    foreach (var link in portalNodee.Links)
                    {
                        Debug.ShowLine(portalNodee.Position + linkLineShift, link.NodeLinkingTo.Position + linkLineShift, Debug.Gray32);

                        //D.WriteLine("    Links to " + link.NodeLinkingTo.ID);
                    }
                }
            }
            //D.WriteLine("====================================");
            
            /*// Debug
            var sb = new StringBuilder("--------------\n");
            PositionedNode cNode;
            foreach (var tNode in _NavPolygons)
            {
                cNode = tNode as PositionedNode;
                sb.Append(tNode.Polygon.Name).Append(" ").Append(cNode.CheckedAsMain).Append(" ").Append(cNode.CheckedAsOther)
                  .Append(" ").Append(tNode.Links.Count)
                  .AppendLine();
            }
            sb.AppendLine("--------------");
            D.WriteLine(sb.ToString());*/
#endif
            #endregion -- Debug visuals END
        }

        // v4 phaser-navmesh updated
        /// <summary>Check two collinear line segments to see if they overlap by sorting the points.</summary>
        /// <param name="line1">Polygon's edge</param>
        /// <param name="line2">Other polygon's edge</param>
        /// <returns>Line Segment of edges where they overlap or null id they don't overlap.</returns>
        /// <remarks>
        /// Algorithm source: http://stackoverflow.com/a/17152247
        /// </remarks>
        private SimpleLine _GetSegmentOverlap(SimpleLine line1, SimpleLine line2, bool horisontal, bool swapStartEnd)
        {
            var pointsOfLines = new RLineAndPoint[]
            {
                new RLineAndPoint( line1, line1.Start ),
                new RLineAndPoint( line1, line1.End ),
                new RLineAndPoint( line2, line2.Start ),
                new RLineAndPoint( line2, line2.End )
            };

            if (horisontal)
            {
                Array.Sort(
                    pointsOfLines,
                    (a, b) =>
                    {
                        // If lines have same Y, sort by X
                        if (a.Point.X < b.Point.X) return -1;
                        else if (a.Point.X > b.Point.X) return 1;
                        return 0;
                    }
                );
            }
            else
            {
                Array.Sort(
                    pointsOfLines,
                    (a, b) =>
                    {
                        // If lines have same X, sort by Y
                        if (a.Point.Y < b.Point.Y) return -1;
                        else if (a.Point.Y > b.Point.Y) return 1;
                        return 0;
                    }
                );
            }
            // If the first two points in the array come from the same line, no overlap
            bool noOverlap = pointsOfLines[0].Line == pointsOfLines[1].Line;
            // If the two middle points in the array are the same coordinates, then there is a
            // single point of overlap.
            bool singlePointOverlap = pointsOfLines[1].Point == pointsOfLines[2].Point;

            if (noOverlap || singlePointOverlap)
            {
                return null;
            }
            else
            {
                if (swapStartEnd)
                    return new SimpleLine(ref pointsOfLines[2].Point, ref pointsOfLines[1].Point);
                else
                    return new SimpleLine(ref pointsOfLines[1].Point, ref pointsOfLines[2].Point);
            }
        }

        public static SimpleLine _GetInvertedLine(SimpleLine line)
        {
            return new SimpleLine(line.End, line.Start);
        }

        #region    -- Debug permanent
        private void _DrawDebugVisualForPortal(double startX, double startY, double endX, double endY)
        {
            Dbg.ShowLine(startX, startY, endX, endY, Color.Yellow);
            var circle = ShapeManager.AddCircle();
            circle.Radius = 3f;
            circle.Color = Color.Yellow;
            circle.X = (float)startX;
            circle.Y = (float)startY;
        }
        #endregion -- Debug permanent END 

        #endregion --- Methods END
    }
}
