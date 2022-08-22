namespace LotterService.Entities
{
    public class DrawInfo
    {
        public DrawInfo()
        {
        }
        public DrawInfo(string drawTime, string numbers)
        {
            DrawTime = drawTime;
            Numbers = numbers.Replace(',', ';');
        }

        public string DrawTime { get; set; }
        public string Numbers{ get; set; }
    }
}
