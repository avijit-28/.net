using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestrictionLinq
{
    internal class Graph
    {
        public int Id { get; set; }
        public List<int> Connections { get; set; }
        public static List<Graph> nodes = new List<Graph>
        {
            new Graph
            {
                Id = 1,
                Connections = new List<int> { 2, 3 }
            },
            new Graph
            {
                Id = 2,
                Connections = new List<int> { 1, 3, 4, 5, 6 }
            },
            new Graph
            {
                Id = 3,
                Connections = new List<int> { 1, 2, 4, 5 }
            },
            new Graph
            {
                Id = 4,
                Connections = new List<int> { 2, 3 }
            },
            new Graph
            {
                Id = 5,
                Connections = new List<int> { 2, 3, 6, 7 }
            },
            new Graph
            {
                Id = 6,
                Connections = new List<int> { 2, 5, 7 }
            },
            new Graph
            {
                Id = 7,
                Connections = new List<int> { 5, 6, 8, 9, 10 }
            }
        };
    }
}
    
