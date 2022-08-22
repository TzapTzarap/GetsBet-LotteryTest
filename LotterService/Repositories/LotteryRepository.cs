using LotterService.Entities;
using LotterService.Interfaces;
using System.Configuration;
using System.Data.SqlClient;

namespace LotterService.Repositories
{
    public class LotteryRepository : ILotteryRepository
    {
        private readonly IConfiguration _configuration;
        
        public LotteryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> DrawExitsAync(double drawTime)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                using (SqlConnection connection = new SqlConnection( _configuration.GetValue<string>("ConnectionStrings:SqlExpressConnection")))
                {
                    SqlCommand cmd = new SqlCommand("spCheckIfDrawExists", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DrawTime", drawTime);
                    connection.Open();

                    return (bool)await cmd.ExecuteScalarAsync();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }
        public async Task<bool> SaveDrawResultsAsync(LotteryResult drawResults)
        {
            bool isSuccess = true;
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                using (SqlConnection connection = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:SqlExpressConnection")))
                {
                    SqlCommand cmd = new SqlCommand("spAddDrawResult", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DrawTime", drawResults.DrawTime);
                    cmd.Parameters.AddWithValue("@WinningNumbers", drawResults.WinningNumbers);
                    cmd.Parameters.AddWithValue("@LocalDrawTime", drawResults.LocalDrawTime);

                    connection.Open();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (SqlException e)
            {

                Console.WriteLine(e.ToString());
                isSuccess = false;
            }
            return isSuccess;
        }
        public async Task<DrawFrequentNumbers> GetFrequentNumbersAsync(string drawDate)
        {
            var drawFrequentNumbers = new DrawFrequentNumbers();
            try
            {
                drawFrequentNumbers.DrawDay = drawDate;

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                using (SqlConnection connection = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:SqlExpressConnection")))
                {
                    SqlCommand cmd = new SqlCommand("spGetFrequentNumbers", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DrawDate", drawDate);

                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        drawFrequentNumbers.LeastFrequent.Add(Convert.ToInt16(reader["least_frequent"].ToString()));
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        drawFrequentNumbers.MostFrequent.Add(Convert.ToInt16(reader["most_frequent"].ToString()));
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return drawFrequentNumbers;
        }
        public async Task<IEnumerable<DrawInfo>> GetWinningNumbersByDayAsync(string drawDate)
        {
            var drawsList = new List<DrawInfo>();
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                using (SqlConnection connection = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:SqlExpressConnection")))
                {
                    SqlCommand cmd = new SqlCommand("spGetDrawsResults", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DrawDate", drawDate);

                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        var drawTime = reader["UnixDrawTime"].ToString();
                        var numbers = reader["WinningNumbers"].ToString();

                        drawsList.Add(new DrawInfo(drawTime, numbers));
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return drawsList;
        }
    }
}
