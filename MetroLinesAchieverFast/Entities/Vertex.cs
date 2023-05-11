using System;
using System.Collections.Generic;

namespace MetroLinesAchieverFast.Entities
{
    public class Vertex : ICloneable
    {
        public MetroStation       Station     { get; set; }
        public int                Distance    { get; set; }
        public Vertex             Previous    { get; set; } // только для алгоритма AchieveAllMetroLinesPathFinderLittleAlgorithm, унифицировать не успеваю
        public bool               Visited     { get; set; }
        public List<MetroStation> Neiberhoods { get; set; } = new List<MetroStation>(); // только для алгоритма AchieveAllMetroLinesPathFinderSuggested, унифицировать не успеваю
        
        public object Clone() // только для алгоритма AchieveAllMetroLinesPathFinderSuggested, унифицировать не успеваю
        {
            return new Vertex
            {
                Station     = Station,
                Distance    = Distance,
                Previous    = Previous,
                Visited     = Visited,
                Neiberhoods = Neiberhoods
            };
        }
    }
}