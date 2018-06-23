using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FRBNavMesh;
using FlatRedBall.Math.Geometry;
using FlatRedBall.TileGraphics;


namespace FRBNavMesh
{
    public class TiledExtensions
    {
        /// <summary>
        /// Load a navmesh from Tiled and switch it to be the current navmesh. 
        /// Currently assumes that the polygons are squares!
        /// </summary>
        /// <param name="tilemap">The tilemap that contains polygons under an object layer</param>
        /// <param name="objectLayerName">The name of the object layer in the tilemap</param>
        /// <param name="meshShrinkAmount">The amount (in pixels) that the navmesh has been
        /// shrunk around obstacles (a.k.a the amount obstacles have been expanded)</param>
        /// <returns></returns>
        public static NavMesh<TNode, TLink> BuildNavMeshFromTiled<TNode, TLink>(LayeredTileMap tilemap, string objectLayerName /*, int meshShrinkAmount = 0*/)
            where TNode : PortalNodeBase<TLink, TNode>, new()
            where TLink : LinkBase<TLink, TNode>, new()
        {
            // Load up the object layer
            ShapeCollection objectLayer = tilemap.ShapeCollections.FirstOrDefault(layer => layer.Name == objectLayerName);

            if (objectLayer == null)
                throw new Exception($"NavMeshPlugin: The given tilemap has no object layer with the name \"{objectLayerName}\"");
            else if (objectLayer.AxisAlignedRectangles.Count == 0)
                throw new Exception($"NavMeshPlugin: The \"{objectLayerName}\" object layer in the Tilemap has 0 objects in it");

            // Build the navmesh
            // R: FRB version v1 - not sure if right - vertices order may be important
            return new NavMesh<TNode, TLink>(objectLayer.AxisAlignedRectangles);
        }
    }
}
