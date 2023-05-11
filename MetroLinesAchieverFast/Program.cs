using System;
using System.Collections.Generic;
using System.Text;
using MetroLinesAchieverFast.Entities;
using MetroLinesAchieverFast.GraphFactories;
using MetroLinesAchieverFast.PathFinders;

namespace MetroLinesAchieverFast
{
    public class Program
    {
        public static void Main()
        {
            IMetroGraphFactory factory = new MoscowMetroGraphFactory();
            IAchieveAllMetroLinesPathFinder linesPathFinder = new AchieveAllMetroLinesPathFinderArtemSmirnov();
            var graph = factory.Create();
            var shortestPath = linesPathFinder.FindShortestPath(graph, "Отрадное");
            PrintPath(shortestPath);
        }

        private static void PrintPath(IEnumerable<MetroStation> _Path)
        {
            if (_Path == null)
            {
                Console.WriteLine("Путь не найден");
                return;
            }
            var sb = new StringBuilder();
            sb.AppendLine("Путь: ");
            foreach (var pathItem in _Path)
                sb.AppendLine(pathItem.StationName);
            Console.WriteLine(sb.ToString());
        }
    }
}