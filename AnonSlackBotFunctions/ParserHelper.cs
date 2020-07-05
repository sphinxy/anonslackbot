using System;
using System.Linq;

namespace AnonSlackBotFunctions
{
    public static class ParserHelper
    {
        /// <summary>
        /// ищем в тексте ссылку вида '>https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600',
        /// парсим в 5552465890.058600, а саму ссылку удаляем
        /// </summary>
        public static (string newMessageText, string replyThreadTs) RemoveAndParseReplyCommand(string messageText, string replyCommandPrefix)
        {
            
            var resultAsIs = (messageText, (string) null);
            
            if (string.IsNullOrEmpty(messageText) || string.IsNullOrEmpty(replyCommandPrefix))
            {
                return resultAsIs;
            }

            var replyCommand = messageText.Split(' ')
                .FirstOrDefault(s => s.StartsWith(replyCommandPrefix));

            if (string.IsNullOrEmpty(replyCommand))
            {
                return resultAsIs;
            }
            //https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600 -> 5552465890058600
            var threadTs = replyCommand.Replace($"{replyCommandPrefix}/p", "");
            if (threadTs.Length <= 7)
            {
                return resultAsIs;
            }
            threadTs = threadTs.Insert(threadTs.Length - 6, ".");
            var filteredMessageText = messageText
                .Replace(replyCommand + " ", "")
                .Replace(" " + replyCommand, "")
                .Replace(replyCommand, "");


            return (filteredMessageText, threadTs);
        }

        /// <summary>
        /// простая защита от @here и прочих
        /// </summary>
        public static string RemoveSlackCommandsFromText(string messageText)
        {
            messageText = messageText.Replace("@", ".[removed].");
            messageText = messageText.Replace("here", ".[removed].");
            messageText = messageText.Replace("channel", ".[removed].");
            return messageText;
        }
    }
}