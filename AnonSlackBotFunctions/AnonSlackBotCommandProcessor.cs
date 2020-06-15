using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AnonSlackBotFunctions
{
    public static class AnonSlackBotCommandProcessor
    {
        private const string SlackBotWebhookUrl = "https://hooks.slack.com/services/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";
        
        /// <summary>
        /// принимаем запрос от бота в формате https://api.slack.com/interactivity/slash-commands#app_command_handling,
        /// парсим и вытаскиваем оттуда text. Далее просто кидаем его на webhook бота, привязанного к конкретному каналу )
        ///
        /// curl --location --request POST 'localhost:7071/api/AnonSlackBotCommandProcessor' \
        /// --header 'Content-Type: application/x-www-form-urlencoded' \
        /// --data-urlencode 'text=sampletext'
        /// </summary>
        [FunctionName("AnonSlackBotCommandProcessor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("Начали парсинг");
            try
            {
                var messageText = req.Form["text"];
                log.LogInformation($"text from form is {messageText}");
                if (!string.IsNullOrEmpty(messageText))
                {
                    using var client = new HttpClient();
                    //json в формате, понимаемом slack
                    var slackMessage = new StringContent($"{{\"text\" : \"{messageText}\"}}");
                    var result = await client.PostAsync(SlackBotWebhookUrl, slackMessage);
                    log.LogInformation($"webhook call result is {result})");
                    result.EnsureSuccessStatusCode();
                    log.LogInformation("отправлено успешно.");
                    return new OkObjectResult("Сообщение анонимно отправлено в канал");
                }
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, "Полный провал");
            }
            log.LogInformation("Ошибка");
            return new OkObjectResult("Кажется, произошла ошибка");
        }
    }
}



