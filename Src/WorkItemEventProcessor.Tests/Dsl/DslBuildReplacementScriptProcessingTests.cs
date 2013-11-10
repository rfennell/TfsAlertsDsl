using System;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.QualityTools.Testing.Fakes;

namespace TFSEventsProcessor.Tests.Dsl
{
    using IronPython.Hosting;

    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client.Fakes;
    using Microsoft.TeamFoundation.VersionControl.Client.Fakes;

    using Moq;

    using NLog;

    using TFSEventsProcessor.Providers;

    [TestFixture]
    public class DslBuildReplacementScriptProcessingTests
    {

        [Test]
        public void Can_use_Dsl_to_set_build_retension_by_quality()
        {

            // arrange
            var memLogger = Helpers.Logging.CreateMemoryTargetLogger(LogLevel.Debug);
   
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            var testUri = new Uri("vstfs:///Build/Build/123");
            var build = new Moq.Mock<IBuildDetail>();
            build.Setup(b => b.Uri).Returns(testUri);
            build.Setup(b => b.Quality).Returns("Test Quality");
            build.Setup(b => b.BuildNumber).Returns("CTAppBox.Main.CI_1.5.15.6731");

            tfsProvider.Setup(t => t.GetBuildDetails(It.IsAny<Uri>())).Returns(build.Object);

            var engine = new TFSEventsProcessor.Dsl.DslProcessor();
            var args = new Dictionary<string, object>
            {
                { "Arguments", new[] { "BuildEvent", "vstfs:///Build/Build/123" } },
            };

            // act
            engine.RunScript(@"dsl\tfs\setbuildretensionbyquality.py", args, tfsProvider.Object, emailProvider.Object);

            // assert
            tfsProvider.Verify(t => t.SetBuildRetension(testUri, true));
            emailProvider.Verify(e => e.SendEmailAlert("richard@typhoontfs", "CTAppBox.Main.CI_1.5.15.6731 quality changed", "'CTAppBox.Main.CI_1.5.15.6731' retension set to 'True' as quality was changed to 'Test Quality'"));
       
            Assert.AreEqual(3, memLogger.Logs.Count);
            // memLogger.Logs[0] is the log message from the runscript method
            // memLogger.Logs[1] is the log message from the runscript method
            Assert.AreEqual("INFO | TFSEventsProcessor.Dsl.DslLibrary | 'CTAppBox.Main.CI_1.5.15.6731' retension set to 'True' as quality was changed to 'Test Quality'", memLogger.Logs[2]);
        

        }

        [Test]
        public void Can_use_Dsl_to_unset_build_retension_by_quality()
        {

            // arrange
            var memLogger = Helpers.Logging.CreateMemoryTargetLogger(LogLevel.Debug);

            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            var testUri = new Uri("vstfs:///Build/Build/123");
            var build = new Moq.Mock<IBuildDetail>();
            build.Setup(b => b.Uri).Returns(testUri);
            build.Setup(b => b.Quality).Returns("Test Failed");
            build.Setup(b => b.BuildNumber).Returns("CTAppBox.Main.CI_1.5.15.6731");

            tfsProvider.Setup(t => t.GetBuildDetails(It.IsAny<Uri>())).Returns(build.Object);

            var engine = new TFSEventsProcessor.Dsl.DslProcessor();
            var args = new Dictionary<string, object>
            {
                { "Arguments", new[] { "BuildEvent", "vstfs:///Build/Build/123" } },
            };

            // act
            engine.RunScript(@"dsl\tfs\setbuildretensionbyquality.py", args, tfsProvider.Object, emailProvider.Object);

            // assert
            tfsProvider.Verify(t => t.SetBuildRetension(testUri, false));
            emailProvider.Verify(e => e.SendEmailAlert("richard@typhoontfs", "CTAppBox.Main.CI_1.5.15.6731 quality changed", "'CTAppBox.Main.CI_1.5.15.6731' retension set to 'False' as quality was changed to 'Test Failed'"));

            Assert.AreEqual(3, memLogger.Logs.Count);
            // memLogger.Logs[0] is the log message from the runscript method
            // memLogger.Logs[1] is the log message from the runscript method
            Assert.AreEqual("INFO | TFSEventsProcessor.Dsl.DslLibrary | 'CTAppBox.Main.CI_1.5.15.6731' retension set to 'False' as quality was changed to 'Test Failed'", memLogger.Logs[2]);


        }


        [Test]
        public void Get_error_log_if_pass_invalid_eventype_provided()
        {

            // arrange
            var memLogger = Helpers.Logging.CreateMemoryTargetLogger(LogLevel.Error);

            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            var engine = new TFSEventsProcessor.Dsl.DslProcessor();
            var args = new Dictionary<string, object>
            {
                { "Arguments", new[] { "Invalidstring", "ignored" } },
            };

            // act
            engine.RunScript(@"dsl\tfs\setbuildretensionbyquality.py", args, tfsProvider.Object, emailProvider.Object);

            // assert
            Assert.AreEqual(2, memLogger.Logs.Count);
            Assert.AreEqual("ERROR | TFSEventsProcessor.Dsl.DslLibrary | Was not expecting to get here", memLogger.Logs[0]);
            Assert.AreEqual("ERROR | TFSEventsProcessor.Dsl.DslLibrary | List: [Invalidstring] [ignored] ", memLogger.Logs[1]);

        }
    }
}
