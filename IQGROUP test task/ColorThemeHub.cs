using IQGROUP_test_task.Services;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace IQGROUP_test_task
{
    public class ColorThemeHub : Hub
    {
        public List<string> ConnectedUsersList { get; set; }
        ILogger<ColorThemeHub> _logger;
        IDateTimeService _dateTimeService;

        public ColorThemeHub(ILogger<ColorThemeHub> logger, IDateTimeService dateTimeService)
        {
            ConnectedUsersList = new List<string>();
            _logger = logger;
            _dateTimeService = dateTimeService;
        }

        public async Task SwitchColorTheme(string theme, string id)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();

            try
            {
                string swTheme = theme == "white" ? "dark" : "white";
                // Изменить тему у того, кто обратился к хабу

                _logger.LogInformation($"[{timestamp}] User with id - [{id}] successfully switch theme to [{swTheme}]");
                await this.Clients.Caller.SendAsync("SwitchTheme", swTheme);
                // Отправить уведомление всем остальным, кто подключён к хабу

                _logger.LogInformation($"[{timestamp}] Successfully sended notification to connected users to SignalR hub");
                await this.Clients.Others.SendAsync("NotificateOthers", $"Пользователь с id [{id}] сменил тему на {swTheme}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{timestamp}] Internal SignalR hub error. Details: {ex.Message}");
            }
        }
    }
}
