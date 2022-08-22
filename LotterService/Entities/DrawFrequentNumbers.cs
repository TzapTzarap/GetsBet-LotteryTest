namespace LotterService.Entities
{
    public class DrawFrequentNumbers
    {
        public DrawFrequentNumbers()
        {
            MostFrequent = new List<int>();
            LeastFrequent = new List<int>();
        }
        public string DrawDay { get; set; }
        public List<int> MostFrequent { get; set; }
        public List<int> LeastFrequent { get; set; }
    }
}
