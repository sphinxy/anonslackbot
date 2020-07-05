using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AnonSlackBotFunctions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace AnonSlackBotFunctions
{
    public static class AnonSlackBotCommandProcessor
    {
        private const string SlackBotWebhookUrlPrefix = "https://hooks.slack.com/services/";
        private const string WebhookPostfixQueryParamName = "webhookPostfix";
        private const string ReplyCommandPrefixQueryParamName = "replyCommandPrefix";

        /// <summary>
        /// принимаем запрос от бота в формате https://api.slack.com/interactivity/slash-commands#app_command_handling,
        /// парсим и вытаскиваем оттуда messageText. Далее просто кидаем его на webhook бота, привязанного к конкретному каналу,
        /// секретная часть от вебхука передаётся через webhookPostfix
        ///
        /// опциональный префикс для команды ответа на тред передаётся через replyCommandPrefix, нужно только для парсинга, например
        /// >hello, >https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600 this is reply
        ///
        /// curl --location --request POST 'localhost:7071/api/AnonSlackBotCommandProcessor?webhookPostfix=T037ZKH7E/B015WFGF6V7/8vmeIRsNSScwlrb4rwNfTa7A&channelId=AA1B2CCC3 \
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
                var webhookPostfix = req.Query[WebhookPostfixQueryParamName].FirstOrDefault();
                if (string.IsNullOrEmpty(webhookPostfix))
                {
                    return new OkObjectResult($"Ошибка. Не передан query-параметр {WebhookPostfixQueryParamName} для {SlackBotWebhookUrlPrefix}[...]!");
                }
                
                var messageText = (string)req.Form["text"];
                log.LogInformation($"text from form is {messageText}");
                var filteredMessageText = ParserHelper.RemoveSlackCommandsFromText(messageText);
                log.LogInformation($"filtered message is {filteredMessageText}");

                //ищем, а нет ли там reply-команды в тексте
                var replyCommandPrefix = req.Query[ReplyCommandPrefixQueryParamName].FirstOrDefault();
                var replyCommandData = ParserHelper.RemoveAndParseReplyCommand(filteredMessageText, replyCommandPrefix);
                log.LogInformation($"reply thread is {replyCommandData.replyThreadTs} (for replyCommandPrefix = {replyCommandPrefix})");
                var finalMessageText = replyCommandData.newMessageText;
                log.LogInformation($"final message is {finalMessageText}");

                if (!string.IsNullOrEmpty(finalMessageText) )
                {
                    using var client = new HttpClient {BaseAddress = new Uri(SlackBotWebhookUrlPrefix)};
                    
                    var slackMessage = new SlackMessage(finalMessageText, replyCommandData.replyThreadTs);
                    var slackMessageJson = new StringContent(JsonSerializer.Serialize(slackMessage));
                    var result = await client.PostAsync(webhookPostfix, slackMessageJson);
                    result.EnsureSuccessStatusCode();
                    log.LogInformation("отправлено успешно.");
                    return new OkResult();
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



