namespace LotterService.Entities
{

    public class LotteryResult
    {
        public LotteryResult()
        {
            IsValid = false;
        }
        public LotteryResult(string drawTime, string winingNumbers)
        {
            IsValid = true;
            DrawTime = GetUnixTime(drawTime);
            LocalDrawTime = ConvertToLocalTime(DrawTime);
            WinningNumersList = GetWiningNumbersList(winingNumbers);
            WinningNumbers = winingNumbers;
        }

        public bool IsValid { get; set; }

        private List<int> GetWiningNumbersList(string winingNumbers)
        {
            try
            {
                return winingNumbers.Split(',').Select(Int32.Parse).ToList();
            }
            catch (Exception ex)
            {
                IsValid = false;
                return new List<int>();
            }
        }

        private double GetUnixTime(string drawTime)
        {
            try
            {
                return Convert.ToDouble(drawTime);
            }
            catch (Exception ex)
            {
                IsValid = false;
                return 0;
            }
        }

        private DateTime ConvertToLocalTime(double unixTimeStamp)
        {
            try
            {
                System.DateTime utcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                utcDateTime = utcDateTime.AddMilliseconds(unixTimeStamp);
                var localTime = utcDateTime.ToLocalTime();

                return localTime;
            }
            catch (Exception ex)
            {
                IsValid = false;
                return DateTime.MinValue;
            }
        }

        public double DrawTime { get; set; }
        public string WinningNumbers { get; set; }
        public List<int> WinningNumersList { get; set; }
        public DateTime LocalDrawTime { get; set; }
    }
}
