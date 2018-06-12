using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall.TileGraphics;
using FlatRedBall.Math.Geometry;

namespace FRBNavMesh
{
    class NavMeshPlugin
    {
        List<NavMesh> _navMeshes = new List<NavMesh>();

        /*
        * Load a navmesh from Tiled and switch it to be the current navmesh. Currently assumes that the
        * polygons are squares!
        * 
        * @param {Phaser.Tilemap} tilemap The tilemap that contains polygons under an object layer
        * @param {string} objectKey The name of the object layer in the tilemap
        * @param {number} [meshShrinkAmount=0] The amount (in pixels) that the navmesh has been
        * shrunk around obstacles (a.k.a the amount obstacles have been expanded)
        * 
        * @memberof NavMeshPlugin
        */

        /// <summary>
        /// Load a navmesh from Tiled and switch it to be the current navmesh. 
        /// Currently assumes that the polygons are squares!
        /// </summary>
        /// <param name="tilemap">The tilemap that contains polygons under an object layer</param>
        /// <param name="objectLayerName">The name of the object layer in the tilemap</param>
        /// <param name="meshShrinkAmount">The amount (in pixels) that the navmesh has been
        /// shrunk around obstacles (a.k.a the amount obstacles have been expanded)</param>
        /// <returns></returns>
        NavMesh buildMeshFromTiled(LayeredTileMap tilemap, string objectLayerName, int meshShrinkAmount = 0)
        {
            // Load up the object layer
            ShapeCollection objectLayer = tilemap.ShapeCollections.FirstOrDefault(layer => layer.Name == objectLayerName);

            if (objectLayer == null)
                throw new Exception($"NavMeshPlugin: The given tilemap has no object layer with the name \"{objectLayerName}\"");
            else if (objectLayer.AxisAlignedRectangles.Count == 0)
                throw new Exception($"NavMeshPlugin: The \"{objectLayerName}\" object layer in the Tilemap has 0 objects in it");

            // Loop over the objects and construct a polygon
            /*
            const polygons = [];
            for (const r of rects) 
            {
                const top = r.y;
                const bottom = r.y + r.height;
                const left = r.x;
                const right = r.x + r.width;
                const poly = new Phaser.Polygon(left, top, left, bottom, right, bottom, right, top);
                polygons.push(poly);
            }
            */

            // Build the navmesh
            // R: FRB version v1 - not sure if right - vertices order may be important
            var mesh = new NavMesh(null, objectLayer.AxisAlignedRectangles.ToList(), meshShrinkAmount);
            _navMeshes.Add(mesh);
            return mesh;
        }
    }
}
