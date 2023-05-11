using System.Collections.Generic;
using MetroLinesAchieverFast.Entities;

namespace MetroLinesAchieverFast.PathFinders
{
    public interface IAchieveAllMetroLinesPathFinder
    {
        List<MetroStation> FindShortestPath(Graph _Graph, string _Start);
    }
}