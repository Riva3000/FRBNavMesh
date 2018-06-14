using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    /*
    * Graph for javascript-astar. It implements the functionality for astar. See GPS test from astar
    * repo for structure: https://github.com/bgrins/javascript-astar/blob/master/test/tests.js
    *
    * @class NavGraph 
    * @private
    */
    /// <summary>
    /// R: FIN
    /// <para>Graph for javascript-astar. It implements the functionality for astar. See GPS test from astar</para>
    /// </summary>
    /// <remarks>
    /// repo for structure: https://github.com/bgrins/javascript-astar/blob/master/test/tests.js
    /// </remarks>
    class NavGraph
    {
        public List<NavPoly> nodes;



        public NavGraph(List<NavPoly> navPolygons)
        {
            this.nodes = navPolygons;
        }



        public List<NavPoly> neighbors(NavPoly navPolygon)
        {
            return navPolygon.Neighbors;
        }

        public double navHeuristic(NavPoly navPolygon1, NavPoly navPolygon2)
        {
            return navPolygon1.centroidDistance(navPolygon2);
        }

    }
}
