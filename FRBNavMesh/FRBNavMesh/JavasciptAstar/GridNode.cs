using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh.JavasciptAstar
{
    class GridNode
    {
        public int x;
        public int y;
        public int weight;  

        public int f = 0;
        public int g = 0;
        public double h = 0;
        public bool visited = false;
        public bool closed = false;
        public GridNode parent = null;



        public GridNode(float x, float y, int weight)
        {
            this.x = x;
            this.y = y;
            this.weight = weight;
        }



        public override string ToString()
        {
            return "GridNode[" + this.x + " " + this.y + "]";
        }

        public int getCost(GridNode fromNeighbor)
        {
            // Take diagonal weight into consideration.
            //if (fromNeighbor && fromNeighbor.x != this.x && fromNeighbor.y != this.y)
            if (fromNeighbor != null && fromNeighbor.x != this.x && fromNeighbor.y != this.y)
            {
                return (int)(this.weight * 1.41421f);
            }
            return this.weight;
        }

        public bool isWall()
        {
            return this.weight == 0;
        }


    }
}
