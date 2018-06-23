﻿//#define RemovedForTesting

using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xna = Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

using RCommonFRB;

using System.Diagnostics;
using D = System.Diagnostics.Debug;
using Microsoft.Xna.Framework.Graphics;
//using FDebug = FRBNavMesh.Debug;

namespace FRBNavMesh
{
    //using FDebug = FRBNavMesh.Debug;

    /*
    * The workhorse that represents a navigation mesh built from a series of polygons. Once built, the
    * mesh can be asked for a path from one point to another point. It has debug methods for 
    * visualizing paths and visualizing the individual polygons. Some internal terminology usage:
    * 
    * - neighbor: a polygon that shares part of an edge with another polygon
    * - portal: when two neighbor's have edges that overlap, the portal is the overlapping line segment
    * - channel: the path of polygons from starting point to end point
    * - pull the string: run the funnel algorithm on the channel so that the path hugs the edges of the
    *   channel. Equivalent to having a string snaking through a hallway and then pulling it taut.
    */

    public class RLineAndPoint
    {
        public SimpleLine line;
        public Point point;
    }

    /// <summary>
    /// R: Almost FIN
    /// </summary>
    public class NavMesh<TNode, TLink>
        where TNode : PositionedNodeBase<TLink, TNode>, new()
        where TLink : LinkBase<TLink, TNode>, new()
    {
        public readonly List< NavArea<TNode, TLink> > NavAreas;
        public readonly List<TNode> PortalNodes;

        #region    -- A*
        // This reduces memory allocation during runtime and also reduces the argument list size
        protected List<TNode> _ClosedList = new List<TNode>(30);
        protected List<TNode> _OpenList = new List<TNode>(30);

        protected float mShortestPath;
        #endregion -- A* END






        /// <summary></summary>
        /// <param name="polygons"></param>
        /// <param name="meshShrinkAmount">The amount (in pixels) that the navmesh has been
        /// shrunk around obstacles (a.k.a the amount obstacles have been expanded)</param>
        public NavMesh(IList<AxisAlignedRectangle> polygons /*, int meshShrinkAmount = 0*/)
        {
            //_meshShrinkAmount = meshShrinkAmount;

            // Construct NavArea instances for each polygon
            NavAreas = new List<NavArea<TNode,TLink>>(polygons.Count);
            for (int i = 0; i < polygons.Count; i++)
            {
                NavAreas.Add( new NavArea<TNode,TLink>(polygons[i], i) );
            }

            PortalNodes = new List<TNode>(polygons.Count * 3);

            _CalculateNeighbors();
        }






        /*
        * Find a path from the start point to the end point using this nav mesh.
        *
        * @param {Phaser.Point} startPoint
        * @param {Phaser.Point} endPoint
        * @param {object} [drawOptions={}] Options for controlling debug drawing
        * @param {boolean} [drawOptions.drawPolyPath=false] Whether or not to visualize the path
        * through the polygons - e.g. the path that astar found.
        * @param {boolean} [drawOptions.drawFinalPath=false] Whether or not to visualize the path
        * through the path that was returned.
        * @returns {Phaser.Point[]|null} An array of points if a path is found, or null if no path
        *
        * @memberof NavMesh
        */
        /// <summary>Find a path from the start point to the end point using this nav mesh.</summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="drawPolyPath">Whether or not to visualize the path through the polygons - e.g. the path that astar found.</param>
        /// <param name="drawFinalPath">Whether or not to visualize the path through the path that was returned.</param>
        /// <returns>An array of points and nodes if a path is found, or nulls if no path</returns>
        public List<Point> FindPath(Point startPoint, Point endPoint, out List<TNode> nodesPath, bool drawPolyPath = false, bool drawFinalPath = false)
        {
#if true
            #region    --- Find the closest poly for the starting and ending point
            NavArea<TNode, TLink> startArea = FindNavRectFromPoint(ref startPoint);
            NavArea<TNode, TLink> endArea = FindNavRectFromPoint(ref endPoint);

            #region    If the start point wasn't inside a polygon, run a more liberal check that allows a point
            /*
                // If the start point wasn't inside a polygon, run a more liberal check that allows a point
                // to be within meshShrinkAmount radius of a polygon
                if (!startPoly && this._meshShrinkAmount > 0)
                {
                    for (const navPoly of this._navPolygons) 
                    {
                        // Check if point is within bounding circle to avoid extra projection calculations
                        r = navPoly.boundingRadius + this._meshShrinkAmount;
                        d = navPoly.centroid.distance(startPoint);
                        if (d <= r) {
                            // Check if projected point is within range of a polgyon and is closer than the
                            // previous point
                            const { distance } = this._projectPointToPolygon(startPoint, navPoly);
                            if (distance <= this._meshShrinkAmount && distance < startDistance) {
                                startPoly = navPoly;
                                startDistance = distance;
                            }
                        }
                    }
                }
                */

            /*
            // Same check as above, but for the end point
            if (!endPoly && this._meshShrinkAmount > 0) {
                for (const navPoly of this._navPolygons) {
                    r = navPoly.boundingRadius + this._meshShrinkAmount;
                    d = navPoly.centroid.distance(endPoint);
                    if (d <= r) {
                        const { distance } = this._projectPointToPolygon(endPoint, navPoly);
                        if (distance <= this._meshShrinkAmount && distance < endDistance) {
                            endPoly = navPoly;
                            endDistance = distance;
                        }
                    }
                }
            }
            */
            #endregion If the start point wasn't inside a polygon, run a more liberal check that allows a point END

            // No matching polygons locations for the start or end, so no path found
            // R: = start or end point not on nav mesh
            if (startArea == null || endArea == null)
            {
                nodesPath = null;
                return null;
            }

            // If the start and end polygons are the same, return a direct path
            if (startArea == endArea)
            {
                List<Point> pointsPath = new List<Point> { startPoint, endPoint };
                //if (drawFinalPath) this.debugDrawPath(phaserPath, 0xffd900, 10);
                nodesPath = null; // not traversing any Portal Nodes
                return pointsPath;
            }
            #endregion --- Find the closest poly for the starting and ending point END



            #region    --- A* search
            // --- Search!
            /*NavPoly[] astarPath = JavasciptAstar.Astar.search(
                                        this._Graph, startPoly, endPoly,
                                        new JavasciptAstar.Astar.Options<NavPoly> { heuristic = this._Graph.navHeuristic }
                                  );*/
            nodesPath = new List<TNode>();
            GetPath(
                PositionedNodeBase<TLink, TNode>.CreateFakeNodeDebug(ref startPoint, startArea.Portals, -1, false, startArea),
                PositionedNodeBase<TLink, TNode>.CreateFakeNodeDebug(ref endPoint, endArea.Portals, endArea.ID, true, endArea), 
                nodesPath
            );

            if (nodesPath.Count == 0)
            {
                // While the start and end polygons may be valid, no path between them

                nodesPath = null;
                return null;
            }
            #endregion --- A* search END



            #region    --- Funnel algorithm
            // We have a path, so now time for the funnel algorithm
            D.WriteLine("======== Path search ========");
            D.WriteLine("  --- Path ---");
            foreach (var pathNode in nodesPath)
            {
                D.WriteLine("    " + pathNode.ID);
            }
            D.WriteLine("  --- Channel ---");
            Channel channel = new Channel();
            channel.Add(startPoint);

            SimpleLine portal;
            int countLimit = nodesPath.Count - 1;
            TNode pathPortalNode;
            TNode nextPathPortalNode;
            for (int i = 1; i < countLimit; i++) // skipping first Node - I don't need it's Links - I don't need their Portals
            {
                pathPortalNode = nodesPath[i];
                nextPathPortalNode = nodesPath[i + 1];

                D.WriteLine($"    pathPortalNode: {pathPortalNode.ID} and pathPortalNode: {nextPathPortalNode.ID}");

                // Find the portal
                portal = null;
                /*SimpleLine portal = null;
                for (int j = 0; j < navPolygon.Neighbors.Count; j++)
                {
                    if (navPolygon.Neighbors[j].Id == nextNavPolygon.Id)
                    {
                        portal = navPolygon.Portals[j];
                    }
                }*/
                //nextNavPolygon.mParentNode
                foreach (var link in pathPortalNode.Links)
                {
                    //if ( link.NodeLinkingTo.ReferenceEquals(nextPathPortalNode) )
                    if ( link.NodeLinkingTo.ID == nextPathPortalNode.ID )
                    {
                        D.WriteLine($"      link to {link.NodeLinkingTo.ID} - link found and portal added");

                        portal = link.Portal;
                        // Push the portal vertices into the channel
                        channel.Add(portal.Start, portal.End);
                        break;
                    }
                    else
                    {
                        D.WriteLine($"      link to {link.NodeLinkingTo.ID} - link NOT found !");
                    }
                }
            }

            channel.Add(endPoint);


            // Pull a string along the channel to run the funnel
            channel.StringPull();

            PositionedNodeBase<TLink, TNode>.CleanupFakeEndNode(endArea.Portals);

            // Clone path, excluding duplicates
            Point? lastPoint = null;
            List<Point> finalPointsPath = new List<Point>();
            foreach (var p in channel.Path)
            {
                //var newPoint = p.clone();
                var newPoint = p;
                //if (!lastPoint || !newPoint.equals(lastPoint)) 
                if (lastPoint.HasValue == false || newPoint != lastPoint)
                    finalPointsPath.Add(newPoint);
                lastPoint = newPoint;
            }

            return finalPointsPath;
            #endregion --- Funnel algorithm END
#else
            nodesPath = null;
            return null;
#endif
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
            foreach (var navArea in NavAreas)
            {
                r = navArea.Polygon.BoundingRadius;
                // Start
                d = RCommonFRB.Geometry.Distance2D(ref navArea.Polygon.Position, ref point);
                if (d <= bestDistance && d <= r && navArea.Polygon.IsPointInside( (float)point.X, (float)point.Y) ) // @
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

                mShortestPath = float.PositiveInfinity;
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

                    if (cost < mShortestPath)
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
                                mShortestPath = nodeLinkingTo.mCostToGetHere;
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
            D.WriteLine(" * NavMesh._CalculateNeighbors()");

            // Fill out the neighbor information for each NavArea
            // Find and create Portals (Portal Nodes) for each NavArea
            #region    
            NavArea<TNode, TLink> navArea;
            NavArea<TNode, TLink> otherNavArea;
            AxisAlignedRectangle navAreaPolygon;
            AxisAlignedRectangle otherNavAreaPolygon;
            TNode portalNode;
            TNode otherPortalNode;
            SimpleLine portal;
            int portalId = 1;
            for (int i = 0; i < NavAreas.Count; i++)
            {
                navArea = NavAreas[i];
                navAreaPolygon = navArea.Polygon;

                D.WriteLine("   navArea: " + navAreaPolygon.Name);
                //(navArea as PositionedNode).CheckedAsMain = true;

                for (int j = i + 1; j < NavAreas.Count; j++)
                {
                    otherNavArea = NavAreas[j];
                    otherNavAreaPolygon = otherNavArea.Polygon;

                    D.WriteLine("     otherNavPoly: " + otherNavAreaPolygon.Name);

                    #region    - Check polygons distance
                    // Check if the other navpoly is within range to touch
                    // Distance between centers
                    var distanceBetweenCenters = RCommonFRB.Geometry.Distance2D(ref navAreaPolygon.Position, ref otherNavAreaPolygon.Position);
                    // If Distance between centers is bigger than combined radii, they are not in range
                    // If Distance between centers is smaller or equal, they are in range (not necessarily touching)
                    if (distanceBetweenCenters >= navAreaPolygon.BoundingRadius + otherNavAreaPolygon.BoundingRadius)
                    {
                        // Not in range => proceed to another navpoly
                        D.WriteLine($"       Not in range (distanceBetweenCenters: {distanceBetweenCenters} totalRadii: {navAreaPolygon.BoundingRadius + otherNavAreaPolygon.BoundingRadius})");
                        continue;
                    }

                    D.WriteLine($"       In range (distanceBetweenCenters: {distanceBetweenCenters} totalRadii: {navAreaPolygon.BoundingRadius + otherNavAreaPolygon.BoundingRadius})");
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
                        D.WriteLine($"       Other above -- Touching this Top ({navAreaPolygon.Top}) - other Bottom ({otherNavAreaPolygon.Bottom})");

                        portal = _GetSegmentOverlap(navArea.EdgeTop, otherNavArea.EdgeBottom, true, false);

                        // Debug visuals
                        if (portal != null)
                        {
                            //_DrawDebugVisualForPortal(portal.Start.X + 3f, portal.Start.Y - 3f, portal.End.X - 3f, portal.End.Y - 3f);
                            Debug.ShowLine(portal, Color.Yellow);
                        }
                    }
                    // other is below
                    else if (navAreaPolygon.Bottom == otherNavAreaPolygon.Top)
                    {
                        D.WriteLine($"       Other below -- Touching this Bottom ({navAreaPolygon.Bottom}) - other Top ({otherNavAreaPolygon.Top})");

                        portal = _GetSegmentOverlap(navArea.EdgeBottom, otherNavArea.EdgeTop, true, true);

                        // Debug visuals
                        if (portal != null)
                        {
                            //_DrawDebugVisualForPortal(portal.Start.X - 3f, portal.Start.Y + 3f, portal.End.X + 3f, portal.End.Y + 3f);
                            Debug.ShowLine(portal, Color.Yellow);
                        }
                    }
                    // other is to left
                    else if (navAreaPolygon.Left == otherNavAreaPolygon.Right)
                    {
                        D.WriteLine($"       Other to left -- Touching this Left ({navAreaPolygon.Left}) - other Right ({otherNavAreaPolygon.Right})");

                        portal = _GetSegmentOverlap(navArea.EdgeLeft, otherNavArea.EdgeRight, false, false);

                        // Debug visuals
                        if (portal != null)
                        {
                            //_DrawDebugVisualForPortal(portal.Start.X + 3f, portal.Start.Y + 3f, portal.End.X + 3f, portal.End.Y - 3f);
                            Debug.ShowLine(portal, Color.Yellow);
                        }
                    }
                    // other is to right
                    else if (navAreaPolygon.Right == otherNavAreaPolygon.Left)
                    {
                        D.WriteLine($"       Other to right -- Touching this Right ({navAreaPolygon.Right}) - other Left ({otherNavAreaPolygon.Left})");

                        portal = _GetSegmentOverlap(navArea.EdgeRight, otherNavArea.EdgeLeft, false, true);

                        // Debug visuals
                        if (portal != null)
                        {
                            //_DrawDebugVisualForPortal(portal.Start.X - 3f, portal.Start.Y - 3f, portal.End.X - 3f, portal.End.Y + 3f);
                            Debug.ShowLine(portal, Color.Yellow);
                        }
                    }
                    // not touching
                    else
                    {
                        // Not actually touching => proceed to another navpoly
                        D.WriteLine($"       Not Touching");
                        continue;
                    }
                    #endregion - Using common sense END


                    if (portal != null) // this check IS needed
                    {
                        // Found portal between 2 NavAreas
                        // - have to add portal Node
                        portalNode = PositionedNodeBase<TLink, TNode>.Create(portalId, navArea, otherNavArea, portal);
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

                    #region    -- Debug / visuals
                        Debug.ShowText(ref portalNode.Position, portalNode.ID.ToString());

                        /*Debug.ShowLine(portal, Color.Yellow);
                        var circle = ShapeManager.AddCircle();
                        circle.Radius = 6f;
                        circle.Color = Color.Yellow;
                        circle.X = (float)portal.Start.X;
                        circle.Y = (float)portal.Start.Y;
                        Debug.ShowLine(navAreaPolygon.Position, otherNavAreaPolygon.Position, Debug.Gray32);*/
                    }
                    else
                    { 
                        D.WriteLine($"       Not Touching");
                    #endregion -- Debug / visuals END
                    }

                }// for other j
            }// for one i 
            #endregion

            // Link Portal Nodes together inside each NavArea
            #region
            navArea = null;
            portalNode = null;
            otherPortalNode = null;
            //D.WriteLine("========== Portals' Linking ==========");
            for (int i = 0; i < NavAreas.Count; i++)
            {
                navArea = NavAreas[i];

                //D.WriteLine("NavArea Portals: " + navArea.Portals.Count + " " + navArea.Polygon.Name);

                if (navArea.Portals.Count > 1)
                {
                    for (int iPortalNode = 0; iPortalNode < navArea.Portals.Count; iPortalNode++)
                    {
                        portalNode = navArea.Portals[iPortalNode]; // as TNode;

                        //D.WriteLine("  PortalNode " + portalNode.ID);

                        for (int iOtherPortalNode = iPortalNode + 1; iOtherPortalNode < navArea.Portals.Count; iOtherPortalNode++)
                        {
                            otherPortalNode = navArea.Portals[iOtherPortalNode]; // as TNode;

                            //D.WriteLine("    Links to " + otherPortalNode.ID);

                            portalNode.LinkTo(otherPortalNode, navArea);
                        }
                    }
                }
            }
            //D.WriteLine("====================================\n");
            #endregion

            #region    -- Debug / visuals for links
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
            #endregion -- Debug visuals END
        }

        private void _DrawDebugVisualForPortal(double startX, double startY, double endX, double endY)
        {
            Debug.ShowLine(startX, startY, endX, endY, Color.Yellow);
            var circle = ShapeManager.AddCircle();
            circle.Radius = 3f;
            circle.Color = Color.Yellow;
            circle.X = (float)startX;
            circle.Y = (float)startY;
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
                new RLineAndPoint { line = line1, point = line1.Start },
                new RLineAndPoint { line = line1, point = line1.End },
                new RLineAndPoint { line = line2, point = line2.Start },
                new RLineAndPoint { line = line2, point = line2.End }
            };

            if (horisontal)
            {
                Array.Sort(
                    pointsOfLines,
                    (a, b) =>
                    {
                        // If lines have same Y, sort by X
                        if (a.point.X < b.point.X) return -1;
                        else if (a.point.X > b.point.X) return 1;
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
                        if (a.point.Y < b.point.Y) return -1;
                        else if (a.point.Y > b.point.Y) return 1;
                        return 0;
                    }
                );
            }
            // If the first two points in the array come from the same line, no overlap
            bool noOverlap = pointsOfLines[0].line == pointsOfLines[1].line;
            // If the two middle points in the array are the same coordinates, then there is a
            // single point of overlap.
            bool singlePointOverlap = pointsOfLines[1].point == pointsOfLines[2].point;

            if (noOverlap || singlePointOverlap)
            {
                return null;
            }
            else
            {
                if (swapStartEnd)
                    return new SimpleLine(pointsOfLines[2].point, pointsOfLines[1].point);
                else
                    return new SimpleLine(pointsOfLines[1].point, pointsOfLines[2].point);
            }
        }

