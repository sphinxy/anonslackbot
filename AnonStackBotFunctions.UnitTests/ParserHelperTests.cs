using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnonSlackBotFunctions.UnitTests
{
    [TestClass]
    public class ParserHelperTests
    {
        private const string CorrectReplyCommandPrefix = ">https://domain.slack.com/archives/AA1B2CCC3";
        private const string WrongReplyCommandPrefix = ">https://domain.slack.com/archives/ZZ9Y8YYY7";
        [DataTestMethod]
        [DataRow(null, CorrectReplyCommandPrefix, null, null)]
        //нет символа >
        [DataRow("https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600", CorrectReplyCommandPrefix, "https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600", null)]
        //корректные варианты
        [DataRow(">https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600", CorrectReplyCommandPrefix, "", "5552465890.058600")]
        [DataRow(">https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600 this is reply", CorrectReplyCommandPrefix, "this is reply", "5552465890.058600")]

        [DataRow("hello, >https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600 this is reply", CorrectReplyCommandPrefix, "hello, this is reply", "5552465890.058600")]
        //всё правильно, но timestamp кривой
        [DataRow(">https://domain.slack.com/archives/AA1B2CCC3/p555246", CorrectReplyCommandPrefix, ">https://domain.slack.com/archives/AA1B2CCC3/p555246", null)]
        //всё правильно, кроме ID канала
        [DataRow(">https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600", WrongReplyCommandPrefix, ">https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600", null)]
        public void RemoveAndParseReplyCommand_Tests(string input, string replyCommandPrefix, string expectedMessageText, string expectedReplyThreadTs)
        {
            var result = ParserHelper.RemoveAndParseReplyCommand(input, replyCommandPrefix);
            Assert.AreEqual(expectedMessageText, result.newMessageText);
            Assert.AreEqual(expectedReplyThreadTs, result.replyThreadTs);
        }

        [DataTestMethod]
        [DataRow("@", ".[removed].")]
        [DataRow("@@", ".[removed]..[removed].")]
        [DataRow("here", ".[removed].")]
        [DataRow("channel", ".[removed].")]
        [DataRow("hello here, @channel", "hello .[removed]., .[removed]..[removed].")]
        [DataRow("sometext @command sometext", "sometext .[removed].command sometext")]

        public void RemoveSlackCommandsFromText_Tests(string input, string expected)
        {
          var result = ParserHelper.RemoveSlackCommandsFromText(input);
          Assert.AreEqual(expected, result);
        }
        

    }
}