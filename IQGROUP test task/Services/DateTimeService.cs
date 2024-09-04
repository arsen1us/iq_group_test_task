namespace IQGROUP_test_task.Services
{
    public class DateTimeService : IDateTimeService
    {
        ILogger<DateTimeService> _logger;

        public DateTimeService(ILogger<DateTimeService> logger)
        {
            _logger = logger;
        }

        public string GetDateTimeNow()
        {
            _logger.LogInformation($"INFO: [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Now datetime successfully received");
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
