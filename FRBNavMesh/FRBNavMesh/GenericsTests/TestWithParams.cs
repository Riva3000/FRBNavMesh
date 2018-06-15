using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh.GenericsTests
{
    class TestWithParams
    {
        void Test()
        {
            var link = LinkWithParams.Create(null, 0f);
            link.IsStairs = true;

            var node = new PositionedNodeWithParams();
            node.IsStairs = true;
        }
    }
}
