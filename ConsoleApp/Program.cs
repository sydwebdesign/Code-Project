using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    public class Node
    {
        public string Name { get; set; }
        
        public int GatePointerIndex { get; set; } = new Random().Next(2);
        
        public List<Node> ChildrenNodes { get; set; } = new List<Node>();
        
        public int Weight { get; set; }
    }
    
    class Program
    {
        static Node ConstructNodeTree(int maxDepth, out List<Node> containerList)
        {
            containerList = new List<Node>();
            var root = new Node { Name = "Root" };
            var workSet = new List<Node>();
            workSet.Add(root);
            for (var currentDepth = 1; currentDepth <= maxDepth; currentDepth++)
            {
                for (var i = 0; i < workSet.Count; i++)
                {
                    if (currentDepth < maxDepth)
                    {
                        workSet[i].ChildrenNodes.AddRange(new[] {new Node(), new Node()});
                    }
                    else
                    {
                        workSet[i].ChildrenNodes.AddRange(new[]
                        {
                            new Node { Name = $"{Convert.ToChar('A' + i * 2)}" }, 
                            new Node { Name = $"{Convert.ToChar('A' + i * 2 + 1)}" }
                        });
                        containerList.AddRange(workSet[i].ChildrenNodes);
                    }
                }
                
                workSet = workSet.SelectMany(ws => ws.ChildrenNodes).ToList();
            }

            return root;
        }

        static List<Node> PredictEmptyContainer(Node root, int numberOfBalls)
        {
            var emptyContainers = new List<Node>();
            root.Weight = numberOfBalls;
            var workSet = new Queue<Node>();
            workSet.Enqueue(root);
            while (workSet.Count > 0)
            {
                var currentParentNode = workSet.Dequeue();
                var residualWeight = currentParentNode.Weight;
                var nextPointerIndex = currentParentNode.GatePointerIndex;
                while (residualWeight > 0)
                {
                    residualWeight--;
                    currentParentNode.ChildrenNodes[nextPointerIndex++ % 2].Weight++;
                }
                currentParentNode.ChildrenNodes.ForEach(node =>
                {
                    if (node.ChildrenNodes.Any())
                    {
                        workSet.Enqueue(node);
                    }
                    else if (node.Weight == 0)
                    {
                        Console.WriteLine($"Container {node.Name} will have no ball");
                        emptyContainers.Add(node);
                    }
                });
            }

            return emptyContainers;
        }

        static List<Node> DropBalls(Node root, int numberOFBalls)
        {
            var containerWithBalls = new List<Node>();
            for (int i = 0; i < numberOFBalls; i++)
            {
                var currentNode = root;
                while (currentNode.ChildrenNodes.Any())
                {
                    currentNode = currentNode.ChildrenNodes[currentNode.GatePointerIndex++ % 2];
                }
                Console.WriteLine($"{currentNode.Name} received ball");
                containerWithBalls.Add(currentNode);
                
            }

            return containerWithBalls;
        }
        
        static void Main(string[] args)
        {
            var root = ConstructNodeTree(4, out var containerList);
            var emptyContainers = PredictEmptyContainer(root, 15);
            var containerWithBalls = DropBalls(root, 15);
            var containerNameList = string.Join("", containerList.Select(node => node.Name).OrderBy(s => s));
            var check = string.Join("", emptyContainers.Select(node => node.Name).Union(containerWithBalls.Select(node => node.Name)).OrderBy(s => s)) == containerNameList;
            Console.WriteLine($"Check Pass: {check}");
        }
    }
}