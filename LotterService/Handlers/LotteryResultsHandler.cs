using LotterService.Entities;
using LotterService.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace LotterService.Handlers
{

    public class LotteryResultsHandler: ILotteryResultsHandler
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _lotteryResultsApiAddress;
        private const string WinningNumbersTag = "winningNumbers";
        private const string WinningNumbersListTag = "list";
        private const string DrawTimeTag = "drawTime";


        public LotteryResultsHandler()
        {
            _httpClient = new HttpClient();
            _lotteryResultsApiAddress = new Uri("https://api.opap.gr/draws/v3.0/1100/last/100");
        }

        public async Task<LotteryResult> GetLotteryResultAync()
        {
            var response = await _httpClient.GetAsync(_lotteryResultsApiAddress);
            var contentStream = response.Content.ReadAsStream();
            var streamReader = new StreamReader(contentStream);
            var jsonReader = new JsonTextReader(streamReader);

            JsonSerializer jsonSerializer = new JsonSerializer();
            var lotteriesResults = jsonSerializer.Deserialize<IEnumerable<JObject>>(jsonReader);

            var drawResult = new LotteryResult();
            if (lotteriesResults == null)
            {
                //log error
                return drawResult;
            }

            foreach (var item in lotteriesResults)
            {
                if (item[WinningNumbersTag] != null && item[WinningNumbersTag][WinningNumbersListTag] != null && item[DrawTimeTag] != null)
                {
                    string drawTime = item[DrawTimeTag].ToString();
                    string winingNumbers = item[WinningNumbersTag][WinningNumbersListTag].ToString();
                    winingNumbers = Regex.Replace(winingNumbers, "[^0-9,]+", "");
                    drawResult = new LotteryResult(drawTime, winingNumbers);

                    break;
                }
            }

            return drawResult;
        }

        public void PublishResults(List<int> numbers)
        {
            new Thread(() =>
            {
                var random = new Random();

                while (numbers.Any())
                {
                    var selectedIndex = random.Next(numbers.Count);
                    var selectedNumber = numbers[selectedIndex];

                    numbers.RemoveAt(selectedIndex);

                    //call publish

                    Task.Delay(3000);
                }
            });
        }
    }
}
