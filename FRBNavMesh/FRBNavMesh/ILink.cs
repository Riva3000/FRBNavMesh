﻿using FlatRedBall.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh
{
    public interface ILink<ITNode> //: IEquatable<ILink>
    {
        ITNode NodeLinkingTo { get; set; }
        Line Portal { get; }
        float Cost { get; set; }
        bool Active { get; set; }        
        

        //Link(IPositionedNode nodeLinkingTo, float cost);


    }
}