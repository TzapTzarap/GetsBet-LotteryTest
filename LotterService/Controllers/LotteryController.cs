using LotterService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

namespace LotterService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotteryController : ControllerBase
    {
        private readonly ILotteryRepository _repository;


        public LotteryController(ILotteryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("GetDrawResults")]
        [Produces("application/json")]
        public async Task<ActionResult> GetDrawResultsByDayAsync(DateTime drawDay)
        {
            string date = drawDay.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var drawsList = await _repository.GetWinningNumbersByDayAsync(date);

            if (!drawsList.Any())
            {
                return NotFound();
            }

            return new JsonResult(
                      drawsList,
                      new JsonSerializerOptions
                      {
                          PropertyNamingPolicy = null
                      });
        }

        [HttpGet]
        [Route("GetDrawFrequentNumbers")]
        [Produces("application/json")]
        public async Task<ActionResult> GetDrawFrequentNumbersByDayAsync(DateTime drawDay)
        {
            string date = drawDay.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var frequentNumbers= await _repository.GetFrequentNumbersAsync(date);

            if (!frequentNumbers.MostFrequent.Any())
            {
                return NotFound();
            }

            return new JsonResult(
                      frequentNumbers,
                      new JsonSerializerOptions
                      {
                          PropertyNamingPolicy = null
                      });
        }
    }
}
