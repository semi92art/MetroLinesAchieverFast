using System.Collections.Generic;
using System.Linq;
using MetroLinesAchieverFast.Entities;

namespace MetroLinesAchieverFast.PathFinders
{
    public class AchieveAllMetroLinesPathFinderLittleAlgorithm : IAchieveAllMetroLinesPathFinder
    {
        #region api

        public List<MetroStation> FindShortestPath(Graph _Graph, string _Start)
        {
            var vertices = GetVertices(_Graph);
            // Начальная вершина
            var startVertex = vertices.First(_V => _V.Station.StationName == _Start);
            startVertex.Distance = 0;
            // Остальные вершины
            var unvisitedVertices = new List<Vertex>(vertices);
            while (unvisitedVertices.Any())
            {
                // Находим вершину с наименьшим расстоянием до начальной вершины
                var currentVertex = unvisitedVertices.OrderBy(_V => _V.Distance).First();

                // Если это конечная вершина, выходим из цикла
                if (_Graph.MetroLineTransferStations.Any(
                    _S => _S == currentVertex.Station))
                {
                    break;
                }

                // Обновляем расстояния до соседних вершин
                foreach (var edge in _Graph.Edges.Where(_E => _E.Start == currentVertex.Station))
                {
                    var neighbourVertex = vertices.Single(_V => _V.Station == edge.End);
                    if (neighbourVertex.Visited || neighbourVertex.Station.LineId == currentVertex.Station.LineId)
                        continue;
                    int distance = currentVertex.Distance + edge.Weight;
                    if (distance >= neighbourVertex.Distance)
                        continue;
                    neighbourVertex.Distance = distance;
                    neighbourVertex.Previous = currentVertex;
                }

                // Помечаем текущую вершину как посещенную
                currentVertex.Visited = true;

                // Удаляем текущую вершину из списка непосещенных вершин
                unvisitedVertices.Remove(currentVertex);
            }

            return GetShortestPath(_Graph, vertices);
        }

        #endregion

        #region nonpublic methods

        private static List<Vertex> GetVertices(Graph _Graph)
        {
            var vertices = new List<Vertex>();
            // Создание списка вершин графа
            foreach (var edge in _Graph.Edges)
            {
                if (vertices.All(_V => _V.Station != edge.Start))
                    vertices.Add(new Vertex {Station = edge.Start});
                if (vertices.All(_V => _V.Station != edge.End))
                    vertices.Add(new Vertex {Station = edge.End});
            }

            return vertices;
        }

        private static List<MetroStation> GetShortestPath(
            Graph                       _Graph,
            IReadOnlyCollection<Vertex> _Vertices)
        {
            // Составление списка станций, входящих в кратчайший путь
            var paths = new List<List<MetroStation>>();
            foreach (var endStation in _Graph.MetroLineTransferStations)
            {
                if (!_Vertices.Any())
                    continue;
                var endVertex = _Vertices.FirstOrDefault(_V => _V.Station == endStation);
                // Собираем путь, начиная с конечной вершины
                var path = new List<MetroStation>();
                var currentVertex = endVertex;
                while (currentVertex != null)
                {
                    path.Add(currentVertex.Station);
                    currentVertex = currentVertex.Previous;
                }

                // Переворачиваем список, чтобы путь начинался с начальной вершины
                path.Reverse();
                paths.Add(path);
            }

            // Находим кратчайший путь из всех возможных
            var shortestPath = paths.OrderBy(_P => _P.Count).FirstOrDefault();

            return shortestPath;
        }

        #endregion
    }
}