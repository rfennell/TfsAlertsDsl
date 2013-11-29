using System;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.QualityTools.Testing.Fakes;

namespace TFSEventsProcessor.Tests.Dsl
{
    using System.Linq;

    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client.Fakes;
    using Microsoft.TeamFoundation.VersionControl.Client.Fakes;

    using Moq;

    using NLog;

    using TFSEventsProcessor.Providers;

    [TestFixture]
    public class DslTfsProcessingTests
    {

        [Test]
        public void Can_use_Dsl_to_retrieve_a_work_item()
        {

            using (ShimsContext.Create())
            {
                // arrange
                // redirect the console
                var consoleOut = Helpers.Logging.RedirectConsoleOut();

                var wi = new ShimWorkItem() { TitleGet = () => "The wi title", IdGet = () => 99 };
                var emailProvider = new Moq.Mock<IEmailProvider>();
                var tfsProvider = new Moq.Mock<ITfsProvider>();
                tfsProvider.Setup(t => t.GetWorkItem(99)).Returns(wi);
                var engine = new TFSEventsProcessor.Dsl.DslProcessor();

                // act
                engine.RunScript(@"dsl\tfs\loadwi.py", tfsProvider.Object, emailProvider.Object);

                // assert
                Assert.AreEqual(
                    "Work item '99' has the title 'The wi title'" + Environment.NewLine,
                    consoleOut.ToString());
            }
        }

        [Test]
        public void Can_use_Dsl_to_retrieve_a_changeset()
        {
            using (ShimsContext.Create())
            {
                // arrange
                // redirect the console
                var consoleOut = Helpers.Logging.RedirectConsoleOut();

                var cs = new ShimChangeset() { CommentGet = () => "A comment", ChangesetIdGet = () => 99 };
                var emailProvider = new Moq.Mock<IEmailProvider>();
                var tfsProvider = new Moq.Mock<ITfsProvider>();
                tfsProvider.Setup(t => t.GetChangeSet(99)).Returns(cs);
                var engine = new TFSEventsProcessor.Dsl.DslProcessor();

                // act
                engine.RunScript(@"dsl\tfs\loadchangeset.py", tfsProvider.Object, emailProvider.Object);

                // assert
                Assert.AreEqual(
                    "Changeset '99' has the comment 'A comment'" + Environment.NewLine,
                    consoleOut.ToString());
            }
        }

        [Test]
        public void Can_use_Dsl_to_create_a_work_item()
        {

            using (ShimsContext.Create())
            {
                // arrange
                // redirect the console
                var consoleOut = Helpers.Logging.RedirectConsoleOut();

                var wi = new ShimWorkItem() { TitleGet = () => "Title", IdGet = () => 99 };
                var emailProvider = new Moq.Mock<IEmailProvider>();
                var tfsProvider = new Moq.Mock<ITfsProvider>();
                tfsProvider.Setup(t => t.CreateWorkItem(
                    "tp",
                    "Bug",
                    new Dictionary<string, object>()
                    {
                        {"Title", "The Title"},
                        {"Estimate", 2}
                    })).Returns(wi);
                var engine = new TFSEventsProcessor.Dsl.DslProcessor();

                // act
                engine.RunScript(@"dsl\tfs\createwi.py", tfsProvider.Object, emailProvider.Object);

                // assert
              Assert.AreEqual(
                 "Work item '99' has been created with the title 'Title'" + Environment.NewLine,
                 consoleOut.ToString());
            }
        }

        [Test]
        public void Can_pass_realistic_build_arguments_to_script()
        {
            // arrange
            // redirect the console
            var consoleOut = Helpers.Logging.RedirectConsoleOut();


            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            var args = new Dictionary<string, object>
            {
                { "Arguments", new[] { "BuildEvent", "vstfs:///Build/Build/123" } },
            };
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\tfs\fullscript.py", args, tfsProvider.Object, emailProvider.Object);

            // assert

            Assert.AreEqual("A build event vstfs:///Build/Build/123" + Environment.NewLine, consoleOut.ToString());

        }

        [Test]
        public void Can_use_Dsl_to_get_build_details()
        {
            // arrange
            var consoleOut = Helpers.Logging.RedirectConsoleOut();
            var testUri = new Uri("vstfs:///Build/Build/123");

            var emailProvider = new Moq.Mock<IEmailProvider>();
            var build = new Moq.Mock<IBuildDetail>();
            build.Setup(b => b.Uri).Returns(testUri);
            build.Setup(b => b.Quality).Returns("Test Quality");

            var tfsProvider = new Moq.Mock<ITfsProvider>();
            tfsProvider.Setup(t => t.GetBuildDetails(It.IsAny<Uri>())).Returns(build.Object);
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\tfs\loadbuild.py", tfsProvider.Object, emailProvider.Object);
            // assert
            Assert.AreEqual("Build 'vstfs:///Build/Build/123' has the quality 'Test Quality'" + Environment.NewLine, consoleOut.ToString());
        }

        [Test]
        public void Can_use_Dsl_to_set_build_retension()
        {

            // arrange
            var consoleOut = Helpers.Logging.RedirectConsoleOut();
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();
            var testUri = new Uri("vstfs:///Build/Build/123");
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\tfs\keepbuild.py", tfsProvider.Object, emailProvider.Object);

            // assert
            tfsProvider.Verify(t => t.SetBuildRetension(testUri, true));
            Assert.AreEqual(
                "Set build retension for 'vstfs:///Build/Build/123'" + Environment.NewLine,
                consoleOut.ToString());

        }


       
    }
}
