using NUnit.Framework;
using NLog;

namespace TFSEventsProcessor.Tests.Dsl
{
    using TFSEventsProcessor.Providers;

    [TestFixture]
    public class DslLoggingProcessingTests
    {
   
    
        [Test]
        public void Can_log_debug_message_to_nlog()
        {
            // arrange
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            // create a memory logger
            var memLogger = Helpers.Logging.CreateMemoryTargetLogger(LogLevel.Debug);
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\logging\logmessage.py", tfsProvider.Object, emailProvider.Object);

            // assert
            Assert.AreEqual(6, memLogger.Logs.Count);
            // memLogger.Logs[0] is the log message from the runscript method
            // memLogger.Logs[1] is the log message from the runscript method
            Assert.AreEqual("DEBUG | TFSEventsProcessor.Dsl.DslLibrary | This is a debug line", memLogger.Logs[2]);
            Assert.AreEqual("INFO | TFSEventsProcessor.Dsl.DslLibrary | This is a info line", memLogger.Logs[3]);
            Assert.AreEqual("ERROR | TFSEventsProcessor.Dsl.DslLibrary | This is an error line", memLogger.Logs[4]);
            Assert.AreEqual("DEBUG | TFSEventsProcessor.Dsl.DslLibrary | List: [List item 1] [List item 2] ", memLogger.Logs[5]);

        }

        [Test]
        public void Can_log_info_and_error_messages_only_to_nlog()
        {
            // arrange
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            // create a memory logger
            var memLogger = Helpers.Logging.CreateMemoryTargetLogger(LogLevel.Info);
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\logging\logmessage.py", tfsProvider.Object, emailProvider.Object);

            // assert
            Assert.AreEqual(4, memLogger.Logs.Count);
            // memLogger.Logs[0] is the log message from the runscript method
            // memLogger.Logs[1] is the log message from the runscript method
            Assert.AreEqual("INFO | TFSEventsProcessor.Dsl.DslLibrary | This is a info line", memLogger.Logs[2]);
            Assert.AreEqual("ERROR | TFSEventsProcessor.Dsl.DslLibrary | This is an error line", memLogger.Logs[3]);

        }

        [Test]
        public void Can_log_error_messages_only_to_nlog()
        {
            // arrange
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            // create a memory logger
            var memLogger = Helpers.Logging.CreateMemoryTargetLogger(LogLevel.Error);
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\logging\logmessage.py", tfsProvider.Object, emailProvider.Object);

            // assert
            Assert.AreEqual(1, memLogger.Logs.Count);
            Assert.AreEqual("ERROR | TFSEventsProcessor.Dsl.DslLibrary | This is an error line", memLogger.Logs[0]);

        }

     
    }
}
