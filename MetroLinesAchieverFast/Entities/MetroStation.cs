namespace MetroLinesAchieverFast.Entities
{
    public class MetroStation
    {
        public string StationName { get; set; }
        public string LineId      { get; set; }
        
        public static bool operator == (MetroStation _Station1, MetroStation _Station2)
        {
            return _Station1.LineId == _Station2.LineId && _Station1.StationName == _Station2.StationName;
        }

        public static bool operator !=(MetroStation _Station1, MetroStation _Station2)
        {
            return !(_Station1 == _Station2);
        }
    }
}