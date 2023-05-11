using System;
using System.Collections.Generic;
using System.Linq;
using MetroLinesAchieverFast.Entities;

namespace MetroLinesAchieverFast.PathFinders
{
    public class AchieveAllMetroLinesPathFinderArtemSmirnov : IAchieveAllMetroLinesPathFinder
    {
        #region types

        private class PathWithValidity
        {
            public List<Vertex> Path  { get; set; }
            public bool         Valid { get; set; }
        }

        #endregion
        
        #region nonpublic members
        
        private List<PathWithValidity> m_PathsV = new List<PathWithValidity>();

        private List<string> m_LineIds;
        private Graph        m_Graph;
        private int          m_StationsCount;

        #endregion

        #region api

        public List<MetroStation> FindShortestPath(Graph _Graph, string _Start)
        {
            m_Graph = _Graph;
            m_StationsCount = _Graph.Edges
                .SelectMany(_E => new[] {_E.Start, _E.End})
                .Distinct()
                .Count();
            m_LineIds = _Graph.Edges
                .SelectMany(_E => new[] {_E.Start, _E.End})
                .Select(_S => _S.LineId)
                .Distinct()
                .ToList();
            var startPathV = new PathWithValidity {Path = new List<Vertex>()};
            var startStation = _Graph.Edges
                .First(_E => _E.Start.StationName == _Start).Start;
            var startVertex = CreateNewVertex(startStation);
            startPathV.Path.Add(startVertex);
            m_PathsV.Add(startPathV);
            FindAllPaths(startPathV, startVertex);
            var shortestPath = FindShortestPathCore();
            return shortestPath == null ? null :
                shortestPath.Path.Select(_V => _V.Station)
                .ToList();
        }

        #endregion

        #region nonpublic methods
        
        private void FindAllPaths(PathWithValidity _PathV, Vertex _LastVertex)
        {
            if (_LastVertex.Visited) // опционально
                return;
            _LastVertex.Visited = true;
            if (m_LineIds.All(_Id => _PathV.Path
                .Where(_I => _I.Visited)
                .Select(_I => _I.Station.LineId).Contains(_Id)))
            {
                Console.WriteLine("Один из путей найден!");
                _PathV.Valid = true;
                return;
            }
            if (_PathV.Path.Count > m_StationsCount / 2) // опционально
                return;
            foreach (var neiberhood in _LastVertex.Neiberhoods.ToArray())
            {
                if (_PathV.Path.Any(_V => _V.Station == neiberhood)) // опционально
                    continue;
                var newVertex = CreateNewVertex(neiberhood);
                if (_LastVertex.Station.LineId == neiberhood.LineId)
                {
                    _PathV.Path.Add(newVertex);
                    FindAllPaths(_PathV, newVertex);
                }
                else
                {
                    var newPath = _PathV.Path.Select(_V => (Vertex) _V.Clone()).ToList();
                    var newPathV = new PathWithValidity {Path = newPath};
                    m_PathsV.Add(newPathV);
                    newPath.Add(newVertex);
                    FindAllPaths(newPathV, newVertex);
                }
            }
        }

        private Vertex CreateNewVertex(MetroStation _Station)
        {
            var vertice = new Vertex {Station = _Station};
            foreach (var edge in m_Graph.Edges)
            {
                if (vertice.Station == edge.Start)
                    vertice.Neiberhoods.Add(edge.End);
                if (vertice.Station == edge.End)
                    vertice.Neiberhoods.Add(edge.Start);
            }
            vertice.Neiberhoods = vertice.Neiberhoods.Distinct().ToList();
            return vertice;
        }

        private PathWithValidity FindShortestPathCore()
        {
            int shortestPathLength = int.MaxValue;
            PathWithValidity shortestPath = null;
            foreach (var pathV in m_PathsV.Where(_Pv => _Pv.Valid))
            {
                int distance = pathV.Path.Count;
                if (distance > shortestPathLength)
                    continue;
                shortestPathLength = distance;
                shortestPath = pathV;
            }
            return shortestPath;
        }

        #endregion
    }
}