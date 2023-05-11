using System.Collections.Generic;

namespace MetroLinesAchieverFast.Entities
{
    public class Graph
    {
        public List<MetroStation> MetroLineTransferStations { get; set; }
        public List<Edge>         Edges                     { get; set; }
    }
}