using System;
using NUnit.Framework;
using TFSEventsProcessor.Tests.Helpers;

namespace TFSEventsProcessor.Tests.Dsl
{
    using TFSEventsProcessor.Providers;

    [TestFixture]
    public class DslWithEventMessagesTests
    {
        [Test]
        public void Can_run_a_script_using_work_item_data()
        {
            // arrange
            var alertMessage = TestData.DummyWorkItemChangedAlertXml();
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();
            var sut = new DslScriptService(emailProvider.Object, tfsProvider.Object, @"dsl\tfs\fullscript.py");

            // redirect the console
            var consoleOut = Helpers.Logging.RedirectConsoleOut();
            
            // act
            sut.Notify(alertMessage, string.Empty);

            // assert
            Assert.AreEqual("A wi event 416" + Environment.NewLine, consoleOut.ToString());

        }

        [Test]
        public void Can_run_a_script_using_build_data()
        {
            // arrange
            var alertMessage = TestData.DummyBuildStatusChangedAlertXmlWithQualityChange();
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();
            var sut = new DslScriptService(emailProvider.Object, tfsProvider.Object, @"dsl\tfs\fullscript.py");

            // redirect the console
            var consoleOut = Helpers.Logging.RedirectConsoleOut();

            // act
            sut.Notify(alertMessage, string.Empty);

            // assert
            Assert.AreEqual("A build event vstfs:///Build/Build/49" + Environment.NewLine, consoleOut.ToString());

        }

        [Test]
        public void Can_run_a_script_using_checkin_data()
        {
            // arrange
            var alertMessage = TestData.DummyCheckInAlertXml();
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();
            var sut = new DslScriptService(emailProvider.Object, tfsProvider.Object, @"dsl\tfs\fullscript.py");

            // redirect the console
            var consoleOut = Helpers.Logging.RedirectConsoleOut();

            // act
            sut.Notify(alertMessage, string.Empty);

            // assert
            Assert.AreEqual("A checkin event 62" + Environment.NewLine, consoleOut.ToString());

        }
    }
}
