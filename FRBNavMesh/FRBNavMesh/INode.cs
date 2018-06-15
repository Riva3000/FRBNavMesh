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
    public interface INode<ITLink> : IStaticPositionable //, INameable
    {
        //* IStaticPositionable signatures don't have to be included here

        //string Name { get; set; }
        ReadOnlyCollection<ITLink> Links { get; }
        bool Active { get; set; }
        int Id { get; }
        AxisAlignedRectangle Polygon { get; set; }
        //Vector3 Position { get; set; }
        //Vector3 Position;
        Line[] Edges { get; }




        ITLink GetLinkTo(INode<ITLink> node);
        bool IsLinkedTo(INode<ITLink> node);
        void LinkTo(INode<ITLink> nodeToLinkTo);
        void LinkTo(INode<ITLink> nodeToLinkTo, float costTo);
        //void LinkTo(IPositionedNode nodeToLinkTo, float costTo, float costFrom);

    }
}
