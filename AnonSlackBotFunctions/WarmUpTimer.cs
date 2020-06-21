using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AnonSlackBotFunctions
{
    public static class WarmUpTimer
    {
        /// <summary>
        /// ������� ��������� �� ��������� ������ �������� �������. ���� �� �������, ��� � 4 ������
        /// (��� ���� ��� � 20 �����) �������. ��� ������, �� ������ ��� �������, ������ �� ������ ������.
        /// </summary>
        [FunctionName("WarmUpTimer")]
        public static void Run([TimerTrigger("0 */4 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
