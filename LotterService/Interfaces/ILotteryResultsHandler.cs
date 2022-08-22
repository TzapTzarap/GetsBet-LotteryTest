using LotterService.Entities;

namespace LotterService.Interfaces
{
    public interface ILotteryResultsHandler
    {
        Task<LotteryResult> GetLotteryResultAync();
        void PublishResults(List<int> numbers);
    }
}
