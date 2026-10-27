using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestrictionLinq
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var result = Graph.nodes
                .Where(n => n.Connections.Count > 4)
                .Select(n => new
                {
                    Node = n.Id,
                    degree = n.Connections.Count
                });

            foreach (var node in result)
            {
                Console.WriteLine($"Node {node.Node} - Degree: {node.degree}");
            }

        }
    }
}