        /*/// <summary>Project a point onto a polygon in the shortest distance possible.</summary>
        /// <param name="point">The point to project</param>
        /// <param name="navPoly">The navigation polygon to test against</param>
        /// <returns></returns>
        private Tuple<Point?, float> _ProjectPointToPolygon(Point point, NavPoly navPoly)
        {
            Point? closestProjection = null;
            var closestDistance = float.MaxValue;
            foreach (var edge in navPoly.Edges) {
                var projectedPoint = this._ProjectPointToEdge(point, edge);
                //var d = point.distance(projectedPoint);
                var d = (float)RCommonFRB.Geometry.Distance(ref point, ref projectedPoint);
                if (closestProjection == null || d < closestDistance) {
                    closestDistance = d;
                    closestProjection = projectedPoint;
                }
            }
            return Tuple.Create(closestProjection, closestDistance);
        }*/

        /// <summary>Distance</summary>
        /// <returns>Distance between two 2D points</returns>
        private double _DistanceSquared(Point a, Point b) {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            return dx * dx + dy * dy;
        }

        /// <summary>
        /// Project a point onto a line segment
        /// JS Source: http://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment</summary>
        /// </summary>
        private Point _ProjectPointToEdge(Point point, SimpleLine line) {
            Point a = line.Start;
            Point b = line.End;
            // Consider the parametric equation for the edge's line, p = a + t (b - a). We want to find
            // where our point lies on the line by solving for t:
            //  t = [(p-a) . (b-a)] / |b-a|^2
            double l2 = this._DistanceSquared(a, b);
            var t = ((point.X - a.X) * (b.X - a.X) + (point.Y - a.Y) * (b.Y - a.Y)) / l2;
            // We clamp t from [0,1] to handle points outside the segment vw.
            //t = Phaser.Math.clamp(t, 0, 1);
            t = RUtils.Clamp(t, 0, 1);
            // Project onto the segment
            var p = new Point(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y));
            return p;
        }

        public static SimpleLine _GetInvertedLine(SimpleLine line)
        {
            return new SimpleLine(line.End, line.Start);
        }

        #region    --- Debug permanent
        #endregion --- Debug permanent END
    }
}
