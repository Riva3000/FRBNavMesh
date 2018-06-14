using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static FRBNavMesh.JavasciptAstar.Astar;

namespace FRBNavMesh.JavasciptAstar
{
    class Graph<TNode>
    {
        List<GridNode> nodes;
        bool diagonal;
        List<List<GridNode>> grid;
        List<GridNode> dirtyNodes;

        public Graph(List<List<int>> gridIn, Options options)
        {
            //options = options || {};
            this.nodes = new List<GridNode>();
            this.diagonal = options.diagonal;
            this.grid = new List<List<GridNode>>();

            for (var x = 0; x < gridIn.Count; x++)
            {
                this.grid[x] = new List<GridNode>();

                List<int> row = gridIn[x];
                for (int y = 0; y < row.Count; y++)
                {
                    var node = new GridNode(x, y, row[y]);
                    this.grid[x][y] = node;
                    this.nodes.Add(node);
                }
            }

            this.init();
        }


        void init()
        {
            this.dirtyNodes = new List<GridNode>();  // [];

            for (var i = 0; i < this.nodes.Count; i++)
            {
                Astar.cleanNode(this.nodes[i]);
            }
        }

        public void cleanDirty()
        {
            for (var i = 0; i < this.dirtyNodes.Count; i++) {
                Astar.cleanNode(this.dirtyNodes[i]);
            }
            this.dirtyNodes = new List<GridNode>();
        }

        public void markDirty(GridNode node)
        {
            this.dirtyNodes.Add(node);
        }

        List<GridNode> neighbors(GridNode node) {
            var ret = new List<GridNode>(); // [];
            var x = node.x;
            var y = node.y;
            var grid = this.grid;

            // West
            if (grid[x - 1] != null && grid[x - 1][y] != null) {
                ret.Add(grid[x - 1][y]);
            }

            // East
            if (grid[x + 1] != null && grid[x + 1][y] != null) {
                ret.Add(grid[x + 1][y]);
            }

            // South
            if (grid[x] != null && grid[x][y - 1] != null) {
                ret.Add(grid[x][y - 1]);
            }

            // North
            if (grid[x] != null && grid[x][y + 1] != null) {
                ret.Add(grid[x][y + 1]);
            }

            if (this.diagonal) {
                // Southwest
                if (grid[x - 1] != null && grid[x - 1][y - 1] != null) {
                    ret.Add(grid[x - 1][y - 1]);
                }

                // Southeast
                if (grid[x + 1] != null && grid[x + 1][y - 1] != null) {
                    ret.Add(grid[x + 1][y - 1]);
                }

                // Northwest
                if (grid[x - 1] != null && grid[x - 1][y + 1] != null) {
                    ret.Add(grid[x - 1][y + 1]);
                }

                // Northeast
                if (grid[x + 1] != null && grid[x + 1][y + 1] != null) {
                    ret.Add(grid[x + 1][y + 1]);
                }
            }

            return ret;
        }

        public override string ToString()
        {
            var graphString = new StringBuilder();
            var nodes = this.grid;
            for (var x = 0; x < nodes.Count; x++) {
                var rowDebug = new StringBuilder();
                var row = nodes[x];
                for (var y = 0; y < row.Count; y++) {
                    rowDebug.Append(row[y].weight);
                }
                graphString.Append(rowDebug).Append(" ");
            }
            return graphString.Append("\n").ToString();
        }





    }
}
