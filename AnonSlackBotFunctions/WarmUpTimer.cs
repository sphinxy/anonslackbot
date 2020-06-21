using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AnonSlackBotFunctions
{
    public static class WarmUpTimer
    {
        /// <summary>
        /// Попытка избавится от холодного старта основной функции. Судя по статьям, раз в 4 минуты
        /// (или даже раз в 20 минут) поможет. Вся сборка, не только эта функция, теперь не должна тупить.
        /// </summary>
        [FunctionName("WarmUpTimer")]
        public static void Run([TimerTrigger("0 */4 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
