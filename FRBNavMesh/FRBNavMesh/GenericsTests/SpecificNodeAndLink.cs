﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRBNavMesh
{
    public class PositionedNode : PositionedNodeBase<Link, PositionedNode>
    {
    }

    public class Link : LinkBase<Link, PositionedNode>
    {
    }
}