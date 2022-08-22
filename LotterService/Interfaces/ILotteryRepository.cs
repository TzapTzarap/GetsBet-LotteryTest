using LotterService.Entities;

namespace LotterService.Interfaces
{
    public interface ILotteryRepository
    {
        Task<bool> DrawExitsAync(double drawTime);
        Task<bool> SaveDrawResultsAsync(LotteryResult drawResults);
        Task<DrawFrequentNumbers> GetFrequentNumbersAsync(string drawDate);
        Task<IEnumerable<DrawInfo>> GetWinningNumbersByDayAsync(string drawDate);
    }
}
