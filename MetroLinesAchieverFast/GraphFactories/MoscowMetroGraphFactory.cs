using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetroLinesAchieverFast.Entities;
using MetroLinesAchieverFast.Utils;
using Newtonsoft.Json;

namespace MetroLinesAchieverFast.GraphFactories
{
    public class MoscowMetroGraphFactory : IMetroGraphFactory
    {
        #region raw data types group 1
        
        private class StationsOnLineRawData
        {
            public List<StationGeography> Stations { get; set; }
        }

        private class StationGeography
        {
            public float  Lat    { get; set; }
            public float  Lng   { get; set; }
            public string Name   { get; set; }
        }

        #endregion

        #region raw data types group 2

        private class MetroMapData
        {
            public Dictionary<string, List<string>> Stations    { get; set; }
            public List<List<ConnectionNode>>       Connections { get; set; }
        }
        
        private class ConnectionNode
        {
            public string LineId { get; set; }
            public string Name   { get; set; }
        }
        
        #endregion

        #region api

        public Graph Create()
        {
            var rawData1 = ParseRawDataFile1();
            var rawData2 = ParseRawDataFile2();
            return CreateFromRawData(rawData1, rawData2);
        }
        
        #endregion 

        
        #region nonpublic methods
        
        private static IEnumerable<StationsOnLineRawData> ParseRawDataFile1()
        {
            // https://raw.githubusercontent.com/morphey83/MetroAndDistrict/master/metro.msk.json
            using var sReader1 = File.OpenText("stations_geography.json");
            var serializer1 = new JsonSerializer();
            var deserializedObj1 = serializer1.Deserialize(sReader1, typeof(List<StationsOnLineRawData>));
            if (!(deserializedObj1 is List<StationsOnLineRawData> linesAndStationsRawData))
                throw new Exception("Wrong format for raw data file 1");
            return linesAndStationsRawData;
        }

        private static MetroMapData ParseRawDataFile2()
        {
            // https://github.com/TechnoidRus/MoscowMetroParser - тут нужно отдельной программой создавать файл
            using var sReader2 = File.OpenText("map.json");
            var serializer2 = new JsonSerializer();
            var deserializedObj2 = serializer2.Deserialize(sReader2, typeof(MetroMapData));
            if (!(deserializedObj2 is MetroMapData metroMapData))
                throw new Exception("Wrong format for raw data file 2");
            return metroMapData;
        }
        
        private static Graph CreateFromRawData(
            IEnumerable<StationsOnLineRawData> _RawData1,
            MetroMapData                       _RawData2)
        {
            var allStationsData = _RawData1
                .SelectMany(_D => _D.Stations)
                .ToList();
            var edges = new List<Edge>();
            foreach (var lineAndstationsKvp in _RawData2.Stations)
            {
                var stations = lineAndstationsKvp.Value;
                for (int i = 0; i < stations.Count - 1; i++)
                {
                    var station1 = allStationsData.FirstOrDefault(_S => _S.Name == stations[i]);
                    var station2 = allStationsData.FirstOrDefault(_S => _S.Name == stations[i]);
                    if (station1 == null || station2 == null)
                        continue;
                    var edge = new Edge
                    {
                        Start  = new MetroStation{ StationName = stations[i], LineId = lineAndstationsKvp.Key},
                        End    = new MetroStation{ StationName = stations[i + 1], LineId = lineAndstationsKvp.Key},
                        Weight = CalculateWeight(station1, station2),
                    };
                    edges.Add(edge);
                }
            }
            var startNodes = new List<MetroStation>();
            foreach (var connection in _RawData2.Connections)
            {
                for (int i = 1; i < connection.Count; i++)
                {
                    var edge = new Edge
                    {
                        Start  = new MetroStation {StationName = connection[0].Name, LineId = connection[0].LineId},
                        End    = new MetroStation {StationName = connection[i].Name, LineId = connection[i].LineId},
                        Weight = 1
                    };
                    edges.Add(edge);
                }
            }
            foreach (var connection in _RawData2.Connections
                .SelectMany(_Connections => _Connections))
            {
                var node = new MetroStation
                {
                    LineId      = connection.LineId,
                    StationName = connection.Name
                };
                startNodes.Add(node);
            }
            var result = new Graph
            {
                Edges                     = edges,
                MetroLineTransferStations = startNodes
            };
            return result;
        }
        
        private static int CalculateWeight(StationGeography _Station1, StationGeography _Station2)
        {
            return (int)DistanceCalculator.Calculate(
                _Station1.Lat, _Station1.Lng, _Station2.Lat, _Station2.Lng);
        }
        
        #endregion
    }
}