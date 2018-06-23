using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRBNavMesh
{
    public class PortalNode : PortalNodeBase<Link, PortalNode>
    {
    }

    public class Link : LinkBase<Link, PortalNode>
    {
    }
}
