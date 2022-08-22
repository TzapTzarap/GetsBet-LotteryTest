using LotterService.Interfaces;

namespace LotterService.Workers
{

    public class LotteryWorker : BackgroundService
    {
        private readonly ILotteryResultsHandler _lotteryHandler;
        private readonly ILotteryRepository _lotteryRepository;
        private readonly ILogger<LotteryWorker> _logger;


        public LotteryWorker(ILogger<LotteryWorker> logger, ILotteryRepository lotteryRepository, ILotteryResultsHandler lotteryResultsHandler)
        {
            _logger = logger;
            _lotteryHandler = lotteryResultsHandler;
            _lotteryRepository = lotteryRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var drawResult = await _lotteryHandler.GetLotteryResultAync();

                if (drawResult.IsValid && !await _lotteryRepository.DrawExitsAync(drawResult.DrawTime))
                {
                    await _lotteryRepository.SaveDrawResultsAsync(drawResult);
                    _lotteryHandler.PublishResults(drawResult.WinningNumersList);
                }
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}
