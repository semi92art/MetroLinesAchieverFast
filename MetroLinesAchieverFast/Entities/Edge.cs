namespace MetroLinesAchieverFast.Entities
{
    public class Edge
    {
        public MetroStation Start  { get; set; }
        public MetroStation End    { get; set; }
        public int          Weight { get; set; }
    }
}