using FlatRedBall.Math;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    public interface IPositionedNode : IStaticPositionable //, INameable
    {
        //* IStaticPositionable signatures don't have to be included here

        //string Name { get; set; }
        ReadOnlyCollection<ILink> Links { get; }
        bool Active { get; set; }
        int Id { get; }
        AxisAlignedRectangle Polygon { get; set; }
        //Vector3 Position { get; set; }
        //Vector3 Position;
        Line[] Edges { get; }




        ILink GetLinkTo(IPositionedNode node);
        bool IsLinkedTo(IPositionedNode node);
        void LinkTo(IPositionedNode nodeToLinkTo);
        void LinkTo(IPositionedNode nodeToLinkTo, float costTo);
        //void LinkTo(IPositionedNode nodeToLinkTo, float costTo, float costFrom);

    }
}
