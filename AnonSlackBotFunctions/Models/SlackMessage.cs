using System.Text.Json.Serialization;

namespace AnonSlackBotFunctions.Models
{
    public class SlackMessage
    {
        [JsonPropertyName("text")]
        public string MessageText { get; }
        [JsonPropertyName("thread_ts")]
        public string ReplyThreadTs { get; }
        public SlackMessage(string messageText, string replyThreadTs)
        {
            MessageText = messageText;
            ReplyThreadTs = replyThreadTs;
        }
    }
}